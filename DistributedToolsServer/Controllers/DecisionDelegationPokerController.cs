using Microsoft.AspNetCore.Mvc;

namespace DistributedToolsServer.Controllers
{
    public class DecisionDelegationPokerController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}