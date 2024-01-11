using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Monitor.Data.Config;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddDataDbContext(this IServiceCollection s, IConfiguration c)
    {
        s.Configure<EncryptionOptions>(options =>
        {
            c.GetSection("DatabaseEncryption").Bind(options);
        });
        
        s.AddDbContext<ApplicationDbContext>(o =>
        {
            var connectionString = c.GetConnectionString("Default");
            if (string.IsNullOrWhiteSpace(connectionString))
                o.UseNpgsql();
            else
                o.UseNpgsql(connectionString,
                    o => o.EnableRetryOnFailure());
        });
        
        return s;
    }
    
}