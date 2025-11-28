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
  public class CommentRepository : ICommentRepository
  {
    private readonly AppDbContext context;

    public CommentRepository(AppDbContext context)
    {
      this.context = context;
    }
    public async Task<Comment> CreateAsync(Comment comment)
    {
      var commentToAdd = new Comment
      {
        BlogPostId = comment.BlogPostId,
        Content = comment.Content,
        ApplicationUserId = comment.ApplicationUserId,
        CreatedAt = DateTime.UtcNow
      };

      await context.Comments.AddAsync(commentToAdd);
      await context.SaveChangesAsync();

      // FIXME: warning
      return commentToAdd;
    }

    public async Task<Comment?> DeleteAsync(Guid id)
    {
      var commentToDelete = await context.Comments.Include(x => x.ApplicationUser).Include(x => x.BlogPost).FirstOrDefaultAsync(x => x.Id == id);
      if (commentToDelete == null) return null;

      context.Comments.Remove(commentToDelete);
      await context.SaveChangesAsync();
      return commentToDelete;
    }

    public async Task<List<Comment>> GetAllAsync(
string? filterOn = null,
string? filterQuery = null,
string? sortBy = null,
bool isAscending = true,
int pageNumber = 1,
int pageSize = 20)
    {
      // Maksimum pageSize sınırı koyduk
      pageSize = Math.Min(pageSize, 100);

      var comments = context.Comments
          .Include(x => x.ApplicationUser)
          .Include(x => x.BlogPost)
          .AsQueryable();

      // Filtering
      if (!string.IsNullOrWhiteSpace(filterOn) && !string.IsNullOrWhiteSpace(filterQuery))
      {
        switch (filterOn.ToLower())
        {
          case "content":
            comments = comments.Where(x => x.Content.Contains(filterQuery));
            break;
        }
      }

      // Sorting
      if (!string.IsNullOrWhiteSpace(sortBy))
      {
        if (sortBy.Equals("content", StringComparison.OrdinalIgnoreCase))
        {
          comments = isAscending ? comments.OrderBy(x => x.Content) : comments.OrderByDescending(x => x.Content);
        }
        else if (sortBy.Equals("date", StringComparison.OrdinalIgnoreCase) || sortBy.Equals("createdat", StringComparison.OrdinalIgnoreCase))
        {
          comments = isAscending ? comments.OrderBy(x => x.CreatedAt) : comments.OrderByDescending(x => x.CreatedAt);
        }
      }
      else
      {
        // New feature: Default sorting
        comments = comments.OrderByDescending(x => x.CreatedAt);
      }

      // Pagination
      var skipResults = (pageNumber - 1) * pageSize;
      return await comments.Skip(skipResults).Take(pageSize).ToListAsync();
    }

    public async Task<Comment?> GetByIdAsync(Guid id)
    {
      // [Q]: Why you're always including User & BlogPost information?
      // from a performance perspective is not ideal. We can save this discussion for the future BTW.
      // [A]: Because usually when we get a comment, we want to see who made the comment and on which blog post on front end.
      // [Q]: likely if you're fetching a comment by ID, it means you already fetched the BlogPost (and since the only the owner can fetch his own blog posts you also have the user information).
      // Deleted
       return await context.Comments
                .Include(x => x.ApplicationUser)
                //.Include(x => x.BlogPost)
                .FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<Comment?> UpdateAsync(Guid id, Comment comment)
    {
      var commentToUpdate = await context.Comments.FirstOrDefaultAsync(x => x.Id == id);

      if (commentToUpdate == null) return null;

      var userExists = await context.Users.AnyAsync(x => x.Id == comment.ApplicationUserId);
      var blogExists = await context.BlogPosts.AnyAsync(x => x.Id == comment.BlogPostId);

      if (!userExists)
      {
        throw new Exception("User does not exist.");
      }
      if (!blogExists)
      {
        throw new Exception("Blog post does not exist.");
      }


      commentToUpdate.Content = comment.Content;
      commentToUpdate.BlogPostId = comment.BlogPostId;
      commentToUpdate.ApplicationUserId = comment.ApplicationUserId;
      context.Comments.Update(commentToUpdate);
      await context.SaveChangesAsync();
      return commentToUpdate;

    }

    public async Task<List<Comment>> GetByBlogPostIdAsync(Guid blogPostId)
    {
      return await context.Comments
          .Include(x => x.ApplicationUser)
          .Include(x => x.BlogPost)
          .Where(x => x.BlogPostId == blogPostId)
          .ToListAsync();
    }

        public async Task<List<Comment>> GetByUserIdAsync(Guid userId)
        {
         
            return await context.Comments
                .Include(x => x.BlogPost)
                .Where(x => x.ApplicationUserId == userId)
                .OrderByDescending(x => x.CreatedAt) 
                .ToListAsync();
        }
    }
}
