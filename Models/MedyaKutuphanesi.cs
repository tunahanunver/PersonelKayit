using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace PersonelKayit.Models
{
    public class MedyaKutuphanesi
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Media adı zorunludur")]
        [Display(Name = "Media Adı")]
        public string? MedyaAdi { get; set; }

        [Required(ErrorMessage = "Media GUID zorunludur")]
        [Display(Name = "Media GUID")]
        public string? MedyaGuid { get; set; }

        [Display(Name = "Personel Media ID")]
        public int? PersonelMedyaId { get; set; }

        [ForeignKey("PersonelMedyaId")]
        public PersonelMedya? PersonelMedya { get; set; }
    }
}
