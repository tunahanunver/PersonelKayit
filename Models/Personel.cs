using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PersonelKayit.Models
{
    /// <summary>
    /// Personel bilgilerini tutan model sınıfı
    /// </summary>
    public class Personel
    {
        // Personel ID'si (Primary Key)
        [Key]
        public int Id { get; set; }

        // Personel adı
        [Required(ErrorMessage = "Ad alanı zorunludur")]
        [StringLength(50)]
        [Display(Name = "Ad")]
        public string? Ad { get; set; }

        // Personel soyadı
        [Required(ErrorMessage = "Soyad alanı zorunludur")]
        [StringLength(50)]
        [Display(Name = "Soyad")]
        public string? Soyad { get; set; }

        // Personel doğum tarihi
        [Required(ErrorMessage = "Doğum tarihi zorunludur")]
        [DataType(DataType.Date)]
        [Display(Name = "Doğum Tarihi")]
        public DateTime DogumTarihi { get; set; }

        // Personel cinsiyeti
        [Required(ErrorMessage = "Cinsiyet seçimi zorunludur")]
        [Display(Name = "Cinsiyet")]
        public string? Cinsiyet { get; set; }

        // Personelin bulunduğu ülke (veritabanında saklanmaz)
        [NotMapped]
        [Display(Name = "Ülke")]
        public string? Ulke { get; private set; }

        // Personelin bulunduğu şehir (veritabanında saklanmaz)
        [NotMapped]
        [Display(Name = "Şehir")]
        public string? Sehir { get; private set; }

        // Personelin bulunduğu ilçe (Lokasyon nesnesinden alınır)
        [NotMapped]
        [Display(Name = "İlçe")]
        public string? Ilce => Lokasyon?.Name;

        //[Required(ErrorMessage = "Resim alanı zorunludur")]
        [Display(Name = "Resim")]
        public string? Image {  get; set; }

        [NotMapped]
        public List<PersonelMedya> Medyalar { get; set; } = new List<PersonelMedya>();

        // Personel hakkında ek bilgiler
        [Display(Name = "Açıklama")]
        [DataType(DataType.MultilineText)]
        [StringLength(500, ErrorMessage = "Açıklama en fazla 500 karakter olabilir")]
        public string? Aciklama { get; set; }

        // Personelin bağlı olduğu lokasyon ID'si (Foreign Key)
        public int? LokasyonId { get; set; }

        // Personelin bağlı olduğu lokasyon nesnesi (Navigation Property)
        public Lokasyon? Lokasyon { get; set; }

        /// <summary>
        /// Personelin ülke ve şehir bilgilerini set eder
        /// </summary>
        /// <param name="ulke">Ülke adı</param>
        /// <param name="sehir">Şehir adı</param>
        public void SetLocationInfo(string ulke, string sehir)
        {
            Ulke = ulke;
            Sehir = sehir;
        }
    }
}
