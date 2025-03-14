using System.ComponentModel.DataAnnotations;

namespace PersonelKayit.Models.ViewModels
{
    public class Profile
    {
        [Required(ErrorMessage = "Email alanı zorunludur")]
        [EmailAddress(ErrorMessage = "Geçerli bir email adresi giriniz")]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Kullanıcı adı zorunludur")]
        [Display(Name = "Kullanıcı Adı")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Telefon numarası zorunludur")]
        [Phone(ErrorMessage = "Geçerli bir telefon numarası giriniz")]
        [Display(Name = "Telefon Numarası")]
        [RegularExpression(@"^[0-9]{10}$", ErrorMessage = "Telefon numarası 10 haneli olmalıdır")]
        public string PhoneNumber { get; set; }
    }
}
