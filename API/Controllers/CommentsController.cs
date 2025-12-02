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
  [Produces("application/json")]
  public class CommentsController : ControllerBase
  {
    private readonly ICommentRepository repository;
    private readonly IMapper mapper;

    public CommentsController(ICommentRepository repository, IMapper mapper)
    {
      this.repository = repository;
      this.mapper = mapper;
    }

    // FIXME: to me, it doesn't make any sense this endpoint. What is the point of seeing comments mixed from one blog post and another?
    // TODO: it's better to have "CommentsByUser" and "CommentsByBlogPost". => is it done?
    // TODO: let me rephrase this.
    // "comments" should be a dependant entity from the "blogpost", and "user" entities.
    // You should have routes like:
    /*
      /api/blog-posts/1/comments => give me all the comments of blog post with ID = 1
      /api/blog-posts/1/comments/23 => give me the comment with ID = 23 of blog post with ID = 1
      /api/users/1/comments => give me all the comments of user with ID = 1
    */
    // in this way, you can have a hierarchical structure.
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
      if (comment == null)
      {
        return NotFound();
      }

      var commentDto = mapper.Map<CommentDto>(comment);
      return Ok(commentDto);
    }

    /// <summary>
    /// Creates a new comment.
    /// </summary>
    /// <param name="createCommentDto">The JSON body containing data to create the comment.</param>
    /// <returns>The newly created comment.</returns>
    [HttpPost]
    [ValidateModel]
    [Authorize(Roles = "Writer,Admin")]
    [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(CommentDto))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> CreateComment([FromBody] CreateCommentDto createCommentDto)
    {
      if (createCommentDto == null)
      {
        return BadRequest("Comment is null");
      }

      var userId = User.GetUserId();
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
      {
        return BadRequest("Update data is null");
      }

      var existingComment = await repository.GetByIdAsync(id);
      if (existingComment == null)
      {
        return NotFound("Comment not found");
      }

      if (!IsUserAuthorizedToEdit(existingComment))
      {
        return StatusCode(StatusCodes.Status403Forbidden, "You can only update your own comments.");
      }

      var comment = mapper.Map<Comment>(updateCommentDto);
      comment.ApplicationUserId = existingComment.ApplicationUserId;

      var updatedComment = await repository.UpdateAsync(id, comment);
      if (updatedComment == null)
      {
        return NotFound("Comment could not be found during update.");
      }

      var updatedCommentDto = mapper.Map<CommentDto>(updatedComment);
      return Ok(updatedCommentDto);
    }

    /// <summary>
    /// Deletes a comment.
    /// </summary>
    /// <param name="id">The ID of the comment to delete.</param>
    /// <returns>Returns no content.</returns>
    [HttpDelete("{id}")]
    [Authorize(Roles = "Writer,Admin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteComment(Guid id)
    {
      var comment = await repository.GetByIdAsync(id);

      if (comment == null)
      {
        return NotFound("Comment is null");
      }

      if (!IsUserAuthorizedToEdit(comment))
      {
        return StatusCode(StatusCodes.Status403Forbidden, "You can only delete your own comments.");
      }

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

    /// <summary>
    /// Gets all comments made by a specific user.
    /// </summary>
    /// <param name="userId">The ID of the user.</param>
    /// <returns>A list of comments made by the user.</returns>
    [HttpGet("byuser/{userId}")]
    [Authorize(Roles = "Reader,Writer,Admin")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<CommentDto>))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetCommentsByUserId(Guid userId)
    {
      var comments = await repository.GetByUserIdAsync(userId);
      var commentDtos = mapper.Map<List<CommentDto>>(comments);
      return Ok(commentDtos);
    }

    [NonAction]
    private bool IsUserAuthorizedToEdit(Comment comment)
    {
      var userId = User.GetUserId();
      var isAdmin = User.IsInRole("Admin");

      if (comment.ApplicationUserId != userId && !isAdmin)
      {
        return false;
      }

      return true;
    }
  }
}
