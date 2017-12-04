using System.ComponentModel.DataAnnotations;

namespace Iris.Api
{
    public class LoginViewModel
    {
        [Required]
        public string Username { get; set; }

        [Required]
        public string Password { get; set; }
    }
}