using MishMashWebApp.ViewModels;
using SIS.HTTP.Responses.Contracts;
using SIS.MvcFramework.Attributes;

namespace MishMashWebApp.Controllers
{
    public class HomeController : BaseController
    {
        [HttpGet("/Home/Index")]
        public IHttpResponse Index(HomeViewModel model)
        {
            return View("/Home/Index", model);
        }

        [HttpGet("/")]
        public IHttpResponse RootIndex(HomeViewModel model)
        {
            return View("/Home/Index", model);
        }
    }
}
