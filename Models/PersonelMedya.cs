using System.ComponentModel.DataAnnotations;

namespace PersonelKayit.Models
{
    public class PersonelMedya
    {
        [Key]
        [Display(Name = "Medya ID")]
        public int Id { get; set; }

        [Required(ErrorMessage = "Medya adı zorunludur.")]
        [Display(Name = "Medya İsmi")]
        public string? Name { get; set; }

        [Display(Name = "Personel ID")]
        public int? PersonelId { get; set; }
        public Personel? Personel { get; set; }
        public IEnumerable<Personel>? Personels { get; set; }
    }
}
