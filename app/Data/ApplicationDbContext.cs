using Monitor.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace Monitor.Data;

public class ApplicationDbContext : DbContext

{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options) { }
    
    public DbSet<Report> Reports => Set<Report>();
    public DbSet<ReportType> ReportTypes => Set<ReportType>();
    public DbSet<Instance> Instances => Set<Instance>();
    public DbSet<ReportStatus> ReportStatus  => Set<ReportStatus>();
    public DbSet<Site> Sites  => Set<Site>();
    
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
    }

}