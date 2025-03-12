using System.ComponentModel.DataAnnotations;

namespace PersonelKayit.Models.ViewModels
{
    public class Register
    {
        [Required(ErrorMessage = "Kullanıcı Adı zorunludur")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Ad alanı zorunludur")]
        public string Ad { get; set; }

        [Required(ErrorMessage = "Soyad alanı zorunludur")]
        public string Soyad { get; set; }

        [Required(ErrorMessage = "Email Adresi zorunludur")]
        [EmailAddress(ErrorMessage = "Geçerli bir Email Adresi giriniz")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Şifre zorunludur")]
        [DataType(DataType.Password)]
        [MinLength(6, ErrorMessage = "Şifre en az 6 karakter olmalıdır")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Şifre tekrarı zorunludur")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Şifreler eşleşmiyor")]
        public string ConfirmPassword { get; set; }
    }
}
