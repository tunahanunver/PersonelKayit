using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace PersonelKayit.Models
{
    public class PersonelDbContext : IdentityDbContext<ApplicationUser>
    {
        public DbSet<Personel> Personeller { get; set; }
        public DbSet<Lokasyon> Lokasyonlar { get; set; }
        public DbSet<PersonelMedya> PersonelMedyalari { get; set; }
        public DbSet<MedyaKutuphanesi> MedyaKutuphaneleri { get; set; }
        public DbSet<PersonelEgitim> PersonelEgitimleri { get; set; }

        public PersonelDbContext(DbContextOptions<PersonelDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Lokasyon>().Property(b => b.Name).IsRequired().HasColumnType("varchar(50)").HasMaxLength(50);
            modelBuilder.Entity<Personel>().HasOne(p => p.Lokasyon).WithMany(p => p.Personels).HasForeignKey(p => p.LokasyonId);
            
            // PersonelMedya - Personel ilişkisi
            modelBuilder.Entity<PersonelMedya>()
                .HasOne(pm => pm.Personel)
                .WithMany()
                .HasForeignKey(pm => pm.PersonelId);

            // PersonelMedya - MedyaKutuphanesi ilişkisi
            modelBuilder.Entity<PersonelMedya>()
                .HasOne(pm => pm.MedyaKutuphanesi)
                .WithMany(mk => mk.PersonelMedyalar)
                .HasForeignKey(pm => pm.MedyaId);

            // PersonelEgitim - Personel ilişkisi
            modelBuilder.Entity<PersonelEgitim>()
                .HasOne(pe => pe.Personel)
                .WithMany(pe => pe.Egitimler)
                .HasForeignKey(pe => pe.PersonelId)
                .OnDelete(DeleteBehavior.Cascade);

            base.OnModelCreating(modelBuilder);
        }
    }
}
