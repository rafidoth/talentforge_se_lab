using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using server.Data;
using server.Dto;
using server.Entities;

namespace server.Services.ProfileServices;

public class ProfileService(ApplicationDbContext db) : IProfileService
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
}
