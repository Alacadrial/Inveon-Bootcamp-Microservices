using Inveon.Web.Hubs;
using Inveon.Web.Models;
using Inveon.Web.Services.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Security.Claims;

namespace Inveon.Web.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IProductService _productService;
        private readonly IHubContext<LoginHub> _loginHub;

        public HomeController(ILogger<HomeController> logger, IProductService productService, IHubContext<LoginHub> loginHub)
        {
            _logger = logger;
            _productService = productService;
            _loginHub = loginHub;
        }

        public async Task<IActionResult> Index()
        {
            List<ProductDto> list = new();
            var response = await _productService.GetAllProductsAsync<ResponseDto>("");
            if (response != null && response.IsSuccess)
            {
                list = JsonConvert.DeserializeObject<List<ProductDto>>(Convert.ToString(response.Result));
            }
            return View(list);

        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

		[Authorize]
		public async Task<IActionResult> Login()
		{
			var role = User.Claims.Where(u => u.Type == "role")?.FirstOrDefault()?.Value;
            var name = User.Claims.Where(u => u.Type == "given_name")?.FirstOrDefault()?.Value;
            var loginInfo = new { name = name, role = role, time = DateTime.Now.ToString("HH:mm:ss"), status = true };

            // Burada Huba Bağlan ve Info pushla
            await _loginHub.Clients.All.SendAsync("lastLogin", loginInfo);

            if (role == "Admin")
			{
				// return Redirect("~/Admin/Yonetici");
				return RedirectToAction("Git", "Yonetici", new { area = "Admin" });
			}
			//buradan IdentityServer daki login sayfasına gidiliyor.

			return RedirectToAction(nameof(Index));
		}

        [Authorize]
        public IActionResult Logout()
        {
            // Burada Huba Bağlan ve Info pushla
            var role = User.Claims.Where(u => u.Type == "role")?.FirstOrDefault()?.Value;
            var name = User.Claims.Where(u => u.Type == "given_name")?.FirstOrDefault()?.Value;
            var loginInfo = new { name = name, role = role, time = DateTime.Now.ToString("HH:mm:ss"), status = false };

            _loginHub.Clients.All.SendAsync("lastLogin", loginInfo);
            return SignOut("Cookies", "oidc");
		}
	}
}