using System.ComponentModel.DataAnnotations;
// TODO: warnings to address
namespace Entities
{
  public class BlogPost : BaseEntity<Guid>
  {
    public string Title { get; set; }
    public string Content { get; set; }
    public Guid ApplicationUserId { get; set; }
    public ApplicationUser ApplicationUser { get; set; }

    public List<Comment> Comments { get; set; } = new List<Comment>();

    [Url]
    public string? ImageUrl { get; set; }

  }
}
