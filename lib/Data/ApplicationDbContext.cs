using System.Security.Cryptography;
using System.Text;
using Monitor.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.DataEncryption;
using Microsoft.EntityFrameworkCore.DataEncryption.Providers;

namespace Monitor.Data;

public class ApplicationDbContext : DbContext

{
    private readonly byte[] _encryptionKey = Encoding.Unicode.GetBytes("Fk0bI2pC"); // TODO: Read this from config
    private readonly IEncryptionProvider _provider;
    
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
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

        _provider = new AesProvider(Encoding.Unicode.GetBytes("Fk0bI2pCVdppFhEj"), Encoding.Unicode.GetBytes("Fk0bI2pC"));
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