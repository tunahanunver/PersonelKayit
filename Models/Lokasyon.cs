using System.ComponentModel.DataAnnotations;

namespace PersonelKayit.Models
{
    /// <summary>
    /// Lokasyon bilgilerini tutan model sınıfı
    /// Ülke, şehir ve ilçe hiyerarşisini yönetir
    /// </summary>
    public class Lokasyon
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Lokasyon adı zorunludur.")]
        [Display(Name = "Lokasyon Adı")]
        public string Name { get; set; }

        public int ParentId { get; set; }

        // Lokasyon hakkında ek bilgiler
        [Display(Name = "Açıklama")]
        public string? Description { get; set; }

        // Bu lokasyona bağlı personeller (Navigation Property)
        public IEnumerable<Personel>? Personels { get; set; }
    }
}
