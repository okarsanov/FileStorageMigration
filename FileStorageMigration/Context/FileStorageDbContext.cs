using FileStorageMigration.Entities.FileStorage;
using Microsoft.EntityFrameworkCore;

namespace FileStorageMigration.Context
{
    public class FileStorageDbContext : DbContext
    {
        public DbSet<DriveItemEntity> DriveItemEntities { get; set; }
        public DbSet<FileInfoEntity> FileInfoEntities { get; set; }


        private string _connectionString;
        public FileStorageDbContext(string connectionString)
        {
            _connectionString = connectionString;
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            => optionsBuilder.UseNpgsql(_connectionString);
    }
}
