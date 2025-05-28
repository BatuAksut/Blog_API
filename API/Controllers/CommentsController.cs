using API.CustomActionFilters;
using AutoMapper;
using DataAccess.Abstract;
using Entities;
using Entities.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using API.Extensions;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CommentsController : ControllerBase
    {
        private readonly ICommentRepository repository;
        private readonly IMapper mapper;
        private readonly IMemoryCache memoryCache;

        public CommentsController(ICommentRepository repository, IMapper mapper, IMemoryCache memoryCache)
        {
            this.repository = repository;
            this.mapper = mapper;
            this.memoryCache = memoryCache;
        }

        [HttpGet]
        [Authorize(Roles = "Reader,Writer,Admin")]
        public async Task<IActionResult> GetComments(
    [FromQuery] string? filterOn,
    [FromQuery] string? filterQuery,
    [FromQuery] string? sortBy,
    [FromQuery] bool? isAscending,
    [FromQuery] int pageNumber = 1,
    [FromQuery] int pageSize = 20)
        {
            var comments = await repository.GetAllAsync(
                filterOn,
                filterQuery,
                sortBy,
                isAscending ?? true,
                pageNumber,
                pageSize);

            var commentDto = mapper.Map<List<CommentDto>>(comments);

            return Ok(commentDto);
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "Reader,Writer,Admin")]
        public async Task<IActionResult> GetComment(Guid id)
        {
            var comment = await repository.GetByIdAsync(id);
            if (comment == null) return NotFound();
            var commentDto = mapper.Map<CommentDto>(comment);
            return Ok(commentDto);
        }

        [HttpPost]
        [ValidateModel]
        [Authorize(Roles = "Writer,Admin")]
        public async Task<IActionResult> CreateComment([FromBody] CreateCommentDto createCommentDto)
        {
            if (createCommentDto == null) return BadRequest("Comment is null");

            var userId = User.GetUserId(); // Kullanıcı ID'si JWT'den
            var comment = mapper.Map<Comment>(createCommentDto);
            comment.ApplicationUserId = userId;

            var createdComment = await repository.CreateAsync(comment);
            var createdCommentDto = mapper.Map<CommentDto>(createdComment);
            return CreatedAtAction(nameof(GetComment), new { id = createdCommentDto.Id }, createdCommentDto);
        }

        [HttpPut("{id}")]
        [ValidateModel]
        [Authorize(Roles = "Writer,Admin")]
        public async Task<IActionResult> UpdateComment(Guid id, [FromBody] UpdateCommentDto updateCommentDto)
        {
            if (updateCommentDto == null)
                return BadRequest("Comment is null");

            var existingComment = await repository.GetByIdAsync(id);
            if (existingComment == null)
                return NotFound();

            var userId = User.GetUserId();
            if (existingComment.ApplicationUserId != userId)
                return StatusCode(StatusCodes.Status403Forbidden, "Unauthorized.");

            var comment = mapper.Map<Comment>(updateCommentDto);

            // Kullanıcı ID'sini token'dan alıyoruz çünkü DTO'dan artık gelmiyor
            comment.ApplicationUserId = userId;

            var updatedComment = await repository.UpdateAsync(id, comment);
            if (updatedComment == null)
                return NotFound();

            var updatedCommentDto = mapper.Map<CommentDto>(updatedComment);
            return Ok(updatedCommentDto);
        }


        [HttpDelete("{id}")]
        [Authorize(Roles = "Writer,Admin")]
        public async Task<IActionResult> DeleteComment(Guid id)
        {
            var userId = User.GetUserId();
            var comment = await repository.GetByIdAsync(id);

            if (comment == null)
                return NotFound("Comment is null");

            if (!User.IsInRole("Admin") && comment.ApplicationUserId != userId)
                return StatusCode(StatusCodes.Status403Forbidden, "Unauthorized");

            await repository.DeleteAsync(comment.Id);
            return NoContent();
        }

        [HttpGet("byblogpost/{blogPostId}")]
        [Authorize(Roles ="Reader,Writer,Admin")]
        public async Task<IActionResult> GetCommentsByBlogPostId(Guid blogPostId)
        {
            var comments = await repository.GetByBlogPostIdAsync(blogPostId);

            var commentDtos = mapper.Map<List<CommentDto>>(comments);

            return Ok(commentDtos);
        }
    }
}
