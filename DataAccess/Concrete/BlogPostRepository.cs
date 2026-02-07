using DataAccess.Abstract;
using Entities;
using Microsoft.EntityFrameworkCore;
using Sieve.Models;
using Sieve.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Concrete
{
  public class BlogPostRepository : IBlogPostRepository
  {
    private readonly AppDbContext context;
    private readonly ISieveProcessor sieveProcessor;

    public BlogPostRepository(AppDbContext context, ISieveProcessor sieveProcessor)
    {
      this.context = context;
      this.sieveProcessor = sieveProcessor;
    }

    public async Task<BlogPost> CreateAsync(BlogPost blogPost)
    {

      var newBlogPost = new BlogPost
      {
        Title = blogPost.Title,
        Content = blogPost.Content,
        ApplicationUserId = blogPost.ApplicationUserId,
        ImageUrl = blogPost.ImageUrl
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

    // NICETOHAVE: evaluate if you can use something like the Sieve model to not do everything by hand.
    // TODO: I'll test it out as soon as I can safely register and consume the APIs.
    public async Task<List<BlogPost>> GetAllAsync(
string? filterOn = null,
string? filterQuery = null,
string? sortBy = null,
bool isAscending = true,
int pageNumber = 1,
int pageSize = 20)
    {

      var model = new SieveModel
      {
        Filters = string.IsNullOrWhiteSpace(filterOn) || string.IsNullOrWhiteSpace(filterQuery)
       ? null
       : $"{filterOn}@={filterQuery}",
        Sorts = sortBy,
        Page = pageNumber,
        PageSize = pageSize
      };

      pageSize = Math.Min(pageSize, 100);

      // FIXME: this is known as the N+1 problem.
      // you have plenty of ways to mitigate this:
      // conditional includes, separate methods, projections of the selected fields, etc.
      // do a bit of research

      var collection = context.BlogPosts.AsNoTracking().AsQueryable();
      collection = sieveProcessor.Apply(model, collection);
      return await collection.Select(x => new BlogPost
            {
                Id = x.Id,
                Title = x.Title,
                Content = x.Content,
                ImageUrl = x.ImageUrl,
                CreatedAt = x.CreatedAt,
                ApplicationUserId = x.ApplicationUserId,

                ApplicationUser = new ApplicationUser
                {
                    Id = x.ApplicationUser.Id,
                    UserName = x.ApplicationUser.UserName,
                    Email = x.ApplicationUser.Email
                }
            }).ToListAsync();
        }


    public async Task<BlogPost?> GetByIdAsync(Guid id)
    {
      return await context.BlogPosts
  .Include(x => x.ApplicationUser)
  .Include(x => x.Comments)
      .ThenInclude(c => c.ApplicationUser)
      .AsSplitQuery()
  .FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<BlogPost?> UpdateAsync(Guid id, BlogPost blogPost)
    {
      var blogPostToUpdate = await context.BlogPosts.FirstOrDefaultAsync(x => x.Id == id);
      if (blogPostToUpdate == null)
      {
        return null;
      }




      blogPostToUpdate.Title = blogPost.Title;
      blogPostToUpdate.Content = blogPost.Content;
      blogPostToUpdate.ApplicationUserId = blogPost.ApplicationUserId;
      blogPostToUpdate.ImageUrl = blogPost.ImageUrl ?? blogPostToUpdate.ImageUrl;

      context.BlogPosts.Update(blogPostToUpdate);
      await context.SaveChangesAsync();

      return blogPostToUpdate;
    }
  }
}

