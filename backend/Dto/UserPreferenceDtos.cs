using System.ComponentModel.DataAnnotations;

namespace server.Dto;

public record UserPreferenceDto
{
    public string Theme { get; init; } = "light";
    public string Language { get; init; } = "en";
}
