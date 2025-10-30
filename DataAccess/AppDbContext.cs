using Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace DataAccess
{
    public class AppDbContext : IdentityDbContext<ApplicationUser, IdentityRole<Guid>, Guid>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<BlogPost> BlogPosts { get; set; }
        public DbSet<Comment> Comments { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            var user1Id = Guid.Parse("00000000-0000-0000-0000-000000000001");
            var user2Id = Guid.Parse("00000000-0000-0000-0000-000000000002");

            modelBuilder.Entity<ApplicationUser>().HasData(
                new ApplicationUser
                {
                    Id = user1Id,
                    UserName = "reader1",
                    NormalizedUserName = "READER1",
                    Email = "reader1@example.com",
                    NormalizedEmail = "READER1@EXAMPLE.COM",
                    EmailConfirmed = true,
                    Firstname = "Reader",
                    Lastname = "One",
                    PasswordHash = new PasswordHasher<ApplicationUser>().HashPassword(null, "YourPassword123!")
                },
                new ApplicationUser
                {
                    Id = user2Id,
                    UserName = "writer1",
                    NormalizedUserName = "WRITER1",
                    Email = "writer1@example.com",
                    NormalizedEmail = "WRITER1@EXAMPLE.COM",
                    EmailConfirmed = true,
                    Firstname = "Writer",
                    Lastname = "One",
                    PasswordHash = new PasswordHasher<ApplicationUser>().HashPassword(null, "YourPassword123!")
                }
            );

            modelBuilder.Entity<BlogPost>()
                .HasOne(b => b.ApplicationUser)
                .WithMany(u => u.BlogPosts)
                .HasForeignKey(b => b.ApplicationUserId);

            modelBuilder.Entity<Comment>()
                .HasOne(c => c.BlogPost)
                .WithMany(b => b.Comments)
                .HasForeignKey(c => c.BlogPostId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Comment>()
                .HasOne(c => c.ApplicationUser)
                .WithMany(u => u.Comments)
                .HasForeignKey(c => c.ApplicationUserId)
                .OnDelete(DeleteBehavior.Restrict);

        
            var readerRoleId = Guid.Parse("11111111-1111-1111-1111-111111111111");
            var writerRoleId = Guid.Parse("22222222-2222-2222-2222-222222222222");
            var adminRoleId = Guid.Parse("33333333-3333-3333-3333-333333333333");

            var roles = new List<IdentityRole<Guid>>()
            {
                new IdentityRole<Guid>()
                {
                    Id = readerRoleId,
                    Name = "Reader",
                    NormalizedName = "READER"
                },
                new IdentityRole<Guid>()
                {
                    Id = writerRoleId,
                    Name = "Writer",
                    NormalizedName = "WRITER"
                },
                new IdentityRole<Guid>()
                {
                    Id = adminRoleId,
                    Name = "Admin",
                    NormalizedName = "ADMIN"
                }
            };

            modelBuilder.Entity<IdentityRole<Guid>>().HasData(roles);

            var blogPost1Id = Guid.Parse("44444444-4444-4444-4444-444444444444");
            var blogPost2Id = Guid.Parse("55555555-5555-5555-5555-555555555555");

            modelBuilder.Entity<BlogPost>().HasData(
                new BlogPost
                {
                    Id = blogPost1Id,
                    Title = "First Blog Post",
                    Content = "This is the content of the first blog post.",
                    ApplicationUserId = user1Id
                },
                new BlogPost
                {
                    Id = blogPost2Id,
                    Title = "Second Blog Post",
                    Content = "This is the content of the second blog post.",
                    ApplicationUserId = user1Id
                }
            );

            modelBuilder.Entity<Comment>().HasData(
                new Comment
                {
                    Id = Guid.Parse("66666666-6666-6666-6666-666666666666"),
                    Content = "This is a comment on the first blog post.",
                    BlogPostId = blogPost1Id,
                    ApplicationUserId = user2Id
                },
                new Comment
                {
                    Id = Guid.Parse("77777777-7777-7777-7777-777777777777"),
                    Content = "This is a comment on the second blog post.",
                    BlogPostId = blogPost2Id,
                    ApplicationUserId = user2Id
                }
            );
        }
    }
}
