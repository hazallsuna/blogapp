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

        //postları listeliyoruz
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
        //belirli bir postun detayını gösteriyoruz
        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var response = await _httpClient.GetAsync($"Posts/{id}");

            if (!response.IsSuccessStatusCode)
                return NotFound();

            var jsonData = await response.Content.ReadAsStringAsync();
            var post = JsonConvert.DeserializeObject<PostViewModel>(jsonData);

            var commentResponse = await _httpClient.GetAsync($"Comments/{id}");
            if (commentResponse.IsSuccessStatusCode)
            {
                var commentJson = await commentResponse.Content.ReadAsStringAsync();
                var comments = JsonConvert.DeserializeObject <List<CommentViewModel>> (commentJson);
                post.Comments = comments!;
            }

            return View(post);
        }

        [HttpGet]
        public IActionResult AddComment(int postId)
        {
            var model = new CommentViewModel { PostId = postId };
            return View(model);
        }
        //yorum ekle
       
        [HttpPost]
        public async Task<IActionResult> AddComment(CommentViewModel model)
        {
            if(!ModelState.IsValid)
            {
                return RedirectToAction("Details", new { id = model.PostId });

            }

            var jsonContent=  new StringContent(JsonConvert.SerializeObject(model),Encoding.UTF8,"application/json");

            var response = await _httpClient.PostAsync("Comments",jsonContent);

            if (!response.IsSuccessStatusCode)
            {
                TempData["Error"] = "Comment could not be added";
            }

            return RedirectToAction("Details", new {id = model.PostId});
        }

        //yeni post oluşturma formunu gösteriyoruz
       
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

        //yeni postu oluşturuyoruz
       
        [HttpPost]
        public async Task<IActionResult> Create(PostCreateViewModel model)
        {
            if (!ModelState.IsValid)
            {
                await LoadCategories(model); //kategori
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

        //postu güncelleme formunu getirdi
        
        [HttpGet]
        public async Task <IActionResult> Edit(int id)
        {
            var response = await _httpClient.GetAsync($"Posts/{id}");
            if (!response.IsSuccessStatusCode)
                return NotFound();

            var json = await response.Content.ReadAsStringAsync();
            var postData = JsonConvert.DeserializeObject<PostCreateViewModel>(json);

            await LoadCategories(postData); // kategori
            return View(postData);
        }

        //postu güncelledi
      
        [HttpPost]
        public async Task<IActionResult> Edit(PostCreateViewModel model)
        {
            if (!ModelState.IsValid)
            {
                await LoadCategories(model);
                return View(model);
            }

            var jsonContent = new StringContent(
                JsonConvert.SerializeObject(model),
                Encoding.UTF8,
                "application/json");

            var putResponse = await _httpClient.PutAsync($"Posts/{model.PostId}", jsonContent);

            if (putResponse.IsSuccessStatusCode)
            {
                return RedirectToAction("Index");
            }

            ModelState.AddModelError("", "Post could not be updated.");
            return View(model);
        }

        //seçilen postu siler
      
        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var response = await _httpClient.DeleteAsync($"Posts/{id}");
            if (response.IsSuccessStatusCode) {
                return RedirectToAction("Index");
            }
            TempData["Error"] = "Post could not be deleted.";
            return RedirectToAction("Index");
        }

        private async Task LoadCategories(PostCreateViewModel model)
        {
            var response = await _httpClient.GetAsync("Category");
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                model.Categories = JsonConvert.DeserializeObject<List<CategoryViewModel>>(json);
            }
        }

    }
}