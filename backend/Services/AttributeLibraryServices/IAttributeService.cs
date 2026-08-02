using System;
using System.Threading.Tasks;
using server.Entities;

namespace server.Services.AttributeLibraryServices;

public interface IAttributeService
{
    Task<AppAttribute> GetAttributeEntityByIdAsync(Guid id);
}
