using System;
using System.Diagnostics;
using DistributedToolsServer.Domain;
using Microsoft.AspNetCore.Mvc;
using DistributedToolsServer.Models;

namespace DistributedToolsServer.Controllers
{
    public class HomeController : Controller
    {
        private readonly IUserRepository userRepository;
        private readonly ICurrentUserAccessor currentUserAccessor;

        public HomeController(IUserRepository userRepository, ICurrentUserAccessor currentUserAccessor)
        {
            this.userRepository = userRepository;
            this.currentUserAccessor = currentUserAccessor;
        }

        public IActionResult Index()
        {
            return View(currentUserAccessor.GetCurrentUser());
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [HttpPost("register")]
        public IActionResult Register([FromForm] UserRegistrationRequest request)
        {
            if (string.IsNullOrEmpty(request.Name))
            {
                TempData["Error"] = "User must have a name";
                if (request.RedirectTo == "/register")
                {
                    return RedirectToAction("Index");
                }

                return Redirect(request.RedirectTo);
            }
            var userId = userRepository.AddUser(request.Name);
            Response.Cookies.Append("el-dt-user-id", userId.ToString());
            return Redirect(GetSafeRedirect(request.RedirectTo));
        }

        private string GetSafeRedirect(string redirect)
        {
            if (Uri.TryCreate(redirect, UriKind.Relative, out var _))
            {
                return redirect;
            }

            return "/";
        }
    }
}
