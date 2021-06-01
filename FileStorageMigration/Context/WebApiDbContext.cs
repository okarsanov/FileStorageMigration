using FileStorageMigration.Entities.WebApi;
using Microsoft.EntityFrameworkCore;

namespace FileStorageMigration.Context
{
    public class WebApiDbContext : DbContext
    {
        public DbSet<FileExt> FilesExt { get; set; }

        private string _connectionString;
        public WebApiDbContext(string connectionString)
        {
            _connectionString = connectionString;
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.HasDefaultSchema("DataMigration");
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql(_connectionString);
        }
    }
}
