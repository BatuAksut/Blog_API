namespace Entities
{
  public class BlogPost : BaseEntity<Guid>
  {
    public string Title { get; set; }
    public string Content { get; set; }
    public Guid ApplicationUserId { get; set; }
    public ApplicationUser ApplicationUser { get; set; }

    public List<Comment> Comments { get; set; } = new List<Comment>();

    // TODO: some URL validation? I know there's something built-in already provided.
    public string? ImageUrl { get; set; }

  }
}
