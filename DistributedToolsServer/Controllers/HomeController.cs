using System.Diagnostics;
using DistributedToolsServer.Domain;
using Microsoft.AspNetCore.Mvc;
using DistributedToolsServer.Models;

namespace DistributedToolsServer.Controllers
{
    public class HomeController : Controller
    {
        private readonly IUserRepository userRepository;

        public HomeController(IUserRepository userRepository)
        {
            this.userRepository = userRepository;
        }

        public IActionResult Index()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [HttpPost("register")]
        public IActionResult Register([FromForm] UserRegistrationRequest request)
        {
            var userId = userRepository.AddUser(request.Name);
            Response.Cookies.Append("el-dt-user-id", userId.ToString());
            return Redirect("/");
        }
    }
}
