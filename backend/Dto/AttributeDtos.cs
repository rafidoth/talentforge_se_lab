using System;
using System.Collections.Generic;

namespace server.Dto;

public record DropdownOptionDto(Guid Id, string Label);

public record AttributeCategoryDto(int Id, string Name);

public record CreateAttributeDto(
    [System.ComponentModel.DataAnnotations.Required(ErrorMessage = "Attribute name is required.")]
    string Name,
    int TypeId,
    int CategoryId,
    string Description,
    List<string>? DropdownOptions = null
);

public record AttributeDto
{
    public Guid Id { get; init; } = Guid.Empty;
    public string Name { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public int TypeId { get; init; }
    public string TypeName { get; init; } = string.Empty;
    public int CategoryId { get; init; }
    public string CategoryName { get; init; } = string.Empty;
    public bool IsBuiltin { get; init; }
    public List<DropdownOptionDto>? DropdownOptions { get; init; }
    public uint Version { get; init; }
}

public record UpdateAttributeDto(
    string? Name,
    string? Description,
    List<string>? DropdownOptions,
    uint Version
);

public record AttributeSearchQueryDto(
    string? Prefix = null,
    int? CategoryId = null,
    bool Recent = false,
    int Page = 1,
    int PageSize = 10
);
