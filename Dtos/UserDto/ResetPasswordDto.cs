using System.ComponentModel.DataAnnotations;

namespace Dtos.UserDto
{
    public class ResetPasswordDto
    {
        [Required]
        public string Token { get; set; }
        [Required]
        [MinLength(6)]
        public string Password { get; set; }
        [Required]
        [Compare(nameof(Password), ErrorMessage ="Wrong Password")]
        public string ConfirmPassword { get; set; }
    }
}
