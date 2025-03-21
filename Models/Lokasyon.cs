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
        [Display(Name = "ID")]
        public int Id { get; set; }

        [Required(ErrorMessage = "Lokasyon adı zorunludur.")]
        [Display(Name = "Lokasyon Adı")]
        public string Name { get; set; }

        [Display(Name = "Üst ID")]
        public int ParentId { get; set; }

        // Lokasyon hakkında ek bilgiler
        [Display(Name = "Açıklama")]
        public string? Description { get; set; }

        // Bu lokasyona bağlı personeller (Navigation Property)
        public IEnumerable<Personel>? Personels { get; set; }
    }
}
