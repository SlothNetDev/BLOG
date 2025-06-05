using Azure;
using BLOG.Models;
using BLOG.Models.Dto;

namespace BLOG.Services.IServices
{
    // Interface for blog-related service operations
    public interface IBlogService
    {
        // Retrieves a list of all blogs
        Task<ResponseType<List<BlogResponseDto>>> GetBlogs();
        Task<ResponseType<List<BlogResponseDto>>> Ordering_byDate();

        // Creates a new blog with the provided request data
        Task<ResponseType<BlogResponseDto>> CreateBlog(CreateBlogDto request);

        // Searches blogs based on the provided search string
        Task<ResponseType<List<BlogResponseDto>>> SearchBlogs(string search);

        // Updates an existing blog by id with the provided request data
        Task<ResponseType<BlogResponseDto>> UpdateBlog(UpdateBlogDto request);

        // Deletes a blog by id
        Task<ResponseType<BlogResponseDto>> DeleteBlog(int id);
    }

    public class ResponseType<T>
    {
        public bool isSuccess { get; set; }
        public string Message { get; set; }
        public  List<string> Errors { get; set; }
        public T Data { get; set; }
    }
}
