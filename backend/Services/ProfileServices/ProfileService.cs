using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using server.Data;
using server.Dto;
using server.Entities;

namespace server.Services.ProfileServices;

public class ProfileService(ApplicationDbContext db, server.Services.AttributeLibraryServices.IAttributeService attrs) : IProfileService
{
    public async Task<MeSectionDto> GetMeSectionAsync(string userId)
    {
        var meAttributes = await db.ProfileAttributes
            .Where(
                pa => pa.UserId == userId &&
                pa.Attribute.Category!.Name == "Personal Information" &&
                pa.Attribute.IsBuiltin == true
            )
            .Include(pa => pa.Attribute)
                .ThenInclude(a => a.DropdownOptions)
            .Include(pa => pa.Attribute)
                .ThenInclude(a => a.Category)
            .Include(pa => pa.Attribute)
                .ThenInclude(a => a.Type)
            .Select(pa => new ProfileAttributeDto
            {
                Id = pa.Id,
                AttributeId = pa.AttributeId,
                AttributeName = pa.Attribute.Name,
                TypeName = pa.Attribute.Type!.Name,
                CategoryName = pa.Attribute.Category!.Name,
                Value = pa.Value,
                Version = EF.Property<uint>(pa, "Version"),
                DropdownOptions = pa.Attribute.DropdownOptions.Select(d => new DropdownOptionDto(d.Id, d.Label)).ToList()
            })
            .ToListAsync();

        return new MeSectionDto
        {
            MeAttributes = meAttributes
        };
    }

    public async Task<MeSectionDto> UpdateMeSectionAsync(
        ApplicationUser user,
        UpdateMeSectionDto dto
    )
    {
        try
        {
            await UpdateMeSectionAttributes(user, dto.Attributes);
            return await GetMeSectionAsync(user.Id);
        }
        catch (DbUpdateConcurrencyException)
        {
            throw new Exception("Conflict: Refresh and Try Again");
        }
    }

    private async Task UpdateMeSectionAttributes(
        ApplicationUser user, System.Collections.Generic.List<UpdateProfileAttributeValueDto> updates
    )
    {
        var profileAttributeIds = updates.Select(a => a.ProfileAttributeId).ToList();
        var profileAttributes = await GetProfileAttributesByIdsAsync(user.Id, profileAttributeIds);
        ApplyUpdatesToProfileAttributes(profileAttributes, updates);
        await db.SaveChangesAsync();
    }

    private void ApplyUpdatesToProfileAttributes(
        System.Collections.Generic.List<ProfileAttribute> profileAttributes, System.Collections.Generic.List<UpdateProfileAttributeValueDto> updates
    )
    {
        foreach (var update in updates)
        {
            var profileAttribute = profileAttributes.First(pa => pa.Id == update.ProfileAttributeId);
            UpdateProfileAttributeValue(profileAttribute, update.Value, update.Version);
        }
    }

    private void UpdateProfileAttributeValue(ProfileAttribute profileAttribute, System.Text.Json.JsonElement newValue, uint existingVersion)
    {
        profileAttribute.Value = newValue;
        profileAttribute.UpdatedAt = DateTime.UtcNow;

        db.Entry(profileAttribute).Property<uint>("Version").OriginalValue = existingVersion;
    }

    private async Task<System.Collections.Generic.List<ProfileAttribute>> GetProfileAttributesByIdsAsync(string userId, System.Collections.Generic.List<Guid> profileAttributeIds)
    {
        var profileAttributes = await db.ProfileAttributes
            .Include(pa => pa.Attribute)
                .ThenInclude(a => a.Type)
            .Where(pa => pa.UserId == userId && profileAttributeIds.Contains(pa.Id))
            .ToListAsync();

        if (profileAttributes.Count != profileAttributeIds.Count)
        {
            throw new Exception("Some profile attributes were not found.");
        }

        return profileAttributes;
    }
    
    public async Task AddAttributeToProfileAsync(string userId, AddProfileAttributeDto dto)
    {
        var attribute = await attrs.GetAttributeEntityByIdAsync(dto.AttributeId);
        ValidateAttributeValueType(attribute, dto.Value);
        var profileAttribute = new ProfileAttribute
        {
            UserId = userId,
            AttributeId = attribute.Id,
            Value = dto.Value,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        await db.ProfileAttributes.AddAsync(profileAttribute);
        await db.SaveChangesAsync();
    }

    private void ValidateAttributeValueType(AppAttribute attribute, System.Text.Json.JsonElement value)
    {
        var stringRelatedTypes = new System.Collections.Generic.List<string> {
            AttributeTypes.String, AttributeTypes.Text, AttributeTypes.Image, AttributeTypes.OneToMany
        };

        if (stringRelatedTypes.Contains(attribute.Type!.Name))
        {
            if (value.ValueKind != System.Text.Json.JsonValueKind.String)
                throw new Exception($"Invalid value type for attribute '{attribute.Name}'. Expected a string.");
        }
    }

    public async Task UpdateAttributeValueInProfileAsync(
            string userId,
            UpdateProfileAttributeValueDto dto
    )
    {
        var profileAttribute = await db.ProfileAttributes
            .Include(pa => pa.Attribute)
                .ThenInclude(a => a.Type)
            .FirstOrDefaultAsync(pa => pa.Id == dto.ProfileAttributeId && pa.UserId == userId) ??
            throw new Exception("Profile Attribute not found");
            
        try
        {
            ValidateAttributeValueType(profileAttribute.Attribute, dto.Value);
            profileAttribute.Value = dto.Value;
            profileAttribute.UpdatedAt = DateTime.UtcNow;
            db.Entry(profileAttribute).Property<uint>("Version").OriginalValue = dto.Version;
            await db.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            throw new Exception("Conflict: Refresh and Try Again");
        }
    }

    public async Task<System.Collections.Generic.List<server.Dto.ProfileAttributeDto>> GetNonBuiltInAttributesAsync(string userId)
    {
        var attributes = await db.ProfileAttributes
            .Include(pa => pa.Attribute)
                .ThenInclude(a => a.Type)
            .Include(pa => pa.Attribute)
                .ThenInclude(a => a.Category)
            .Include(pa => pa.Attribute)
                .ThenInclude(a => a.DropdownOptions)
            .Where(pa => pa.UserId == userId && !pa.Attribute.IsBuiltin)
            .ToListAsync();
            
        return attributes.Select(pa => new server.Dto.ProfileAttributeDto
        {
            Id = pa.Id,
            AttributeId = pa.AttributeId,
            AttributeName = pa.Attribute.Name,
            TypeName = pa.Attribute.Type!.Name,
            CategoryName = pa.Attribute.Category!.Name,
            Value = pa.Value,
            Version = EF.Property<uint>(pa, "Version"),
            DropdownOptions = pa.Attribute.DropdownOptions.Select(d => new server.Dto.DropdownOptionDto(d.Id, d.Label)).ToList()
        }).ToList();
    }

    public async Task<server.Dto.CandidateFullProfileDto> GetCandidateFullProfileAsync(string candidateId)
    {
        var user = await db.Users.FindAsync(candidateId);
        if (user == null) throw new Exception("Candidate not found");
        
        return new server.Dto.CandidateFullProfileDto
        {
            CandidateId = candidateId,
            InfoSection = new server.Dto.InfoSectionDto
            {
                Email = user.Email ?? "",
                Status = user.Status ?? "",
                JoinedAt = user.JoinedAt ?? DateTime.UtcNow
            }
        };
    }
}
