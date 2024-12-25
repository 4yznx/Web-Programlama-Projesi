using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using BarberShop.Models;

public class BarberDbContext : IdentityDbContext<Kullanici>
{
	public BarberDbContext(DbContextOptions<BarberDbContext> options) : base(options) { }

	public DbSet<Calisan> Calisanlar { get; set; }
	public DbSet<Islem> Islemler { get; set; }
	public DbSet<CalisanIslem> CalisanIslemler { get; set; }
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
               .HasOne(r => r.kullanici) // Navigation property
               .WithMany() // If Kullanici can have multiple Randevular, use .WithMany(r => r.Randevular)
               .HasForeignKey(r => r.UserID) // Foreign key
               .OnDelete(DeleteBehavior.Cascade); // Optional: Set delete behavior


        modelBuilder.Entity<Calisan>()
      .HasMany(c => c.Randevular)
      .WithOne(r => r.Calisan)
      .HasForeignKey(r => r.CalisanID)
      .OnDelete(DeleteBehavior.Cascade);

        // Default values for working hours
        modelBuilder.Entity<Calisan>()
            .Property(c => c.CalismaBaslangici)
            .HasDefaultValue(new TimeSpan(9, 0, 0)); // 9:00 AM

        modelBuilder.Entity<Calisan>()
            .Property(c => c.CalismaBitisi)
            .HasDefaultValue(new TimeSpan(17, 0, 0)); // 5:00 PM
    }
}
