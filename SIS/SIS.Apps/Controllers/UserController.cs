using System.Text;
using SIS.MvcFramework.ViewModels;

namespace CakesWebApp.Controllers
{
    using Services.Contracts;
    using SIS.MvcFramework.Attributes;
    using SIS.HTTP.Responses.Contracts;
    using ViewModels.User;
    using System.Linq;
    using CakesWebApp.ViewModels.Shopping;

    public class UserController : BaseController
    {
        private readonly IUserService _userService;
        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet("user/profile")]
        public IHttpResponse GetProfile()
        {
            if (User == null)
            {
                var errorMessage = "You must first login.";
                return View("/error", new ErrorViewModel(errorMessage));
            }

            var model = _userService.Profile(User, Db);

            return View("user/profile", model);
        }

        [HttpGet("/user/hello")]
        public IHttpResponse HelloUser(HelloViewModel model)
        {
            model.Username = User;
            return View("/user/hello", model);
        }

        
        [HttpGet("/user/myOrders")]
        public IHttpResponse MyOrders(MyOrdersViewModel model)
        {
            model.MyOrders = _userService.GetMyOrders(User, Db);

            return View("/user/myOrders", model);
        }
    }
}
