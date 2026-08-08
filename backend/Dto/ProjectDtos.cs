using System;
using System.Collections.Generic;

namespace server.Dto;

public record TagDto(Guid Id, string Name);

public record ProjectDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public DateOnly? StartDate { get; set; }
    public DateOnly? EndDate { get; set; }
    public string? Description { get; set; }
    public List<TagDto> Tags { get; set; } = [];
    public uint Version { get; set; }
}
