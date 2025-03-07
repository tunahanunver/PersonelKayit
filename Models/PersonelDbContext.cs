using Microsoft.EntityFrameworkCore;

namespace PersonelKayit.Models
{
    public class PersonelDbContext : DbContext
    {
        public DbSet<Personel> Personeller { get; set; }
        public DbSet<Lokasyon> Lokasyonlar { get; set; }
        public DbSet<PersonelMedya> PersonelMedyalari { get; set; }
        public DbSet<MedyaKutuphanesi> MedyaKutuphaneleri { get; set; }


        public PersonelDbContext(DbContextOptions<PersonelDbContext> options)
            : base(options)
        {

        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Lokasyon>().Property(b => b.Name).IsRequired().HasColumnType("varchar(50)").HasMaxLength(50);
            modelBuilder.Entity<Personel>().HasOne(p=>p.Lokasyon).WithMany(p=>p.Personels).HasForeignKey(p=>p.LokasyonId);
            modelBuilder.Entity<PersonelMedya>().HasOne(pm => pm.Personel).WithMany().HasForeignKey(pm => pm.PersonelId);
            modelBuilder.Entity<MedyaKutuphanesi>().HasOne(mk => mk.PersonelMedya).WithOne(mk => mk.MedyaKutuphanesi).HasForeignKey<MedyaKutuphanesi>(mk => mk.PersonelMedyaId);

           
            base.OnModelCreating(modelBuilder);
        }
    }
}
