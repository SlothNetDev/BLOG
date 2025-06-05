using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BLOG.Models
{
    public class Post
    {
        public int PostId { get; set; }
        [StringLength(120)]
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty ;
        public DateTime? CreatedAt { get; set; }

        //Foreign key property
        public int BlogId { get; set; }
        
        //Navigate property - represents the "one side"
        public Blog? Blog { get; set; } 
    }
}
