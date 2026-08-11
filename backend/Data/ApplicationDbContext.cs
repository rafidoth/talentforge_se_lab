using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using server.Entities;

namespace server.Data;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    public DbSet<UserPreference> UserPreferences { get; set; }
    public DbSet<AppAttribute> Attributes { get; set; }
    public DbSet<ProfileAttribute> ProfileAttributes { get; set; }
    public DbSet<AttributeCategory> AttributeCategories { get; set; }
    public DbSet<AttributeType> AttributeTypes { get; set; }
    public DbSet<AttributeDropdownOption> AttributeDropdownOptions { get; set; }
    public DbSet<Project> Projects { get; set; }
    public DbSet<TechnologyTag> TechnologyTags { get; set; }
    public DbSet<ProjectTechnologyTag> ProjectTechnologyTags { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
    }
}
