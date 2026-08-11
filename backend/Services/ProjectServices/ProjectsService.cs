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

    public async Task<ProjectDto> CreateProjectAsync(string userId, CreateProjectDto dto)
    {
        using var transaction = await db.Database.BeginTransactionAsync();
        var project = new server.Entities.Project
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Name = dto.Name,
            StartDate = dto.StartDate,
            EndDate = dto.EndDate,
            Description = dto.Description
        };

        if (dto.Tags != null && dto.Tags.Count > 0)
        {
            var tags = await db.TechnologyTags.Where(t => dto.Tags.Contains(t.Id)).ToListAsync();
            if (tags.Count != dto.Tags.Count) throw new Exception("Some tags were not found.");
            
            project.ProjectTechnologyTags = tags.Select(t => new server.Entities.ProjectTechnologyTag 
            { 
                ProjectId = project.Id, 
                TagId = t.Id, 
                Tag = t 
            }).ToList();
        }

        await db.Projects.AddAsync(project);
        await db.SaveChangesAsync();
        await transaction.CommitAsync();

        return new ProjectDto
        {
            Id = project.Id,
            Name = project.Name,
            StartDate = project.StartDate,
            EndDate = project.EndDate,
            Description = project.Description,
            Tags = project.ProjectTechnologyTags?.Select(pt => new TagDto(pt.TagId, pt.Tag.Name)).ToList() ?? new List<TagDto>(),
            Version = EF.Property<uint>(project, "Version")
        };
    }

    public async Task<ProjectDto> UpdateProjectAsync(string userId, Guid projectId, UpdateProjectDto dto)
    {
        var project = await db.Projects
            .Include(p => p.ProjectTechnologyTags)
                .ThenInclude(pt => pt.Tag)
            .FirstOrDefaultAsync(p => p.Id == projectId && p.UserId == userId);
            
        using var transaction = await db.Database.BeginTransactionAsync();
        if (project == null) throw new Exception("Project not found.");

        if (!string.IsNullOrWhiteSpace(dto.Name)) project.Name = dto.Name;
        if (dto.StartDate.HasValue) project.StartDate = dto.StartDate;
        if (dto.EndDate.HasValue) project.EndDate = dto.EndDate;
        if (dto.Description != null) project.Description = dto.Description;
        project.UpdatedAt = DateTime.UtcNow;

        if (dto.Tags != null)
        {
            db.ProjectTechnologyTags.RemoveRange(project.ProjectTechnologyTags);
            if (dto.Tags.Count > 0)
            {
                var tags = await db.TechnologyTags.Where(t => dto.Tags.Contains(t.Id)).ToListAsync();
                if (tags.Count != dto.Tags.Count) throw new Exception("Some tags were not found.");
                
                project.ProjectTechnologyTags = tags.Select(t => new server.Entities.ProjectTechnologyTag 
                { 
                    ProjectId = project.Id, 
                    TagId = t.Id, 
                    Tag = t 
                }).ToList();
            }
        }

        db.Entry(project).Property<uint>("Version").OriginalValue = dto.Version;

        try
        {
            await db.SaveChangesAsync();
            await transaction.CommitAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            throw new Exception("Conflict: Refresh and Try Again");
        }

        return new ProjectDto
        {
            Id = project.Id,
            Name = project.Name,
            StartDate = project.StartDate,
            EndDate = project.EndDate,
            Description = project.Description,
            Tags = project.ProjectTechnologyTags.Select(pt => new TagDto(pt.TagId, pt.Tag.Name)).ToList(),
            Version = EF.Property<uint>(project, "Version")
        };
    }

    public async Task DeleteProjectAsync(string userId, Guid projectId)
    {
        var project = await db.Projects
            .FirstOrDefaultAsync(p => p.Id == projectId && p.UserId == userId);
            
        if (project == null) throw new Exception("Project not found.");

        db.Projects.Remove(project);
        await db.SaveChangesAsync();
    }

    public Task<server.Utils.PagedResponse<ProjectDto>> SearchProjectsAsync(string userId, ProjectSearchQueryDto dto)
    {
        throw new NotImplementedException();
    }
}
