using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using server.Data;
using server.Entities;

namespace server.Services.AttributeLibraryServices;

public class AttributeService(ApplicationDbContext db) : IAttributeService
{
    public async Task<AppAttribute> GetAttributeEntityByIdAsync(Guid id)
    {
        return await db.Attributes.Include(a => a.Type).FirstOrDefaultAsync(a => a.Id == id) 
            ?? throw new Exception("Attribute not found");
    }
}
