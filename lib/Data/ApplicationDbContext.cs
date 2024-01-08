using System.Text;
using Monitor.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.DataEncryption;
using Microsoft.EntityFrameworkCore.DataEncryption.Providers;
using Microsoft.Extensions.Configuration;

namespace Monitor.Data;

public class ApplicationDbContext : DbContext

{
    private readonly IEncryptionProvider _provider;
    
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, IConfiguration config)
        : base(options)
    {
        // Ideally the IV would be different for every field, and added to the stored encrypted text.
        // But using this library's AesProvider means that the IV cannot be dynamic.
        // As it's fine for the IV to be public anyway, it is still secure to be a config value.
        // Even if we're not meeting best practice: 
        var encryptionKey = Encoding.UTF8.GetBytes(config["EncryptionKey"] ?? string.Empty);
        // var encryptionIv = Encoding.UTF8.GetBytes(config["EncryptionIV"] ?? string.Empty);

        _provider = new DynamicIvAesProvider(encryptionKey);

        // _provider = new AesProvider(encryptionKey, encryptionIv);
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