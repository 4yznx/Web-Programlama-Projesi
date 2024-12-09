using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using BarberShop.Models;

namespace BarberShop.Data
{
    public class BarberDbContext : IdentityDbContext<Kullanici>
    {
        public BarberDbContext(DbContextOptions<BarberDbContext> options) : base(options) { }

        public DbSet<Calisan> Calisanlar { get; set; }
        public DbSet<Islem> Islemler { get; set; }

        //public DbSet<Randevu> Randevular { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // many-to-many ilişkisi
            modelBuilder.Entity<Calisan>()
                .HasMany(c => c.Islemler)
                .WithMany(i => i.Calisanlar);
        }
    }
}
