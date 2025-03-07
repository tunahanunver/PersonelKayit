using System.ComponentModel.DataAnnotations;

namespace PersonelKayit.Models
{
    public class PersonelMedya
    {
        [Key]
        [Display(Name = "Media ID")]
        public int Id { get; set; }


        [Required(ErrorMessage = "Media adı zorunludur.")]
        [Display(Name = "Media İsmi")]
        public string? Name { get; set; }

        [Display(Name = "Personel ID")]
        public int? PersonelId { get; set; }
        public Personel? Personel { get; set; }
        public MedyaKutuphanesi? MedyaKutuphanesi { get; set; }
    }
}
