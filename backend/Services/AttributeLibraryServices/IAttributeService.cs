using System;
using System.Threading.Tasks;
using server.Entities;

namespace server.Services.AttributeLibraryServices;

public interface IAttributeService
{
    Task<AppAttribute> GetAttributeEntityByIdAsync(Guid id);
    Task<System.Collections.Generic.List<server.Dto.AttributeCategoryDto>> GetCategoriesAsync();
    Task<System.Collections.Generic.List<AttributeType>> GetAttributeTypesAsync();
    Task<server.Dto.AttributeDto> CreateAttributeAsync(server.Dto.CreateAttributeDto dto);
    Task<server.Dto.AttributeDto> UpdateAttributeAsync(Guid id, server.Dto.UpdateAttributeDto dto);
    Task<bool> DeleteAttributeAsync(Guid id);
    Task<server.Dto.AttributeDto> GetAttributeDtoByIdAsync(Guid id);
}
