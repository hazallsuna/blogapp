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
    public class PostsController : ControllerBase
    {
        public readonly BlogAppDbContext dbContext;
        public PostsController(BlogAppDbContext dbContext)
        {
            this.dbContext = dbContext;
        }
        // Tüm blog yazılarını getir
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var posts = await dbContext.Post
                .Include(p => p.User)
                .Include(p => p.Category)
                .ToListAsync();

            return Ok(posts);
        }

        // Belirli bir blog yazısını getir
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var post = await dbContext.Post
                .Include(p => p.User)
                .Include(p => p.Category)
                .Include(p => p.Comments)
                    .ThenInclude(c => c.User)
                .FirstOrDefaultAsync(p => p.PostId == id);

            if (post == null) return NotFound();
            return Ok(post);
        }

        // Yeni blog yazısı oluştur
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Create([FromBody] Post model)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            var post = new Post
            {
                Title = model.Title,
                Content = model.Content,
                Description = model.Description,
                Url = model.Url,
                Image = model.Image,
                CategoryId = model.CategoryId,
                UserId = userId,
                IsActive = false,
                PublishDate = DateTime.Now,
                UpdateDate = DateTime.Now
            };

            dbContext.Post.Add(post);
            await dbContext.SaveChangesAsync();

            return Ok(post);
        }

        // Blog yazısını güncelle
        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> Update(int id, [FromBody] Post model)
        {
            var post = await dbContext.Post.FindAsync(id);
            if (post == null) return NotFound();

            post.Title = model.Title;
            post.Content = model.Content;
            post.Description = model.Description;
            post.Url = model.Url;
            post.Image = model.Image;
            post.CategoryId = model.CategoryId;
            post.UpdateDate = DateTime.Now;

            await dbContext.SaveChangesAsync();
            return Ok(post);
        }

        // Blog yazısını sil
        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> Delete(int id)
        {
            var post = await dbContext.Post.FindAsync(id);
            if (post == null) return NotFound();

            dbContext.Post.Remove(post);
            await dbContext.SaveChangesAsync();

            return Ok("Deleted");
        }
    }
}
