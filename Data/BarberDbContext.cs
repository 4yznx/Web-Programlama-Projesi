using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using BarberShop.Models;

public class BarberDbContext : IdentityDbContext<Kullanici>
{
    public BarberDbContext(DbContextOptions<BarberDbContext> options) : base(options) { }

    public DbSet<Calisan> Calisanlar { get; set; }
    public DbSet<Islem> Islemler { get; set; }
    public DbSet<CalisanIslem> CalisanIslemler { get; set; }
    public DbSet<CalisanKazanclari> CalisanKazanclari { get; set; }
    public DbSet<Randevu> Randevular { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<CalisanIslem>()
            .HasKey(ci => new { ci.CalisanID, ci.IslemID });

        modelBuilder.Entity<CalisanIslem>()
            .HasOne(ci => ci.Calisan)
            .WithMany(c => c.CalisanIslemler)
            .HasForeignKey(ci => ci.CalisanID);

        modelBuilder.Entity<CalisanIslem>()
            .HasOne(ci => ci.Islem)
            .WithMany(i => i.CalisanIslemler)
            .HasForeignKey(ci => ci.IslemID);

        modelBuilder.Entity<Randevu>()
            .HasOne(r => r.Kullanici)
            .WithMany()
            .HasForeignKey(r => r.UserID);
    }
}