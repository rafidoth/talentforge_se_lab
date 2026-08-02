using System.Threading.Tasks;
using server.Dto;
using server.Entities;

namespace server.Services.ProfileServices;

public interface IProfileService
{
    Task<MeSectionDto> GetMeSectionAsync(string userId);
    Task<MeSectionDto> UpdateMeSectionAsync(ApplicationUser user, UpdateMeSectionDto dto);
}
