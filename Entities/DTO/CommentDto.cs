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

        public Guid UserId { get; set; }
        public string Username { get; set; }
        public string Fullname { get; set; }

    }
}
