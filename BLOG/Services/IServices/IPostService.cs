using BLOG.Models.Dto;

namespace BLOG.Services.IServices
{
    public interface IPostService
    {

        // Creates a new blog with the provided request data
        Task<ResponseType<PostResponseDto>> CreatePost(CreatePostDto request);

        // Searches blogs based on the provided search string
        Task<ResponseType<List<PostResponseDto>>> AscendingPost();
        Task<ResponseType<List<PostResponseDto>>> Descending();
        Task<ResponseType<List<PostResponseDto>>> Ordering_byDate();

        // Updates an existing blog by id with the provided request data
        Task<ResponseType<PostResponseDto>> UpdatePost(UpdatePostDto request);

        // Deletes a blog by id
        Task<ResponseType<PostResponseDto>> DeletePost(int id);
    }
}
