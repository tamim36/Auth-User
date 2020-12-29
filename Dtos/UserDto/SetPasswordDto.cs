using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Dtos.UserDto
{
    public class SetPasswordDto
    {
        [Required]
        [MinLength(6)]
        public string Password { get; set; }
        [Required]
        [Compare(nameof(Password), ErrorMessage = "Wrong Password")]
        public string ConfirmPassword { get; set; }
    }
}
