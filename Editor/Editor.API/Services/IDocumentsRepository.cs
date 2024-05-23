using EditorAPI.Data.Entities;

namespace EditorAPI.Services
{
    public interface IDocumentsRepository
    {
        Task<IEnumerable<Document>> GetUsersDocuments(string userId);
        Task AddDocument(Document document);
        Task<Document?> FindAsync(int? id);
        Task UpdateDocumentAsync(Document document);
        bool DocumentExists(int id);
        Task DeleteDocumentAsync(Document? document);
        Task<int> CountUsersDocuments(string userId);
    }
}