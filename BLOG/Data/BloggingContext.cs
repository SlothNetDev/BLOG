using BLOG.Models;
using Microsoft.EntityFrameworkCore;

namespace BLOG.Data
{
    public class BloggingContext(DbContextOptions options) : DbContext(options)
    {     
        public DbSet<Blog> Blogs => Set<Blog>();
        public DbSet<Post> Posts => Set<Post>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
          
            modelBuilder.Entity<Blog>()
                .HasMany(b => b.Post)
                .WithOne(b => b.Blog)
                .HasForeignKey(x => x.BlogId);

            
            modelBuilder.Entity<Blog>()
                .HasMany(b => b.Post)
                .WithOne(b => b.Blog)
                .OnDelete(DeleteBehavior.Cascade);

        }
    }

}
