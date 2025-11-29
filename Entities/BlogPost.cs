using Sieve.Attributes;
using System.ComponentModel.DataAnnotations;
// TODO: warnings to address
namespace Entities
{
  public class BlogPost : BaseEntity<Guid>
  {
    [Sieve(CanFilter = true, CanSort = true)]
    public string Title { get; set; } = string.Empty;
    [Sieve(CanFilter = true, CanSort = true)]
    public string Content { get; set; } = string.Empty;
    public Guid ApplicationUserId { get; set; }
    public ApplicationUser ApplicationUser { get; set; } = null!;

    public List<Comment> Comments { get; set; } = new List<Comment>();

    [Url]
    public string? ImageUrl { get; set; }

  }
}
