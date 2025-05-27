using DataAccess.Abstract;
using Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Concrete
{
    public class BlogPostRepository:IBlogPostRepository
    {
        private readonly AppDbContext context;

        public BlogPostRepository(AppDbContext context)
        {
            this.context = context;
        }

        public async Task<BlogPost> CreateAsync(BlogPost blogPost)
        {

            var newBlogPost = new BlogPost
            {
                Title = blogPost.Title,
                Content = blogPost.Content,
                ApplicationUserId = blogPost.ApplicationUserId,
            };

            await context.BlogPosts.AddAsync(newBlogPost);
            await context.SaveChangesAsync();
            return newBlogPost;
        }

        public async Task<BlogPost?> DeleteAsync(Guid id)
        {
            var blogPostToDelete = await context.BlogPosts.Include(x => x.ApplicationUser).Include(x => x.Comments).FirstOrDefaultAsync(x => x.Id == id);
            if (blogPostToDelete == null) return null;

            context.BlogPosts.Remove(blogPostToDelete);
            await context.SaveChangesAsync();
            return blogPostToDelete;
        }

        public async Task<List<BlogPost>> GetAllAsync(
    string? filterOn = null,
    string? filterQuery = null,
    string? sortBy = null,
    bool isAscending = true,
    int pageNumber = 1,
    int pageSize = 20)
        {
            // Maksimum pageSize sınırı koyduk
            pageSize = Math.Min(pageSize, 100);

            var blogPosts = context.BlogPosts
                .Include(x => x.ApplicationUser)
                .Include(x => x.Comments)
                    .ThenInclude(c => c.ApplicationUser)
                .AsQueryable();

            // Filtering
            if (!string.IsNullOrWhiteSpace(filterOn) && !string.IsNullOrWhiteSpace(filterQuery))
            {
                switch (filterOn.ToLower())
                {
                    case "title":
                        blogPosts = blogPosts.Where(x => x.Title.Contains(filterQuery));
                        break;
                    case "content":
                        blogPosts = blogPosts.Where(x => x.Content.Contains(filterQuery));
                        break;
                }
            }

            // Sorting
            if (!string.IsNullOrWhiteSpace(sortBy))
            {
                blogPosts = sortBy.ToLower() switch
                {
                    "title" => isAscending ? blogPosts.OrderBy(x => x.Title) : blogPosts.OrderByDescending(x => x.Title),
                    _ => blogPosts
                };
            }

            // Pagination
            var skipResults = (pageNumber - 1) * pageSize;
            return await blogPosts.Skip(skipResults).Take(pageSize).ToListAsync();
        }
        public async Task<BlogPost?> GetByIdAsync(Guid id)
        {
            return await context.BlogPosts
        .Include(x => x.ApplicationUser)
        .Include(x => x.Comments)
            .ThenInclude(c => c.ApplicationUser)
        .FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<BlogPost?> UpdateAsync(Guid id, BlogPost blogPost)
        {
            var blogPostToUpdate = await context.BlogPosts.FirstOrDefaultAsync(x => x.Id == id);
            if (blogPostToUpdate == null) return null;

            // Kullanıcı gerçekten var mı kontrolü
            var userExists = await context.Users.AnyAsync(x => x.Id == blogPost.ApplicationUserId);
            if (!userExists)
                throw new Exception("ApplicationUser does not exist.");

            blogPostToUpdate.Title = blogPost.Title;
            blogPostToUpdate.Content = blogPost.Content;
            blogPostToUpdate.ApplicationUserId = blogPost.ApplicationUserId;

            context.BlogPosts.Update(blogPostToUpdate);
            await context.SaveChangesAsync();

            return blogPostToUpdate;
        }
    }
}

