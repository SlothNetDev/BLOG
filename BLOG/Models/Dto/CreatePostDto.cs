using Microsoft.Extensions.Hosting;
using Microsoft.VisualBasic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace BLOG.Models.Dto
{
    #region Post
     public record CreatePostDto(
     [Required(ErrorMessage ="Id is Required")]
     int BlogId,

     [Required(ErrorMessage ="Title is Required")]
     [StringLength(120)]
     string Title,

     [Required(ErrorMessage = "Content is Required")]
     string Content
     );

    public record UpdatePostDto(
        [Required(ErrorMessage ="Id is Required")]
        int PostId,
        [StringLength(120)]
        string? Title,
        string? Content);
    
    public record PostResponseDto(
        int PostId,
        string Title,
        string Content,
        DateTime? CreatedAt);

    // For minimal response (when Blog isn't needed)
    public record PostWithBlogResponseDto(
        int PostId,
        string Title,
        string Content,
        DateTime? CreatedAt,
        int BlogId
    );

    #endregion
}
