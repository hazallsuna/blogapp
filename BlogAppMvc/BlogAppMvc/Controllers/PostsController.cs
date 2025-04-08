using BlogAppMvc.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Text;

namespace BlogAppMvc.Controllers
{
    public class PostsController : Controller
    {
        private readonly HttpClient _httpClient;

        public PostsController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("api");
        }


        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var response = await _httpClient.GetAsync("Posts");

            if (response.IsSuccessStatusCode)
            {
                var jsonData = await response.Content.ReadAsStringAsync();
                var posts = JsonConvert.DeserializeObject<List<PostViewModel>>(jsonData);
                return View(posts);
            }

            return View(new List<PostViewModel>());
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var response = await _httpClient.GetAsync($"Posts/{id}");

            if (!response.IsSuccessStatusCode)
                return NotFound();

            var jsonData = await response.Content.ReadAsStringAsync();
            var post = JsonConvert.DeserializeObject<PostViewModel>(jsonData);
            return View(post);
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var response = await _httpClient.GetAsync("Category");
            var model = new PostCreateViewModel();

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                model.Categories = JsonConvert.DeserializeObject<List<CategoryViewModel>>(json);
            }

            return View(model);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Create(PostCreateViewModel model)
        {
            if (!ModelState.IsValid)
            {
                // Kategorileri tekrar yükle
                var response = await _httpClient.GetAsync("Category");
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    model.Categories = JsonConvert.DeserializeObject<List<CategoryViewModel>>(json);
                }

                return View(model);
            }

            var jsonContent = new StringContent(
                JsonConvert.SerializeObject(model),
                Encoding.UTF8,
                "application/json");

            var postResponse = await _httpClient.PostAsync("Posts", jsonContent);

            if (postResponse.IsSuccessStatusCode)
            {
                return RedirectToAction("Index");
            }

            ModelState.AddModelError("", "Blog could not be created.");
            return View(model);
        }
    }
}
