using BlogAppWebApi.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BlogAppWebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly BlogAppDbContext dbContext;

        public CategoryController(BlogAppDbContext context)
        {
            this.dbContext = context;
        }

        //kategori listele
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var category = await dbContext.Category.ToListAsync();
            return Ok(category);
        }

        [HttpGet("/{categoryId}")]
        public async Task<IActionResult> GetByCategory(int categoryId)
        {
            var post = await dbContext.Post
                .Include(p => p.User)
                .Include(p => p.Category)
                .Where(p=>p.CategoryId == categoryId)
                .ToListAsync();
            return Ok(post);
        }
    }
}
