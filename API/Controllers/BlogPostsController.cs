using API.CustomActionFilters;
using API.Extensions;
using AutoMapper;
using DataAccess.Abstract;
using Entities;
using Entities.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace API.Controllers
{
  [Route("api/[controller]")]
  [ApiController]
  public class BlogPostsController : ControllerBase
  {
    private readonly IBlogPostRepository repository;
    private readonly IMapper mapper;
    private readonly IMemoryCache memoryCache;

    public BlogPostsController(IBlogPostRepository repository, IMapper mapper, IMemoryCache memoryCache)
    {
      this.repository = repository;
      this.mapper = mapper;
      this.memoryCache = memoryCache;
    }

    /// <summary>
    /// Gets all blog posts with filtering, sorting, and pagination.
    /// </summary>
    /// <param name="filterOn">Field to filter on (e.g., "Title", "Content").</param>
    /// <param name="filterQuery">Query string to filter for.</param>
    /// <param name="sortBy">Field to sort by (e.g., "Title", "DateCreated").</param>
    /// <param name="isAscending">Sort direction (true = ascending, false = descending). Default is true.</param>
    /// <param name="pageNumber">Page number.</param>
    /// <param name="pageSize">Number of records per page.</param>
    /// <returns>A list of filtered and paginated blog posts.</returns>
    [HttpGet]
    //[Authorize(Roles = "Reader,Writer,Admin")]
    public async Task<IActionResult> GetBlogPosts(
        [FromQuery] string? filterOn,
        [FromQuery] string? filterQuery,
        [FromQuery] string? sortBy,
        [FromQuery] bool? isAscending,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 20)
    {
      var blogPosts = await repository.GetAllAsync(
          filterOn,
          filterQuery,
          sortBy,
          isAscending ?? true,
          pageNumber,
          pageSize);

      var blogPostsDto = mapper.Map<List<BlogPostDto>>(blogPosts);

      return Ok(blogPostsDto);
    }

    /// <summary>
    /// Gets a single blog post by its ID.
    /// </summary>
    /// <param name="id">The unique ID (GUID) of the blog post to get.</param>
    /// <returns>The found blog post DTO.</returns>
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(BlogPostDto))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetBlogPost(Guid id)
    {
      var blogPost = await repository.GetByIdAsync(id);
      if (blogPost == null)
      {
        return NotFound();
      }
      var blogPostDto = mapper.Map<BlogPostDto>(blogPost);
      return Ok(blogPostDto);
    }

    /// <summary>
    /// Creates a new blog post with an image (multipart/form-data).
    /// </summary>
    /// <param name="dto">The form data containing required fields (Title, Content, Image) to create the blog post.</param>
    /// <returns>The newly created blog post.</returns>
    /// <response code="201">Blog post created successfully.</response>
    /// <response code="400">Model validation failed (e.g., title is missing).</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="403">User does not have the required role (not Writer or Admin).</response>
    [HttpPost("with-image")]
    [ValidateModel]
    [Authorize(Roles = "Writer,Admin")]
    [Consumes("multipart/form-data")]
    [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(BlogPostDto))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> CreateBlogPost([FromForm] CreateBlogPostDto dto)
    {
      var userId = User.GetUserId();

      var blogPost = new BlogPost
      {
        Title = dto.Title,
        Content = dto.Content,
        ApplicationUserId = userId
      };

      if (dto.Image != null && dto.Image.Length > 0)
      {
        var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images");

        var uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(dto.Image.FileName);
        var filePath = Path.Combine(uploadsFolder, uniqueFileName);

        using (var stream = new FileStream(filePath, FileMode.Create))
        {
          await dto.Image.CopyToAsync(stream);
        }

        blogPost.ImageUrl = $"/images/{uniqueFileName}";
      }

      var created = await repository.CreateAsync(blogPost);
      var createdDto = mapper.Map<BlogPostDto>(created);
      return CreatedAtAction(nameof(GetBlogPost), new { id = created.Id }, createdDto);
    }

    /// <summary>
    /// Updates an existing blog post.
    /// </summary>
    /// <param name="id">The ID of the blog post to update.</param>
    /// <param name="updateBlogPostDto">The JSON body containing the updated data.</param>
    /// <returns>The updated blog post DTO.</returns>
    [HttpPut("{id}")]
    [ValidateModel]
    [Authorize(Roles = "Writer,Admin")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(BlogPostDto))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateBlogPost(Guid id, [FromBody] UpdateBlogPostDto updateBlogPostDto)
    {
      if (updateBlogPostDto == null)
      {
        return BadRequest("Blog Post is null");
      }

      var existingBlogPost = await repository.GetByIdAsync(id);
      if (existingBlogPost == null)
      {
        return NotFound();
      }

      if (!IsUserAuthorizedToEdit(existingBlogPost))
      {
        return StatusCode(StatusCodes.Status403Forbidden, "You are not allowed to update this post. You can only edit your own posts.");
      }

      var blogPost = mapper.Map<BlogPost>(updateBlogPostDto);
      blogPost.ApplicationUserId = existingBlogPost.ApplicationUserId;
            blogPost.CreatedAt = existingBlogPost.CreatedAt;
            blogPost.ImageUrl = existingBlogPost.ImageUrl; 

            var updatedBlogPost = await repository.UpdateAsync(id, blogPost);
      if (updatedBlogPost == null)
      {
        return NotFound("Blog post could not be found during update.");
      }

      var updatedBlogPostDto = mapper.Map<BlogPostDto>(updatedBlogPost);
      return Ok(updatedBlogPostDto);
    }

    /// <summary>
    /// Deletes a blog post.
    /// </summary>
    /// <param name="id">The ID of the blog post to delete.</param>
    /// <returns>Returns no content.</returns>
    [HttpDelete("{id}")]
    [Authorize(Roles = "Writer,Admin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteBlogPost(Guid id)
    {
      var blogPost = await repository.GetByIdAsync(id);

      if (blogPost == null)
      {
        return NotFound("Resource does not exist");
      }

      if (!IsUserAuthorizedToEdit(blogPost))
      {
        return StatusCode(StatusCodes.Status403Forbidden, "You can only delete your own posts.");
      }

      await repository.DeleteAsync(blogPost.Id);
      return NoContent();
    }

    /// <summary>
    /// Uploads a cover image for an existing blog post (multipart/form-data).
    /// </summary>
    /// <param name="id">The ID of the blog post to upload the image for.</param>
    /// <param name="imageFile">The image file to upload.</param>
    /// <returns>An object containing the new URL of the uploaded image.</returns>
    /// <response code="200">Image uploaded successfully and its URL is returned.</response>
    /// <response code="400">No file was uploaded.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="403">User is not the owner of this blog post.</response>
    /// <response code="404">Blog post with the specified ID was not found.</response>
    [HttpPost("{id}/upload-image")]
    [Authorize(Roles = "Writer,Admin")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UploadImage(Guid id, IFormFile imageFile)
    {
      var blogPost = await repository.GetByIdAsync(id);
      if (blogPost == null)
      {
        return NotFound("Blog post not found.");
      }

      if (!IsUserAuthorizedToEdit(blogPost))
      {
        return StatusCode(StatusCodes.Status403Forbidden, "You can only upload images to your own posts.");
      }

      if (imageFile == null || imageFile.Length == 0)
      {
        return BadRequest("No image file uploaded.");
      }

      var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images");

      var uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(imageFile.FileName);
      var filePath = Path.Combine(uploadsFolder, uniqueFileName);

      using (var stream = new FileStream(filePath, FileMode.Create))
      {
        await imageFile.CopyToAsync(stream);
      }

      blogPost.ImageUrl = $"/images/{uniqueFileName}";
      await repository.UpdateAsync(id, blogPost);
      return Ok(new { imageUrl = blogPost.ImageUrl });
    }

    [NonAction]
    private bool IsUserAuthorizedToEdit(BlogPost blogPost)
    {
      var userId = User.GetUserId();


      // [Q]: why do you want a user to be "admin" to change his blog posts?
      // TODO: the logic is contrived, it would make more sense something like:
      /*
      if (blogPost.ApplicationUserId == userId)
      {
        return true;
      }
      return false
      */
      //changed
      if (blogPost.ApplicationUserId != userId)
      {
        return false;
      }

      return true;
    }
  }
}
