using BLOG.Models.Dto;
using BLOG.Services;
using BLOG.Services.IServices;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BLOG.Controllers
{
    
    [ApiController]
    [Route("Post")]
    public class PostController : ControllerBase
    {
        private readonly IPostService _postService;
        private readonly ILogger<PostController> _logger;
        public PostController(IPostService postService, ILogger<PostController> logger)
        {
            _postService = postService;
            _logger = logger;
        }

        [HttpGet]
        [Route("Ascending/post")]
        public async Task<IActionResult> Ascending()
        {
            var ascending = await _postService.AscendingPost();
            return Ok(ascending);
        }
        [HttpGet]
        [Route("Descending/post")]
        public async Task<IActionResult> Descending()
        {
            var ascending = await _postService.Descending();
            return Ok(ascending);
        }
        [HttpGet("orderDate/post")]
        public async Task<IActionResult> OrderDate()
        {
            var result = await _postService.Ordering_byDate();
            return Ok(result);
        }
        [HttpPost]
        [Route("create/post")]
        public async Task<IActionResult> CreatePost(CreatePostDto request)
        {
            var create = await _postService.CreatePost(request);
            if (request == null)
                return BadRequest(create);
            return Ok(create);
        }
        [HttpPatch]
        [Route("update/post")]
        public async Task<IActionResult>UpdatePost(UpdatePostDto request)
        {
            var update = await _postService.UpdatePost(request);
            if(update == null)
                return NotFound(update);
            return Ok(update);
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePost(int id)
        {
            var delete = await _postService.DeletePost(id);
            if(id <= 0)
                return NotFound(delete);
            return Ok(delete);
        }
    }
}
