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
        [HttpGet]
        [Authorize(Roles = "Reader,Writer,Admin")]
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
        [HttpGet("{id}")]
            [Authorize(Roles = "Reader,Writer,Admin")]
            public async Task<IActionResult> GetBlogPost(Guid id)
            {
                var blogPost = await repository.GetByIdAsync(id);
                if (blogPost == null) return NotFound();
                var blogPostDto = mapper.Map<BlogPostDto>(blogPost);
                return Ok(blogPostDto);
            }

        [HttpPost("with-image")]
        [ValidateModel]
        [Authorize(Roles = "Writer,Admin")]
        public async Task<IActionResult> CreateBlogPost([FromForm] CreateBlogPostDto dto)
        {
            var userId = User.GetUserId();

            var blogPost = new BlogPost
            {
                Title = dto.Title,
                Content = dto.Content,
                ApplicationUserId = userId
            };

            // Resim ekleme varsa
            if (dto.Image != null && dto.Image.Length > 0)
            {
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images");
                if (!Directory.Exists(uploadsFolder))
                    Directory.CreateDirectory(uploadsFolder);

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


        [HttpPut("{id}")]
        [ValidateModel]
        [Authorize(Roles = "Writer,Admin")]
        public async Task<IActionResult> UpdateBlogPost(Guid id, [FromBody] UpdateBlogPostDto updateBlogPostDto)
        {
            if (updateBlogPostDto == null)
                return BadRequest("Blog Post is null");

            var existingBlogPost = await repository.GetByIdAsync(id);
            if (existingBlogPost == null)
                return NotFound();

            var userId = User.GetUserId();

         
            if (existingBlogPost.ApplicationUserId != userId)
                return StatusCode(StatusCodes.Status403Forbidden, "Unauthorized.");

            var blogPost = mapper.Map<BlogPost>(updateBlogPostDto);

            blogPost.ApplicationUserId = userId;

            var updatedBlogPost = await repository.UpdateAsync(id, blogPost);
            if (updatedBlogPost == null)
                return NotFound();

            var updatedBlogPostDto = mapper.Map<BlogPostDto>(updatedBlogPost);
            return Ok(updatedBlogPostDto);
        }


        [HttpDelete("{id}")]
        [Authorize(Roles = "Writer,Admin")]
        public async Task<IActionResult> DeleteBlogPost(Guid id)
        {
            var userId = User.GetUserId();
            var blogPost = await repository.GetByIdAsync(id);

            if (blogPost == null)
                return NotFound("Blog is null");

            if (!User.IsInRole("Admin") && blogPost.ApplicationUserId != userId)
                return StatusCode(StatusCodes.Status403Forbidden, "Unauthorized.");

            await repository.DeleteAsync(blogPost.Id);
            return NoContent();
        }



        [HttpPost("{id}/upload-image")]
        [Authorize(Roles = "Writer,Admin")]
        public async Task<IActionResult> UploadImage(Guid id, IFormFile imageFile)
        {
            var blogPost = await repository.GetByIdAsync(id);
            if (blogPost == null)
                return NotFound("Blog post not found.");

            var userId = User.GetUserId();
            if (blogPost.ApplicationUserId != userId)
                return StatusCode(StatusCodes.Status403Forbidden, "Unauthorized.");

            if (imageFile == null || imageFile.Length == 0)
                return BadRequest("No image file uploaded.");

            var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images");
            if (!Directory.Exists(uploadsFolder))
                Directory.CreateDirectory(uploadsFolder);

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
    }
    }

