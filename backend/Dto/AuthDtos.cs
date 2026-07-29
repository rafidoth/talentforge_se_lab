using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using server.Data;

namespace server.Dto;

public record RegisterDto(
    [Required] [EmailAddress] string Email,
    [Required] string Password,
    [Required] JsonElement FirstName,
    [Required] JsonElement LastName,
    [Required] JsonElement Location
);

public record RegisterResponse(bool Success, string UserId, string Role = Roles.Candidate);
