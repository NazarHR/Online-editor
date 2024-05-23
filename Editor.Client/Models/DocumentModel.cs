using System.ComponentModel.DataAnnotations;

namespace Editor.Client.Models
{
    public class DocumentModel
    {
        public int? Id { get; set; }
        [Required]
        public string? Title { get; set; }
        public string? Content { get; set; }
    }
}
