using System.ComponentModel.DataAnnotations;

namespace EditorAPI.Models
{
    public class PaymentLinkDetails
    {
        [Required]
        public string ProductId { get; set; }
        public int Amount { get; set; } = 1;
        [Required]
        public string ReturnLink { get; set; }

    }
}
