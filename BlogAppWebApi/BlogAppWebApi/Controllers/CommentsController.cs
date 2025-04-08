using BlogAppWebApi.Data;
using BlogAppWebApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace BlogAppWebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CommentsController : ControllerBase
    {
        private readonly BlogAppDbContext dbContext;
        public CommentsController(BlogAppDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        //yorumları listele
        [HttpGet("{postId}")]
        public async Task<IActionResult> GetCommentsByPost(int postId)
        {
            var comments = await dbContext.Comment
                .Include(c=>c.User)
                .Where(c=>c.PostId == postId)
                .OrderByDescending(c=>c.PublishedDate)
                .ToListAsync();
            return Ok(comments);
        }

        //yorum ekle
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> AddComment([FromBody] Comment model)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            var comment = new Comment
            {
                Text = model.Text,
                PublishedDate = DateTime.Now,
                PostId = model.PostId,
                UserId = userId
            };

            dbContext.Comment.Add(comment);
            await dbContext.SaveChangesAsync();

            return Ok(comment);
        }
    }
}
