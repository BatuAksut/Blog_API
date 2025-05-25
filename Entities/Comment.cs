using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities
{
    public class Comment:BaseEntity<Guid>
    {
        public string Content { get; set; }
        public Guid BlogPostId { get; set; }
        public BlogPost BlogPost { get; set; }

        public Guid ApplicationUserId { get; set; }
        public ApplicationUser ApplicationUser { get; set; }

    }
}
