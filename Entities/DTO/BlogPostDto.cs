using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.DTO
{
    public class BlogPostDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public UserDto User { get; set; } = null!;
        public List<CommentDto> Comments { get; set; } = new List<CommentDto>();
        public string? ImageUrl { get; set; }
    }
}
