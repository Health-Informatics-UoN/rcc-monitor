using Monitor.Data.Entities;
using Monitor.Data.Entities.Identity;

using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Monitor.Data;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options) { }
    
    public DbSet<RegistrationAllowlistEntry> RegistrationAllowlist => Set<RegistrationAllowlistEntry>();
    
    public DbSet<RegistrationRule> RegistrationRules => Set<RegistrationRule>();
    public DbSet<Team> Teams => Set<Team>();
    public DbSet<Report> Reports => Set<Report>();
    public DbSet<ReportType> ReportTypes => Set<ReportType>();
    public DbSet<Instance> Instances => Set<Instance>();
    public DbSet<ReportStatus> ReportStatus  => Set<ReportStatus>();
    
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
    }

}