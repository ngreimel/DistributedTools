using DistributedToolsServer.Domain;
using DistributedToolsServer.Models;
using Microsoft.AspNetCore.Mvc;

namespace DistributedToolsServer.Controllers
{
    public class CurrentUserViewComponent : ViewComponent
    {
        private readonly ICurrentUserAccessor currentUserAccessor;

        public CurrentUserViewComponent(ICurrentUserAccessor currentUserAccessor)
        {
            this.currentUserAccessor = currentUserAccessor;
        }

        public IViewComponentResult Invoke()
        {
            return View(new UserAndPath
            {
                Path = Request.Path,
                User = currentUserAccessor.GetCurrentUser()
            });
        }
    }
}
