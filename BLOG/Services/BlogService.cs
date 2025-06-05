using BLOG.Data;
using BLOG.Models;
using BLOG.Models.Dto;
using BLOG.Services.Helpers;
using BLOG.Services.IServices;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using System;

namespace BLOG.Services
{
    public class BlogService : IBlogService
    {
        private readonly ILogger<BlogService> _logger;
        private readonly Data.BloggingContext _dbContext;
        public BlogService(Data.BloggingContext dbContext,ILogger<BlogService> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        #region Blog
        public async Task<ResponseType<List<BlogResponseDto>>> SearchBlogs(string search)
        {
            IQueryable<Blog> query = _dbContext.Blogs;
            string lowerSearch = search?.ToLower().Trim() ?? "Empty";

            if (!string.IsNullOrWhiteSpace(search))
            {
                query = _dbContext.Blogs
                   .Where(blog => 
                   blog.BlogTitle.ToLower().Contains(lowerSearch) ||
                   blog.Author.ToLower().Contains(lowerSearch) ||
                   blog.Post.Any(post => 
                   post.Title.ToLower().Contains(lowerSearch) ||
                   post.Content.ToLower().Contains(lowerSearch)
                ));
            }

            var blogResponseDtos = await query
            .Select(blog => new BlogResponseDto(blog.BlogId, blog.BlogTitle, blog.Author, blog.CreatedAt, blog.Post
            .Select(post => new PostResponseDto(post.PostId, post.Title, post.Content, post.CreatedAt)).ToList()
            )).ToListAsync(); // Execute the fully composed query against the database

            if (!string.IsNullOrWhiteSpace(lowerSearch))
            {
                if (blogResponseDtos.Any())
                {
                    return new ResponseType<List<BlogResponseDto>>()
                    {
                        isSuccess = true,
                        Data = blogResponseDtos,
                        Message = $"{blogResponseDtos.Count} blogs found matching '{search}'."
                    };
                }
                else
                {
                    // Search performed, but no results found.
                    return new ResponseType<List<BlogResponseDto>>()
                    {
                        isSuccess = true, // The search operation itself was successful, just no matches
                        Data = new List<BlogResponseDto>(), // Return an empty list
                        Message = "No blogs found matching the search criteria."
                    };
                }
            }
            else
            {
                // No search term was provided (empty or whitespace search).
                // In this case, the 'query' variable would not have had a .Where() filter applied,
                // so 'blogResponseDtos' will contain all blogs.
                return new ResponseType<List<BlogResponseDto>>()
                {
                    isSuccess = true,
                    Data = blogResponseDtos,
                    Message = "All blogs retrieved successfully (no search term provided)."
                };
            }
        }
        public async Task<ResponseType<List<BlogResponseDto>>> GetBlogs()
        {
            ResponseType<List<BlogResponseDto>> response = new();
            if(await _dbContext.Blogs.CountAsync() <= 0)
            {
                _logger.LogInformation($"Current Post {_dbContext.Blogs.Count()}");
                response.isSuccess = false;
                response.Message = "No Blogs Found Found, Create One First";
                return response;
            }
            try
            {            
                var blogAsQuery = _dbContext.Blogs
                    .Select(blog => new BlogResponseDto(blog.BlogId, blog.BlogTitle, blog.Author,blog.CreatedAt, blog.Post
                    .Select(post => new PostResponseDto(post.PostId, post.Title, post.Content, blog.CreatedAt))
                    .ToList()));

                var query = await blogAsQuery.ToListAsync();
                _logger.LogInformation($"Successfully Display All Blogs ");
                response.isSuccess = true;
                response.Data = query;
                response.Message = "Successfully Display all Blogs";
                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError($"{ex} Cannot Get All Blogs");
                response.isSuccess = false;
                response.Message = ex.Message;
                return response;
            }
        }

        public async Task<ResponseType<BlogResponseDto>> CreateBlog(CreateBlogDto request)
        {
            ResponseType<BlogResponseDto> response = new();
            if(request == null)
            {
                _logger.LogWarning($"{request} Cannot be null");
                response.isSuccess = false;
                response.Message = "Create Blog Cannot be Null";
                return response;
            }
            try
            {   var modelValidation = ModelValidation.ModelValidationResponse(request);
                if (modelValidation.Any())
                {
                    _logger.LogWarning($"{modelValidation} Failed, Insert Correct Value ");
                    response.isSuccess = false;
                    response.Message = "There are Model Validation Occurred";
                    return response;
                }

                var blog = new Blog
                {
                    Author = request.Author,
                    BlogTitle = request.BlogTitle,
                    CreatedAt = DateTime.Now,
                    Post = new List<Post>() // leave empty for adding later
                };

                await _dbContext.AddAsync(blog);

                await  _dbContext.SaveChangesAsync();

                _logger.LogInformation($"Successfully  Inserted the {request} Information");
                response.isSuccess = true;
                response.Message = "Successfully Created Blog";
                response.Data = new BlogResponseDto(blog.BlogId,blog.BlogTitle, blog.Author,blog.CreatedAt,new List<PostResponseDto>());
                return response;
            }
            catch(Exception ex)
            {
                _logger.LogError($"Error while Adding Blog");
                response.isSuccess = false;
                response.Message = ex.Message;
                return response;
            }
        }
        public async Task<ResponseType<BlogResponseDto>> UpdateBlog(UpdateBlogDto request)
        {
            ResponseType<BlogResponseDto> response = new();
            if (request == null)
            {
                _logger.LogWarning($"{request} Cannot be null");
                response.isSuccess = false;
                response.Message = "Update Blog request cannot be null";
                return response;
            }
            var modelValidation = ModelValidation.ModelValidationResponse(request);
            if (modelValidation.Any())
            {
                _logger.LogWarning($"{modelValidation} Failed, Insert Correct Value ");
                response.isSuccess = false;
                response.Message = "Validation Failed";
                return response;
            }
            var blog = await _dbContext.Blogs.FirstOrDefaultAsync(b => b.BlogId == request.BlogId);
            if (blog == null)
            {
                _logger.LogWarning($"{blog} Id Cannot Found");
                response.isSuccess = false;
                response.Message = "Blog not found";
                return response;
            }
            try
            {
                blog.Author = request.Author ?? blog.Author;
                blog.BlogTitle = request.BlogTitle ?? blog.BlogTitle;
                blog.Post = new List<Post>();
                
                await _dbContext.SaveChangesAsync();
                _logger.LogInformation("Successfully Updating your Blog");
                response.isSuccess = true;
                response.Message = "Blog updated successfully";
                response.Data = new BlogResponseDto(blog.BlogId,blog.BlogTitle,blog.Author,blog.CreatedAt,new List<PostResponseDto>());
                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError($"{ex} Error Updating your Post");
                response.isSuccess = false;
                response.Message = ex.Message;
                return response;
            }
        }

        public async Task<ResponseType<BlogResponseDto>> DeleteBlog(int id)
        {
            ResponseType<BlogResponseDto> response = new();
            var blogId = await _dbContext.Blogs.Include(b => b.Post).FirstOrDefaultAsync(b => b.BlogId == id);
            if (blogId == null)
            {
                _logger.LogWarning($"{blogId} Cannot be null");
                response.isSuccess = false;
                response.Message = "Blog not found";
                return response;
            }
            try
            {
                _dbContext.Blogs.Remove(blogId);
                await _dbContext.SaveChangesAsync();
                _logger.LogInformation($"Successfully  Deleted {id}");
                response.isSuccess = true;
                response.Message = "Blog and its posts deleted successfully";
                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError($"{ex} Error Deleting Blog with Id {id}");
                response.isSuccess = false;
                response.Message = ex.Message;
                return response;
            }
        }

        public async Task<ResponseType<List<BlogResponseDto>>> Ordering_byDate()
        {
            if(await _dbContext.Blogs.CountAsync() <= 0)
            {
                _logger.LogInformation($"Current Blog {await _dbContext.Blogs.CountAsync()}");
                return new ResponseType<List<BlogResponseDto>>()
                {
                    isSuccess = false,
                    Message = "No Post Found, Create One First"
                };
            }
            var orderedByAsQuery = _dbContext.Blogs
                .OrderByDescending(post => post.CreatedAt)
                .Select(blog => new BlogResponseDto(blog.BlogId, blog.BlogTitle, blog.Author, blog.CreatedAt, blog.Post
                .Select(post => new PostResponseDto(post.PostId, post.Title, post.Content, post.CreatedAt)).ToList()));

            var orderedPost = await orderedByAsQuery.ToListAsync();
            _logger.LogInformation("Fetching Post Data By Date Order");
            return new ResponseType<List<BlogResponseDto>>()
            {
                isSuccess = true,
                Data = orderedPost
            };
        }
        #endregion
    }


}
