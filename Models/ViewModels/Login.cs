using System.ComponentModel.DataAnnotations;

namespace PersonelKayit.Models.ViewModels
{
    public class Login
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Kullanıcı Adı zorunludur")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Şifre zorunludur")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        public bool RememberMe { get; set; }
    }
}
