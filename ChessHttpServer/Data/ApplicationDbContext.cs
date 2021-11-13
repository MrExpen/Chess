using Microsoft.EntityFrameworkCore;
using System;

namespace ChessHttpServer.Data
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<ChessMatch> ChessMatchs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ChessMatch>()
                .HasMany(m => m.Fens)
                .WithOne()
                .OnDelete(DeleteBehavior.Cascade);
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder
                .UseMySql(
                Environment.GetEnvironmentVariable("CONNECTION_STRING"),
                new MySqlServerVersion(new Version(8, 0, 27)))
                .UseLazyLoadingProxies();
        }
        public ApplicationDbContext()
        {
            Database.EnsureCreated();
        }
    }
}
