using Azure;
using Azure.Core;
using BLOG.Data;
using BLOG.Models;
using BLOG.Models.Dto;
using BLOG.Services.Helpers;
using BLOG.Services.IServices;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Metadata;

namespace BLOG.Services
{
    public class PostService : IPostService
    {
        private readonly BloggingContext _dbContext;
        private readonly ILogger<PostService> _logger;
        public  PostService(BloggingContext postService, ILogger<PostService> logger)
        {
            _dbContext = postService;
            _logger = logger;
        }
        public async Task<ResponseType<List<PostResponseDto>>> AscendingPost()
        {
            if(await _dbContext.Posts.CountAsync() <= 0)
            {
                _logger.LogInformation($"Current Post {await _dbContext.Posts.CountAsync()}");
                return new ResponseType<List<PostResponseDto>>()
                {
                    isSuccess = false,
                    Message = "No Post Found, Create One First"
                };
                
            }
            var orderedByAsQuery = _dbContext.Posts
                .OrderBy(post => post.Title)
                .Select(post => new PostResponseDto(post.PostId, post.Title,post.Content, post.CreatedAt));

            var orderedPost = await orderedByAsQuery.ToListAsync();
            _logger.LogInformation("Fetching Post Data in Ascending Order");
            return new ResponseType<List<PostResponseDto>>()
            {
                isSuccess = true,
                Data = orderedPost
            };
        }

        public async Task<ResponseType<PostResponseDto>> CreatePost(CreatePostDto request)
        {
            ResponseType<PostResponseDto> response = new ResponseType<PostResponseDto>();
            if(request == null)
            {
                _logger.LogWarning($"{request} Cannot be null");
                response.isSuccess = false;
                response.Message = "Request Cannot be null";
                return response;
            }
            var modelValidation = ModelValidation.ModelValidationResponse(request);
            if (modelValidation.Any())
            {
                _logger.LogWarning($"Model Validation in {request} Failed");
                response.isSuccess = false;
                response.Message = "Model Validation";
                response.Errors.AddRange(modelValidation);
                return response;
            }
            var blogExist = await _dbContext.Posts.AnyAsync(id => id.PostId != request.BlogId);
            if (blogExist)
            {
                _logger.LogWarning($"Blog {request.BlogId} is Required");
                response.isSuccess = false;
                response.Message = "Blog Cannot Found";
                return response;
            }
            try
            {
                var post = new Post()
                {
                    BlogId = request.BlogId,
                    Title = request.Title,
                    Content = request.Title,
                    CreatedAt = DateTime.Now,
                };

                await _dbContext.AddAsync(post);
                await _dbContext.SaveChangesAsync();

                _logger.LogInformation($"Successfully Adding the Post from {request}");
                response.isSuccess = true;
                response.Message = "Successfully Created Blog";
                response.Data = new PostResponseDto(post.PostId,post.Title,post.Content, post.CreatedAt);
                return response;
            }
            catch(Exception ex)
            {
                _logger.LogError($"Error while creating Post with ID {request}");
                response.isSuccess = false;
                response.Message = ex.Message;
                return response;
            }
        }

        public async Task<ResponseType<PostResponseDto>> DeletePost(int id)
        {
            ResponseType<PostResponseDto> response = new();
            if(id <= 0)
            {
                _logger.LogWarning($"{id} Cannot be less than or equal zero");
                response.isSuccess = false;
                response.Message = "Post Cannot Found";
                return response;
            }
            var getId = _dbContext.Posts.FirstOrDefaultAsync(postId => postId.PostId == id);
            if(getId == null)
            {
                _logger.LogWarning($"{getId} Cannot be null");
                response.isSuccess =false;
                response.Message = "Id Cannot be Null";
                return response;
            }
            try
            {
                _dbContext.Remove(getId);
                await _dbContext.SaveChangesAsync();
                _logger.LogInformation($"Successfully  Deleted {id}");
                response.isSuccess = true;
                response.Message = "Successfully deleted Post";
                return response;
            }
            catch(Exception ex)
            {
                _logger.LogError($"{ex} Error Deleting Blog with Id {id}");
                response.isSuccess = false;
                response.Message = ex.Message;
                return response;
            }
        }

        public async Task<ResponseType<List<PostResponseDto>>> Descending()
        {
            if(await _dbContext.Posts.CountAsync() <= 0)
            {
                _logger.LogInformation($"Current Post {await _dbContext.Posts.CountAsync()}");
                return new ResponseType<List<PostResponseDto>>()
                {
                    isSuccess = false,
                    Message = "No Post Found, Create One First"
                };
            }
            var orderedByAsQuery = _dbContext.Posts
                .OrderByDescending(post => post.Title)
                .Select(post => new PostResponseDto(post.PostId, post.Title,post.Content, post.CreatedAt));

            var orderedPost = await orderedByAsQuery.ToListAsync();
            _logger.LogInformation("Fetching Post Data in Descending Order");
            return new ResponseType<List<PostResponseDto>>()
            {
                isSuccess = true,
                Data = orderedPost
            };
        }

        public async Task<ResponseType<List<PostResponseDto>>> Ordering_byDate()
        {
            if(await _dbContext.Posts.CountAsync() <= 0)
            {
                _logger.LogInformation($"Current Post {_dbContext.Posts.Count()}");
                return new ResponseType<List<PostResponseDto>>()
                {
                    isSuccess = false,
                    Message = "No Post Found, Create One First"
                };
            }
            var orderedByAsQuery = _dbContext.Posts
                .OrderByDescending(post => post.CreatedAt)
                .Select(post => new PostResponseDto(post.PostId, post.Title,post.Content, post.CreatedAt));

            var orderedPost = await orderedByAsQuery.ToListAsync();
            _logger.LogInformation("Fetching Post Data in Ascending Order");
            return new ResponseType<List<PostResponseDto>>()
            {
                isSuccess = true,
                Data = orderedPost
            };
        }

        public async Task<ResponseType<PostResponseDto>> UpdatePost(UpdatePostDto request)
        {
            ResponseType<PostResponseDto> response = new();
            if (request == null)
            {
                _logger.LogWarning($"{request} Cannot be null");
                response.isSuccess = false;
                response.Message = "Post request cannot be null";
                return response;
            }
            var modelValidation = ModelValidation.ModelValidationResponse(request);
            if (modelValidation.Any())
            {
                _logger.LogWarning($"{modelValidation} is Invalid, Enter Correct Information");
                response.isSuccess = false;
                response.Message = "Validation Failed";
                return response;
            }
            var post = await _dbContext.Posts.FirstOrDefaultAsync(x => x.PostId == request.PostId);
            if (post == null)
            {
                _logger.LogWarning($"{post} Id Cannot Found");
                response.isSuccess = false;
                response.Message = "Post not found";
                return response;
            }
            try
            {
                post.Title = request.Title ?? post.Title;
                post.Content = request.Content ?? post.Content;
                
                await _dbContext.AddAsync(post);
                await _dbContext.SaveChangesAsync();

                _logger.LogInformation("Successfully Updating your Post");
                response.isSuccess = true;
                response.Message = "Blog updated successfully";
                response.Data = new PostResponseDto(post.PostId,post.Title,post.Content, post.CreatedAt);
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
    }
}
