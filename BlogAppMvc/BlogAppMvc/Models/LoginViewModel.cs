using System.ComponentModel.DataAnnotations;

namespace BlogAppMvc.Models
{
    public class LoginViewModel
    {
        [Required(ErrorMessage= "The Email field is required")]
        [EmailAddress(ErrorMessage ="Invalid email address.")]
        [Display(Name = "Email")]
        public string Email { get; set; }
        [Required(ErrorMessage = "The password field is required.")]
        [MinLength(6, ErrorMessage = "Password must be at least 6 characters.")]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }
    }
}
