using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities
{
  public class ApplicationUser : IdentityUser<Guid>
  {

    public string Firstname { get; set; } = string.Empty;
    public string Lastname { get; set; } = string.Empty;

    public List<BlogPost> BlogPosts { get; set; } = new List<BlogPost>();
    public List<Comment> Comments { get; set; } = new List<Comment>();
  }
}
