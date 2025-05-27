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
        [Authorize(Roles = "Reader,Writer")]
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
            [Authorize(Roles = "Reader,Writer")]
            public async Task<IActionResult> GetBlogPost(Guid id)
            {
                var blogPost = await repository.GetByIdAsync(id);
                if (blogPost == null) return NotFound();
                var blogPostDto = mapper.Map<BlogPostDto>(blogPost);
                return Ok(blogPostDto);
            }

        [HttpPost]
        [ValidateModel]
        [Authorize(Roles = "Writer")]
        public async Task<IActionResult> CreateBlogPost([FromBody] CreateBlogPostDto createBlogPostDto)
        {
            if (createBlogPostDto == null) return BadRequest("Blog Post is null");

            var userId = User.GetUserId(); 
            var blogPost = mapper.Map<BlogPost>(createBlogPostDto);
            blogPost.ApplicationUserId = userId; 

            var createdBlogPost = await repository.CreateAsync(blogPost);
            var createdBlogPostDto = mapper.Map<BlogPostDto>(createdBlogPost);
            return CreatedAtAction(nameof(GetBlogPost), new { id = createdBlogPost.Id }, createdBlogPostDto);
        }

        [HttpPut("{id}")]
        [ValidateModel]
        [Authorize(Roles = "Writer")]
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
        [Authorize]
        public async Task<IActionResult> DeleteBlogPost(Guid id)
        {
            var userId = User.GetUserId();
            var blogPost = await repository.GetByIdAsync(id);

            if (blogPost == null)
                return NotFound("Blog is null");

            if (blogPost.ApplicationUserId != userId)
                return StatusCode(StatusCodes.Status403Forbidden, "Unauthorized.");

            await repository.DeleteAsync(blogPost.Id);
            return NoContent();
        }
    }
    }

