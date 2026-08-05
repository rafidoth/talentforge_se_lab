using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using server.Data;
using server.Entities;
using System.Linq;

namespace server.Services.AttributeLibraryServices;

public class AttributeService(ApplicationDbContext db) : IAttributeService
{
    public async Task<AppAttribute> GetAttributeEntityByIdAsync(Guid id)
    {
        return await db.Attributes.Include(a => a.Type).FirstOrDefaultAsync(a => a.Id == id) 
            ?? throw new Exception("Attribute not found");
    }

    public async Task<System.Collections.Generic.List<server.Dto.AttributeCategoryDto>> GetCategoriesAsync()
    {
        var categories = await db.AttributeCategories.ToListAsync();
        return categories.Select(c => new server.Dto.AttributeCategoryDto(c.Id, c.Name)).ToList();
    }

    public async Task<System.Collections.Generic.List<AttributeType>> GetAttributeTypesAsync()
    {
        var types = await db.AttributeTypes.ToListAsync();
        if (types == null || types.Count == 0)
            throw new Exception("No attribute types found.");
        return types;
    }

    public async Task<server.Dto.AttributeDto> CreateAttributeAsync(server.Dto.CreateAttributeDto dto)
    {
        var attribute = new AppAttribute
        {
            Id = Guid.NewGuid(),
            Name = dto.Name,
            TypeId = dto.TypeId,
            CategoryId = dto.CategoryId,
            Description = dto.Description,
            IsBuiltin = false
        };

        if (dto.DropdownOptions != null && dto.DropdownOptions.Count > 0)
        {
            foreach (var label in dto.DropdownOptions)
            {
                attribute.DropdownOptions.Add(new AttributeDropdownOption
                {
                    Id = Guid.NewGuid(),
                    AttributeId = attribute.Id,
                    Label = label
                });
            }
        }

        db.Attributes.Add(attribute);
        await db.SaveChangesAsync();

        return await GetAttributeDtoByIdAsync(attribute.Id);
    }
    
    public async Task<server.Dto.AttributeDto> GetAttributeDtoByIdAsync(Guid id)
    {
        var attribute = await GetAttributeEntityByIdAsync(id);
        var version = db.Entry(attribute).Property<uint>("Version").CurrentValue;
        return new server.Dto.AttributeDto
        {
            Id = attribute.Id,
            Name = attribute.Name,
            TypeId = attribute.TypeId ?? 0,
            TypeName = attribute.Type?.Name ?? string.Empty,
            CategoryId = attribute.CategoryId ?? 0,
            CategoryName = attribute.Category?.Name ?? string.Empty,
            IsBuiltin = attribute.IsBuiltin,
            DropdownOptions = attribute.DropdownOptions.Select(o => new server.Dto.DropdownOptionDto(o.Id, o.Label)).ToList(),
            Version = version
        };
    }

    public async Task<server.Dto.AttributeDto> UpdateAttributeAsync(Guid id, server.Dto.UpdateAttributeDto dto)
    {
        using var transaction = await db.Database.BeginTransactionAsync();
        var attribute = await GetAttributeEntityByIdAsync(id);

        if (attribute.IsBuiltin)
            throw new Exception("Built-in attributes cannot be modified.");

        if (!string.IsNullOrWhiteSpace(dto.Name))
            attribute.Name = dto.Name;
            
        if (!string.IsNullOrWhiteSpace(dto.Description))
            attribute.Description = dto.Description;

        db.Entry(attribute).Property<uint>("Version").OriginalValue = dto.Version;

        if (dto.DropdownOptions != null)
        {
            db.AttributeDropdownOptions.RemoveRange(attribute.DropdownOptions);
            foreach (var label in dto.DropdownOptions)
            {
                db.AttributeDropdownOptions.Add(new AttributeDropdownOption
                {
                    Id = Guid.NewGuid(),
                    AttributeId = attribute.Id,
                    Label = label
                });
            }
        }

        try
        {
            await db.SaveChangesAsync();
            await transaction.CommitAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            throw new Exception("The attribute was modified by another user. Please refresh and try again.");
        }
        catch (DbUpdateException)
        {
            throw new Exception($"An attribute with the name '{dto.Name}' already exists.");
        }

        return await GetAttributeDtoByIdAsync(attribute.Id);
    }

    public async Task<bool> DeleteAttributeAsync(Guid id)
    {
        var attribute = await GetAttributeEntityByIdAsync(id);

        if (attribute.IsBuiltin)
            throw new Exception("Built-in attributes cannot be deleted.");

        db.Attributes.Remove(attribute);
        await db.SaveChangesAsync();
        return true;
    }

    public Task<server.Utils.PagedResponse<server.Dto.AttributeDto>> SearchAsync(server.Dto.AttributeSearchQueryDto dto)
    {
        throw new NotImplementedException();
    }
}
