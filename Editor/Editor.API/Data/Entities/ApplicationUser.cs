using Microsoft.AspNetCore.Identity;

namespace EditorAPI.Data.Entities
{
    public class ApplicationUser:IdentityUser
    {
        public int DocumentsLimit { get; set; } = 3;
    }
}
