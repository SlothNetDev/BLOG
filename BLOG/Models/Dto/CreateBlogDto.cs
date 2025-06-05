using System.ComponentModel.DataAnnotations;

namespace BLOG.Models.Dto
{
    public record CreateBlogDto(
    [Required(ErrorMessage ="Blog Title Cannot be Empty")]
    string BlogTitle,
    [Required(ErrorMessage ="AuthorName Cannot be Empty")]
    string Author
    );

    public record UpdateBlogDto(
        [Required(ErrorMessage ="Id is Required")]
        int BlogId,
        [MaxLength(120)]
        string BlogTitle,
        [MaxLength(120)]
        string? Author = null
    );

    public record BlogResponseDto(
        int BlogId,
        string BlogTitle,
        string Author,
        DateTime? CreatedAt,
        IEnumerable<PostResponseDto> Posts
    );
    
}
