using System.ComponentModel.DataAnnotations;

namespace Editor.Client.Models
{
    public class InputModel
    {
        [Required]
        public string Username { get; set; } = "";

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; } = "";
    }
}
