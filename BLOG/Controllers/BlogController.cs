using BLOG.Data;
using BLOG.Models.Dto;
using BLOG.Services;
using BLOG.Services.IServices;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BLOG.Controllers
{
    [ApiController]
    [Route("blog")] // Add a base route for the controller
    public class BlogController : ControllerBase
    {
        private readonly IBlogService _blogService;
        private readonly ILogger<BlogController> _logger;
        public BlogController(IBlogService service, ILogger<BlogController> logger)
        {
            _blogService = service;
            _logger = logger;
        }
        #region Blogs
        [HttpGet("search/blogs/{word}")]
        public async Task<IActionResult> SearchKeyWords(string word)
        {
            var result = await _blogService.SearchBlogs(word);
            _logger.LogInformation(word, result);
            return Ok(result);
        }
        [HttpGet("orderDate/blogs")]
        public async Task<IActionResult> OrderDate()
        {
            var result = await _blogService.Ordering_byDate();
            _logger.LogInformation("Successfully Retrieve Blog in Date Order"); 
            return Ok(result);
        }
        [HttpGet("list/blogs")]
        public async Task<IActionResult> GetAllBlog()
        {
            var result = await _blogService.GetBlogs();
            _logger.LogInformation($"Successfully Retrieve Blog in Date Order, Result: {result}"); 
            return Ok(result);
        }
        [HttpPost("create/blogs")]
        public async Task<IActionResult> CreateBlog(CreateBlogDto request)
        {
            var result = await _blogService.CreateBlog(request);
            if (result == null)
            {
                _logger.LogWarning("Create Request Failed, Request is Null");
                return BadRequest(result);
            }
            _logger.LogInformation("Successfully Created Blogs");
            return Ok(result);
        }
        [HttpPatch("update/blog")]
        public async Task<IActionResult> UpdateBlog(UpdateBlogDto request)
        {
            var result = await _blogService.UpdateBlog(request);
            if(result == null)
            {
                _logger.LogWarning("Updated Request Failed, Request is Null");
                return BadRequest(result);
            }
                
            _logger.LogInformation("Successfully Updated Blogs");
            return Ok(result);
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBlog(int id)
        {
            var result = await _blogService.DeleteBlog(id);
            if( result == null)
            {
                _logger.LogWarning("Delete Request Failed, Id is Null");
                return NotFound(result);
            }
            _logger.LogInformation("Successfully Deleted Blogs");   
            return Ok(result);
        }
        #endregion
    }


}
