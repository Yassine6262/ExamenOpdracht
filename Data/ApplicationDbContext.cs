using MedewerkersBeheerApp.Models;
using Microsoft.EntityFrameworkCore;

namespace MedewerkersBeheerApp.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        { }

        public DbSet<Medewerker> Medewerkers { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Medewerker>().ToTable("Medewerkers");
        }
    }
}
