namespace CakesWebApp.Controllers
{
    using SIS.HTTP.Responses.Contracts;
    using SIS.MvcFramework.Attributes;
    using ViewModels.Home;
    using ViewModels.Shopping;

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
