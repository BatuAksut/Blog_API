using API.CustomActionFilters;
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
           // [Authorize(Roles = "Reader")]
            public async Task<IActionResult> GetBlogPosts([FromQuery] string? filterOn, [FromQuery] string? filterQuery, [FromQuery] string? sortBy, [FromQuery] bool? isAscending,
                [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 1000)
            {

                var cacheKey = $"blogposts_{filterOn}_{filterQuery}_{sortBy}_{isAscending}_{pageNumber}_{pageSize}";
                if (!memoryCache.TryGetValue(cacheKey, out List<BlogPostDto>? blogPostsDto))
                {
                    var blogPosts = await repository.GetAllAsync(filterOn, filterQuery, sortBy, isAscending ?? true, pageNumber, pageSize);
                    blogPostsDto = mapper.Map<List<BlogPostDto>>(blogPosts);

                    var cacheEntryOptions = new MemoryCacheEntryOptions()
                        .SetSlidingExpiration(TimeSpan.FromMinutes(2))
                        .SetAbsoluteExpiration(TimeSpan.FromMinutes(5));
                    memoryCache.Set(cacheKey, blogPostsDto, cacheEntryOptions);
                }
                return Ok(blogPostsDto);
            }

            [HttpGet("{id}")]
            //[Authorize(Roles = "Reader")]
            public async Task<IActionResult> GetBlogPost(Guid id)
            {
                var blogPost = await repository.GetByIdAsync(id);
                if (blogPost == null) return NotFound();
                var blogPostDto = mapper.Map<BlogPostDto>(blogPost);
                return Ok(blogPostDto);
            }


            [HttpPost]
            [ValidateModel]
            //[Authorize(Roles = "Writer")]
            public async Task<IActionResult> CreateBlogPost([FromBody] CreateBlogPostDto createBlogPostDto)
            {
                if (createBlogPostDto == null) return BadRequest("Blog Post is null");

                var blogPost = mapper.Map<BlogPost>(createBlogPostDto);
                var createdBlogPost = await repository.CreateAsync(blogPost);
                var createdBlogPostDto = mapper.Map<BlogPostDto>(createdBlogPost);
                return CreatedAtAction(nameof(GetBlogPost), new { id = createdBlogPost.Id }, createdBlogPostDto);
            }


            [HttpPut("{id}")]
            [ValidateModel]
            //[Authorize(Roles = "Writer")]
            public async Task<IActionResult> UpdateBlogPost(Guid id, [FromBody] UpdateBlogPostDto updateBlogPostDto)
            {
                if (updateBlogPostDto == null) return BadRequest("Book is null");

                var blogPost = mapper.Map<BlogPost>(updateBlogPostDto);
                var updatedBlogPost = await repository.UpdateAsync(id, blogPost);
                if (updatedBlogPost == null) return NotFound();

                var updatedBlogPostDto = mapper.Map<BlogPostDto>(updatedBlogPost);
                return Ok(updatedBlogPostDto);
            }


            [HttpDelete("{id}")]
            //[Authorize(Roles = "Writer")]
            public async Task<IActionResult> DeleteBlogPost(Guid id)
            {
                var blogPost = await repository.DeleteAsync(id);
                if (blogPost == null) return NotFound();
                var blogPostDto = mapper.Map<BlogPostDto>(blogPost);
                return Ok(blogPostDto);
            }
        }
    }

