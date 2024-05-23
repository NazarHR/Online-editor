using System.ComponentModel.DataAnnotations;

namespace Editor.Client.Models
{
    public class RegisterModel
    {
        [Required]
        public string Username { get; set; } = "";
        [Required]
        public string Email { get; set; } = "";
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; } = "";
    }
}
