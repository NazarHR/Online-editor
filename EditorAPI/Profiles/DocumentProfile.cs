using AutoMapper;
using EditorAPI.Data.Entities;
using EditorAPI.Models;

namespace EditorAPI.Profiles
{
    public class DocumentProfile:Profile
    {
        public DocumentProfile() {
            CreateMap<Document, DocumentDto>();
            CreateMap<DocumentDto, Document>();
        }
    }
}
