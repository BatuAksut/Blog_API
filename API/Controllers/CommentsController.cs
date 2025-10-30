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
using System.Collections.Generic; 
using System; 

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")] // Controller'ın varsayılan olarak JSON döndürdüğünü belirtir
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

        /// <summary>
        /// Gets all comments with filtering, sorting, and pagination.
        /// </summary>
        /// <param name="filterOn">Field to filter on (e.g., "Content").</param>
        /// <param name="filterQuery">Query string to filter for.</param>
        /// <param name="sortBy">Field to sort by (e.g., "DateCreated").</param>
        /// <param name="isAscending">Sort direction (true = ascending, false = descending). Default is true.</param>
        /// <param name="pageNumber">Page number. Default is 1.</param>
        /// <param name="pageSize">Number of records per page. Default is 20.</param>
        /// <returns>A list of filtered and paginated comments.</returns>
        [HttpGet]
        [Authorize(Roles = "Reader,Writer,Admin")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<CommentDto>))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
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

        /// <summary>
        /// Gets a single comment by its ID.
        /// </summary>
        /// <param name="id">The unique ID (GUID) of the comment to get.</param>
        /// <returns>The found comment DTO.</returns>
        [HttpGet("{id}")]
        [Authorize(Roles = "Reader,Writer,Admin")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(CommentDto))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetComment(Guid id)
        {
            var comment = await repository.GetByIdAsync(id);
            if (comment == null) return NotFound();
            var commentDto = mapper.Map<CommentDto>(comment);
            return Ok(commentDto);
        }

        /// <summary>
        /// Creates a new comment.
        /// </summary>
        /// <param name="createCommentDto">The JSON body containing data to create the comment.</param>
        /// <returns>The newly created comment.</returns>
        /// <response code="201">Comment created successfully.</response>
        /// <response code="400">Model validation failed or the request body is null.</response>
        /// <response code="401">User is not authenticated.</response>
        /// <response code="403">User does not have the required role (not Writer or Admin).</response>
        [HttpPost]
        [ValidateModel]
        [Authorize(Roles = "Writer,Admin")]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(CommentDto))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> CreateComment([FromBody] CreateCommentDto createCommentDto)
        {
            if (createCommentDto == null) return BadRequest("Comment is null");

            var userId = User.GetUserId(); // User ID from JWT
            var comment = mapper.Map<Comment>(createCommentDto);
            comment.ApplicationUserId = userId;

            var createdComment = await repository.CreateAsync(comment);
            var createdCommentDto = mapper.Map<CommentDto>(createdComment);
            return CreatedAtAction(nameof(GetComment), new { id = createdCommentDto.Id }, createdCommentDto);
        }

        /// <summary>
        /// Updates an existing comment.
        /// </summary>
        /// <param name="id">The ID of the comment to update.</param>
        /// <param name="updateCommentDto">The JSON body containing the updated data.</param>
        /// <returns>The updated comment DTO.</returns>
        /// <response code="200">Comment updated successfully.</response>
        /// <response code="400">Model validation failed or the request body is null.</response>
        /// <response code="401">User is not authenticated.</response>
        /// <response code="403">User is not the owner of this comment.</response>
        /// <response code="404">Comment with the specified ID was not found.</response>
        [HttpPut("{id}")]
        [ValidateModel]
        [Authorize(Roles = "Writer,Admin")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(CommentDto))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
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

            // We get the User ID from the token, as it's not in the DTO
            comment.ApplicationUserId = userId;

            var updatedComment = await repository.UpdateAsync(id, comment);
            if (updatedComment == null)
                return NotFound();

            var updatedCommentDto = mapper.Map<CommentDto>(updatedComment);
            return Ok(updatedCommentDto);
        }

        /// <summary>
        /// Deletes a comment.
        /// </summary>
        /// <param name="id">The ID of the comment to delete.</param>
        /// <returns>Returns no content.</returns>
        /// <response code="204">Comment deleted successfully.</response>
        /// <response code="401">User is not authenticated.</response>
        /// <response code="403">User is not the owner of the comment or not an Admin.</response>
        /// <response code="404">Comment with the specified ID was not found.</response>
        [HttpDelete("{id}")]
        [Authorize(Roles = "Writer,Admin")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
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

        /// <summary>
        /// Gets all comments for a specific blog post.
        /// </summary>
        /// <param name="blogPostId">The ID of the blog post to retrieve comments for.</param>
        /// <returns>A list of comments for the specified blog post.</returns>
        [HttpGet("byblogpost/{blogPostId}")]
        [Authorize(Roles = "Reader,Writer,Admin")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<CommentDto>))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetCommentsByBlogPostId(Guid blogPostId)
        {
            var comments = await repository.GetByBlogPostIdAsync(blogPostId);
            var commentDtos = mapper.Map<List<CommentDto>>(comments);
            return Ok(commentDtos);
        }
    }
}