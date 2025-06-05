using System.ComponentModel.DataAnnotations;

namespace BLOG.Models
{
    public class Blog
    {
        public int BlogId { get; set; }
        [StringLength(120)]
        public string BlogTitle { get; set; }
        [StringLength(120)]
        public string Author { get; set; }
        public DateTime? CreatedAt { get; set; }

        //Navigation property - represents the "many" side
        public ICollection<Post>? Post { get; set; } = new List<Post>();
    }
}
