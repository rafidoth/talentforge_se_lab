using System.Threading.Tasks;
using server.Dto;
using server.Entities;

namespace server.Services.ProfileServices;

public interface IProfileService
{
    Task<MeSectionDto> GetMeSectionAsync(string userId);
    Task<MeSectionDto> UpdateMeSectionAsync(ApplicationUser user, UpdateMeSectionDto dto);
    Task AddAttributeToProfileAsync(string userId, AddProfileAttributeDto dto);
    Task UpdateAttributeValueInProfileAsync(string userId, UpdateProfileAttributeValueDto dto);
    Task<System.Collections.Generic.List<server.Dto.ProfileAttributeDto>> GetNonBuiltInAttributesAsync(string userId);
}
