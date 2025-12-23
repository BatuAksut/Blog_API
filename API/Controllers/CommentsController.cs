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
  // FIXME: the name of the endpoint address must be lowercase.
  // fixed below
  // TODO: have a look at this standard guidelines: https://opensource.zalando.com/restful-api-guidelines/#table-of-contents
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

    /// <summary>
    /// Creates a new comment.
    /// </summary>
    /// <param name="blogPostId">The unique ID of the blog post to add the comment to.</param>
    /// <param name="createCommentDto">The JSON body containing data to create the comment.</param>
    /// <returns>The newly created comment.</returns>
    [HttpPost("/api/blog-posts/{blogPostId:guid}/comments")]
    [ValidateModel]
    [Authorize(Roles = "Writer,Admin")]
    [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(CommentDto))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> CreateComment([FromRoute] Guid blogPostId, [FromBody] CreateCommentDto createCommentDto)
    {
      if (createCommentDto == null)
      {
        return BadRequest("Comment is null");
      }


      var userId = User.GetUserId();
      var comment = mapper.Map<Comment>(createCommentDto);
      comment.ApplicationUserId = userId;
      comment.BlogPostId = blogPostId;

      var createdComment = await repository.CreateAsync(comment);
      var createdCommentDto = mapper.Map<CommentDto>(createdComment);

      return CreatedAtAction(
             nameof(GetCommentForBlogPost),
             new { blogPostId = blogPostId, id = createdCommentDto.Id },
             createdCommentDto);
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
      comment.BlogPostId = existingComment.BlogPostId;

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
    [HttpGet("/api/blog-posts/{blogPostId:guid}/comments")]
    //[Authorize(Roles = "Reader,Writer,Admin")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<CommentDto>))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetCommentsByBlogPost(Guid blogPostId)
    {
      var comments = await repository.GetByBlogPostIdAsync(blogPostId);
      var commentDtos = mapper.Map<List<CommentDto>>(comments);
      return Ok(commentDtos);
    }


    /// <summary>
    /// Gets a specific comment under a specific blog post.
    /// Route: /api/blog-posts/{blogPostId}/comments/{id}
    /// </summary>
    [HttpGet("/api/blog-posts/{blogPostId:guid}/comments/{id:guid}")]
    [Authorize(Roles = "Reader,Writer,Admin")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(CommentDto))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetCommentForBlogPost([FromRoute] Guid blogPostId, [FromRoute] Guid id)
    {
      var comment = await repository.GetByIdAsync(id);


      if (comment == null || comment.BlogPostId != blogPostId)
      {
        return NotFound("Comment not found for this blog post.");
      }

      var commentDto = mapper.Map<CommentDto>(comment);
      return Ok(commentDto);
    }

    /// <summary>
    /// Gets all comments made by a specific user.
    /// Route: /api/users/{userId}/comments
    /// </summary>
    [HttpGet("/api/users/{userId:guid}/comments")]
    [Authorize(Roles = "Reader,Writer,Admin")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<CommentDto>))]
    public async Task<IActionResult> GetCommentsByUser([FromRoute] Guid userId)
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
