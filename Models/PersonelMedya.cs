using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PersonelKayit.Models
{
    public class PersonelMedya
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Personel ID zorunludur")]
        [Display(Name = "Personel ID")]
        public int PersonelId { get; set; }

        [Required(ErrorMessage = "Medya ID zorunludur")]
        [Display(Name = "Medya ID")]
        public int MedyaId { get; set; }

        [ForeignKey("PersonelId")]
        public Personel? Personel { get; set; }

        [ForeignKey("MedyaId")]
        public MedyaKutuphanesi? MedyaKutuphanesi { get; set; }
    }
}
