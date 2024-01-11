using System.Text;
using Monitor.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.DataEncryption;
using Microsoft.Extensions.Options;
using Monitor.Data.Config;
using Monitor.Data.Crypto;

namespace Monitor.Data;

public class ApplicationDbContext : DbContext

{
    private readonly IEncryptionProvider _provider;
    private readonly EncryptionOptions _encryptionOptions;
    
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, IOptions<EncryptionOptions> encryptionOptions)
        : base(options)
    {
        _encryptionOptions = encryptionOptions.Value;
        var encryptionKey =
            Encoding.UTF8.GetBytes(_encryptionOptions.EncryptionKey ?? throw new ArgumentException("EncryptionKey is missing."));

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
    public DbSet<Entities.Config> Config => Set<Entities.Config>();
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.UseEncryption(_provider);
        
        builder.Entity<StudyUser>()
            .HasKey(su => new { su.StudyId, su.UserId });
    }

}