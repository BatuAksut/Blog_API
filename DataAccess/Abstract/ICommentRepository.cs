using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Abstract
{
    public interface ICommentRepository
    {
        Task<List<Comment>> GetAllAsync(string? filterOn = null, string? filterQuery = null, string? sortBy = null, bool isAscending = true, int pageNumber = 1, int pageSize = 1000);
        Task<Comment?> GetByIdAsync(Guid id);
        Task<Comment> CreateAsync(Comment comment);
        Task<Comment?> UpdateAsync(Guid id, Comment comment);
        Task<Comment?> DeleteAsync(Guid id);
        Task<List<Comment>> GetByBlogPostIdAsync(Guid blogPostId);

    }
}
