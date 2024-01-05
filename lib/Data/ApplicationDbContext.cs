using System.Security.Cryptography;
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
        // TODO: We should be generating a different IV everytime we encrypt.
        // Ideally the provider would be adding this to the encrypted text
        // But I don't think it is, so we probably need to use a pair for now.
        // Or write our own wrapper.
        var aes = Aes.Create();
        aes.KeySize = 256;
        aes.BlockSize = 128;
        aes.GenerateIV();
        
        var encryptionKey = Encoding.UTF8.GetBytes(config["EncryptionKey"] ?? string.Empty);
        var encryptionIv = Encoding.UTF8.GetBytes(config["EncryptionIV"] ?? string.Empty);

        _provider = new AesProvider(encryptionKey, encryptionIv);
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