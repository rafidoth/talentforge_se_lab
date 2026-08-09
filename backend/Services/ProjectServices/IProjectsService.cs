using System.Collections.Generic;
using System.Threading.Tasks;
using server.Dto;

namespace server.Services.ProjectServices;

public interface IProjectsService
{
    Task<List<ProjectDto>> GetCandidateProjectsAsync(string userId);
    Task<ProjectDto> CreateProjectAsync(string userId, CreateProjectDto dto);
}
