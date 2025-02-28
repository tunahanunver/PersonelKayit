using Microsoft.EntityFrameworkCore;

namespace PersonelKayit.Models
{
    public class PersonelDbContext : DbContext
    {
        public DbSet<Personel> Personeller { get; set; }
        public DbSet<Lokasyon> Lokasyonlar { get; set; }

        public PersonelDbContext(DbContextOptions<PersonelDbContext> options)
            : base(options)
        {

        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Lokasyon>().Property(b => b.Name).IsRequired().HasColumnType("varchar(50)").HasMaxLength(50);
            modelBuilder.Entity<Personel>().HasOne(p=>p.Lokasyon).WithMany(p=>p.Personels).HasForeignKey(p=>p.LokasyonId);
            base.OnModelCreating(modelBuilder);
        }
    }
}
