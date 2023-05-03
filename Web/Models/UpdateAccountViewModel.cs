using System.ComponentModel.DataAnnotations;

namespace Web.Models
{
    public class UpdateAccountViewModel
    {
        public string? NameSurname { get; set; }
        public string Email { get; set; } = null!;
        public string CurrentPassword { get; set; } = null!;

        [Required(ErrorMessage = "Şifre alanı boş geçilemez!")]
        public string Password { get; set; } = null!;

        [Compare("Password", ErrorMessage = "Şifreler eşleşmiyor!")]
        [Required(ErrorMessage = "Şifre tekrar alanı boş geçilemez!")]
        public string ConfirmPassword { get; set; } = null!;
    }
}
