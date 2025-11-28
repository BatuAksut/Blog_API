using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.DTO
{
    public class CreateCommentDto
    {
        [Required]
        public string Content { get; set; } = string.Empty;

        [Required]
        public Guid BlogPostId { get; set; }

        
    }
}
