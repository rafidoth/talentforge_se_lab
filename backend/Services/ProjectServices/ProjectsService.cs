using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using server.Data;
using server.Dto;

namespace server.Services.ProjectServices;

public class ProjectsService(ApplicationDbContext db) : IProjectsService
{
    public Task<List<ProjectDto>> GetCandidateProjectsAsync(string userId)
    {
        throw new NotImplementedException();
    }
}
