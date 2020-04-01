using DistributedToolsServer.Domain;
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
            return View(currentUserAccessor.GetCurrentUser());
        }
    }
}
