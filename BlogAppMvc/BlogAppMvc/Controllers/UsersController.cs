using BlogAppMvc.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Text;

namespace BlogAppMvc.Controllers
{
    public class UsersController : Controller
    {
        private readonly HttpClient _httpClient;

        public UsersController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("api");
        }

        [HttpGet]
        public IActionResult Login(string? returnUrl=null)
        {
            if (User.Identity != null && User.Identity!.IsAuthenticated)
            {
                return RedirectToAction
                ("Index", "Posts");
            }

            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model , string? returnUrl =null)
        {
            if(!ModelState.IsValid)
                return View(model);

            var content = new StringContent(JsonConvert.SerializeObject(model),Encoding.UTF8,"application/json");
            var response=await _httpClient.PostAsync("Users/Login",content);

            if (response.IsSuccessStatusCode)
            {
                if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                {
                    return Redirect(returnUrl);
                }

                return RedirectToAction("Index", "Posts");
            }
            ModelState.AddModelError("", "Invalid email or password.");
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await _httpClient.PostAsync("Users/Logout",null);
            await HttpContext.SignOutAsync();
            return RedirectToAction("Login");
        }
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var jsonContent = new StringContent(
                JsonConvert.SerializeObject(model),
                Encoding.UTF8,
                "application/json"
            );

            var response = await _httpClient.PostAsync("Users/Registration", jsonContent);

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("Login");
            }

            ModelState.AddModelError("", "Registration failed.");
            return View(model);
        }
    }
}
