using AutoMapper;
using EditorAPI.Data.Entities;
using EditorAPI.Models;
using EditorAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace EditorAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class DocumentsController : ControllerBase
    {
        private readonly IDocumentsRepository _documentsRepository;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IMapper _mapper;

        public DocumentsController(
            IDocumentsRepository documentsRepository, 
            IMapper mapper, 
            UserManager<ApplicationUser> userManager)
        {
            _documentsRepository = documentsRepository;
            _mapper = mapper;
            _userManager = userManager;
        }
        [HttpGet("{userId}")]
        public async Task<ActionResult<IEnumerable<DocumentDto>>> GetDocuments(string userId)
        {
            var documents = await _documentsRepository.GetUsersDocuments(userId);
            var documentsDto = documents.Select(d => _mapper.Map<Document, DocumentDto>(d));
            return Ok(documentsDto);
        }
        [HttpGet("{userId}/{documentId}", Name = "GetDocument")]
        public async Task<ActionResult<DocumentDto>> GetDocument(string userId, int documentId)
        {
            var document = await _documentsRepository.FindAsync(documentId);
            if (document == null)
            {
                return NotFound();
            }
            if (document.UserId != User.FindFirstValue(ClaimTypes.Sid))
            {
                return Forbid();
            }
            return Ok(_mapper.Map<Document,DocumentDto>(document));
        }
        [HttpPost]
        public async Task<IActionResult> CreateAsync(DocumentDto documentDto)
        {
           
            var user = await _userManager.FindByNameAsync(User.Identity!.Name!);
            var documentCoutn = await _documentsRepository.CountUsersDocuments(user!.Id);
           
            if(user.DocumentsLimit-documentCoutn < 1)
            {
                return StatusCode(402);
            }

            var document = _mapper.Map<DocumentDto, Document>(documentDto);
            document.User= user;
            document.UserId = user.Id;
            if (ModelState.IsValid)
            {
                await _documentsRepository.AddDocument(document);
                return CreatedAtRoute("GetDocument",
                    new
                    {
                        userId = document.UserId,
                        documentId = document.Id
                    },
                    null
                    );
            }
            return BadRequest();
        }
        [HttpPut]
        public async Task<IActionResult> Edit(DocumentDto documentDto)
        {
            if (documentDto == null)
            {
                return NotFound();
            }
            var documentToChange = await _documentsRepository.FindAsync(documentDto.Id);
            if(documentToChange == null)
            {
                return NotFound();
            }
            if (documentToChange.UserId != User.FindFirstValue(ClaimTypes.Sid))
            {
                return Forbid();
            }
            documentToChange.Title = documentDto.Title;
            documentToChange.Content = documentDto.Content;
            await _documentsRepository.UpdateDocumentAsync(documentToChange);
            return CreatedAtRoute("GetDocument",
                    new
                    {
                        userId = documentToChange.UserId,
                        documentId = documentToChange.Id
                    },
                    null);
        }
        [HttpDelete("{documentId}")]
        public async Task<IActionResult> DeleteDocument(int? documentId)
        {
            if(documentId == null)
                return NoContent();
            var documentToDelete  = await _documentsRepository.FindAsync(documentId);
            if (documentToDelete == null)
                return NotFound();
            await _documentsRepository.DeleteDocumentAsync(documentToDelete);
            return NoContent();
        }
    }

}
