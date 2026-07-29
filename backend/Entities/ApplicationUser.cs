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
    public UserStatus Status { get; set; }
}
