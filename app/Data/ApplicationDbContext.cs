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
    public DbSet<Study> Studies => Set<Study>();
    public DbSet<StudyGroup> StudyGroups => Set<StudyGroup>();
    public DbSet<StudyUser> StudyUsers => Set<StudyUser>();
    public DbSet<Config> Config => Set<Config>();
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        
        builder.Entity<StudyUser>()
            .HasKey(su => new { su.StudyId, su.UserId });
    }

}