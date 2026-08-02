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
}
