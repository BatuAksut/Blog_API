using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.DTO
{

    public class CommentDto
    {
        public Guid Id { get; set; }
        public string Content { get; set; }
        public Guid BlogPostId { get; set; }

        public UserDto User { get; set; }

        public DateTime CreatedAt { get; set; }
    }

}
