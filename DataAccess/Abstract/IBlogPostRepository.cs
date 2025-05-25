using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Abstract
{
    public interface IBlogPostRepository
    {
        Task<List<BlogPost>> GetAllAsync(string? filterOn = null, string? filterQuery = null, string? sortBy = null, bool isAscending = true, int pageNumber = 1, int pageSize = 1000);
        Task<BlogPost?> GetByIdAsync(Guid id);
        Task<BlogPost> CreateAsync(BlogPost blogPost);
        Task<BlogPost?> UpdateAsync(Guid id, BlogPost blogPost);
        Task<BlogPost?> DeleteAsync(Guid id);
    }
}
