using System;
using Microsoft.AspNetCore.Identity;
using server.Data;

namespace server.Entities;

public class ApplicationUser : IdentityUser
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public DateTime? JoinedAt { get; set; }
    public DateTime? LastLoginAt { get; set; }
    public string Status { get; set; } = server.Data.UserStatus.Active;
}
