using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using server.Data;
using server.Dto;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace server.Services.ProjectServices;

public class ProjectsService(ApplicationDbContext db) : IProjectsService
{
    public async Task<List<ProjectDto>> GetCandidateProjectsAsync(string userId)
    {
        var projects = await db.Projects
            .Include(p => p.ProjectTechnologyTags)
                .ThenInclude(pt => pt.Tag)
            .Where(p => p.UserId == userId)
            .ToListAsync();
            
        return projects.Select(p => new ProjectDto
        {
            Id = p.Id,
            Name = p.Name,
            StartDate = p.StartDate,
            EndDate = p.EndDate,
            Description = p.Description,
            Tags = p.ProjectTechnologyTags.Select(pt => new TagDto(pt.Tag.Id, pt.Tag.Name)).ToList(),
            Version = EF.Property<uint>(p, "Version")
        }).ToList();
    }
}
