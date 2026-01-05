using API.Controllers;
using AutoMapper;
using DataAccess.Abstract;
using Entities;
using Entities.DTO;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Moq;
using Xunit;

namespace API.Tests
{
    public class BlogPostsControllerTests
    {
  
        private readonly Mock<IBlogPostRepository> _mockRepo;
        private readonly Mock<IMapper> _mockMapper;
        private readonly Mock<IMemoryCache> _mockCache;
        private readonly BlogPostsController _controller;

        public BlogPostsControllerTests()
        {
          
            _mockRepo = new Mock<IBlogPostRepository>();
            _mockMapper = new Mock<IMapper>();
            _mockCache = new Mock<IMemoryCache>();

 
            _controller = new BlogPostsController(_mockRepo.Object, _mockMapper.Object, _mockCache.Object);
        }

        [Fact]
        public async Task GetBlogPost_ShouldReturnOk_WhenBlogPostExists()
        {
       
            var blogPostId = Guid.NewGuid();

    
            var blogPost = new BlogPost
            {
                Id = blogPostId,
                Title = "Test Post",
                Content = "Test Content",
                ApplicationUserId = Guid.NewGuid()
            };

           
            _mockRepo.Setup(x => x.GetByIdAsync(blogPostId))
                     .ReturnsAsync(blogPost);

            _mockMapper.Setup(x => x.Map<BlogPostDto>(blogPost))
                       .Returns(new BlogPostDto { Id = blogPostId, Title = "Test Post" });

     
            var result = await _controller.GetBlogPost(blogPostId);


            result.Should().BeOfType<OkObjectResult>();
        }

        [Fact]
        public async Task GetBlogPost_ShouldReturnNotFound_WhenBlogPostDoesNotExist()
        {
           
            var blogPostId = Guid.NewGuid();

            _mockRepo.Setup(x => x.GetByIdAsync(blogPostId))
                     .ReturnsAsync((BlogPost?)null);

            var result = await _controller.GetBlogPost(blogPostId);

            result.Should().BeOfType<NotFoundResult>();
        }
    }
}