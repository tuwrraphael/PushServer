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

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<PushChannelConfiguration>()
                .HasKey(v => v.Id);
            modelBuilder.Entity<PushChannelConfiguration>()
                .HasMany(v => v.Options)
                .WithOne(v => v.PushChannelConfiguration)
                .HasForeignKey(v => v.PushChannelConfigurationID)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
