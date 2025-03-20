using System.ComponentModel.DataAnnotations;

namespace PersonelKayit.Models
{
    public class PersonelEgitim
    {
        [Key]
        [Display(Name ="ID")]
        public int Id { get; set; }

        [Required(ErrorMessage ="Okul Adı Zorunludur!")]
        [Display(Name ="Okul Adı")]
        [StringLength(100)]
        public string OkulAdi { get; set; }

        [Required(ErrorMessage = "Bölüm Adı Zorunludur!")]
        [Display(Name = "Bölüm")]
        [StringLength(100)]
        public string Bolum { get; set; }

        [Display(Name ="Mezuniyet Tarihi")]
        [DataType(DataType.Date)]
        public DateTime? MezuniyetTarihi { get; set; }

        [Display(Name ="Devam Ediyor")]
        public bool DevamEdiyor { get; set; }

        [Display(Name = "Personel ID")]
        public int PersonelId { get; set; }

        public Personel Personel { get; set; }
    }
}
