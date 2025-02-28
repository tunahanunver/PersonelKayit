using System;
using System.ComponentModel.DataAnnotations;

namespace PersonelKayit.Models
{
    public class Personel
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Ad alanı zorunludur")]
        [StringLength(50)]
        [Display(Name = "Ad")]
        public string? Ad { get; set; }

        [Required(ErrorMessage = "Soyad alanı zorunludur")]
        [StringLength(50)]
        [Display(Name = "Soyad")]
        public string? Soyad { get; set; }

        [Required(ErrorMessage = "Doğum tarihi zorunludur")]
        [DataType(DataType.Date)]
        [Display(Name = "Doğum Tarihi")]
        public DateTime DogumTarihi { get; set; }

        [Required(ErrorMessage = "Cinsiyet seçimi zorunludur")]
        [Display(Name = "Cinsiyet")]
        public string? Cinsiyet { get; set; }

        [Required(ErrorMessage = "Ülke seçimi zorunludur")]
        [Display(Name = "Ülke")]
        public string? Ulke { get; set; }

        [Required(ErrorMessage = "Şehir seçimi zorunludur")]
        [Display(Name = "Şehir")]
        public string? Sehir { get; set; }

        [Display(Name = "Açıklama")]
        [DataType(DataType.MultilineText)]
        [StringLength(500, ErrorMessage = "Açıklama en fazla 500 karakter olabilir")]
        public string? Aciklama { get; set; }

        public int LokasyonId { get; set; }
        public Lokasyon? Lokasyon { get; set; }
    }
}
