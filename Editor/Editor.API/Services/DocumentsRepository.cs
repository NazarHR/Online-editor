using EditorAPI.Data;
using EditorAPI.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace EditorAPI.Services
{
    public class DocumentsRepository:IDocumentsRepository
    {
        private readonly EditorDbContext _context;

        public DocumentsRepository(EditorDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<IEnumerable<Document>> GetUsersDocuments(string userId)
        {
            return await _context.Documents.Where(d => d.UserId == userId).ToListAsync();
        }

        public async Task AddDocument(Document document)
        {
            _context.Add(document);
            await _context.SaveChangesAsync();
        }

        public async Task<Document?> FindAsync(int? id)
        {
            return await _context.Documents.FindAsync(id);
        }

        public async Task UpdateDocumentAsync(Document document)
        {
            _context.Update(document);
            await _context.SaveChangesAsync();
        }
        public bool DocumentExists(int id)
        {
            if (id == null) return false;
            return _context.Documents.Any(e => e.Id == id);
        }

        public async Task DeleteDocumentAsync(Document? document)
        {
            if (document != null)
            {
                _context.Documents.Remove(document);
            }
            await _context.SaveChangesAsync();
        }

        public async Task<int> CountUsersDocuments(string userId)
        {
            return _context.Documents.Where(d => d.UserId == userId).Count();
        }
    }
}