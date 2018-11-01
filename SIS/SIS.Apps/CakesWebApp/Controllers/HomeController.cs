using CakesWebApp.ViewModels.Home;
using SIS.HTTP.Responses.Contracts;
using SIS.MvcFramework.Attributes;

namespace CakesWebApp.Controllers
{
    public class HomeController : BaseController
    {
        [HttpGet("/")]
        public IHttpResponse Index(HomeViewModel model)
        {
            return View("/", model);
        }

        [HttpGet("/Home/About")]
        public IHttpResponse About(HomeViewModel model)
        {
            model.Title = "About Us";
            
            return View("/Home/About", model);
        }
    }
}
