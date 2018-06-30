using Microsoft.EntityFrameworkCore;
using PushServer.PushConfiguration.EntityFramework.Entities;

namespace PushServer.PushConfiguration.EntityFramework
{
    public class ConfigurationDbContext : DbContext
    {
        public ConfigurationDbContext(DbContextOptions<ConfigurationDbContext> options)
            : base(options)
        {
        }

        public DbSet<PushChannelConfiguration> PushChannelConfigurations { get; set; }

        public DbSet<PushChannelOption> PushChannelOptions { get; set; }
    }
}
