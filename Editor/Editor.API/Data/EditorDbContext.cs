using EditorAPI.Data.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace EditorAPI.Data
{
    public class EditorDbContext: IdentityDbContext<ApplicationUser>
    {
        public EditorDbContext() { }
        public EditorDbContext(DbContextOptions<EditorDbContext> options) : base(options) { }

        public virtual DbSet<Document> Documents { get; set; }
    }
}
