using System.ComponentModel.DataAnnotations;

namespace PersonelKayit.Models
{
    public class Lokasyon
    {
        [Display(Name = "Lokasyon No")]
        public int Id { get; set; }

        [Display(Name = "Lokasyon Adı")]
        [Required(ErrorMessage = "Lokasyon adı zorunludur.")]
        public string Name { get; set; }

        [Display(Name = "Açıklama")]
        public string? Description { get; set; }

        [Display(Name = "Üst Lokasyon")]
        public int ParentId { get; set; }

        public IEnumerable<Personel>? Personels { get; set; }
    }
}
