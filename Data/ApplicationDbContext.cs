using Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options) { }
    
    public DbSet<Report> Reports => Set<Report>();
    public DbSet<ReportType> ReportTypes => Set<ReportType>();

}