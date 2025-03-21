using System.ComponentModel.DataAnnotations;

namespace PersonelKayit.Models
{
    public class MedyaKutuphanesi
    {
        [Key]
        [Display(Name = "ID")]
        public int Id { get; set; }

        [Required(ErrorMessage = "Medya adı zorunludur")]
        [Display(Name = "Medya Adı")]
        public string? MedyaAdi { get; set; }

        [Required(ErrorMessage = "Medya URL zorunludur")]
        [Display(Name = "Medya URL")]
        public string? MedyaURL { get; set; }

        public ICollection<PersonelMedya>? PersonelMedyalar { get; set; }
    }
}
