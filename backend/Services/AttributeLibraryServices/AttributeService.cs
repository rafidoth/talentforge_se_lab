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
}
