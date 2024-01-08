using System.Text;
using Monitor.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.DataEncryption;
using Microsoft.Extensions.Configuration;
using Monitor.Data.Crypto;

namespace Monitor.Data;

public class ApplicationDbContext : DbContext

{
    private readonly IEncryptionProvider _provider;
    
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, IConfiguration config)
        : base(options)
    {
        var encryptionKey = Encoding.UTF8.GetBytes(config["EncryptionKey"] ?? string.Empty);

        _provider = new DynamicIvAesProvider(encryptionKey);
    }
    
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
        builder.UseEncryption(_provider);
        
        builder.Entity<StudyUser>()
            .HasKey(su => new { su.StudyId, su.UserId });
    }

}