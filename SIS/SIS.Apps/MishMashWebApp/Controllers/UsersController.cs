using MishMashWebApp.InputModels.Users;
using MishMashWebApp.Services.Contracts;
using MishMashWebApp.ViewModels;
using SIS.HTTP.Common;
using SIS.HTTP.Cookies;
using SIS.HTTP.Responses.Contracts;
using SIS.MvcFramework.Attributes;
using SIS.MvcFramework.ViewModels;

namespace MishMashWebApp.Controllers
{
    public class UsersController : BaseController
    {
        private readonly IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
            Model = new HomeViewModel();
        }

        public HomeViewModel Model { get; set; }

        [HttpGet("/Users/Login")]
        public IHttpResponse Login(HomeViewModel model)
        {
            return View("Users/Login", model);
        }

        [HttpPost("/Users/Login")]
        public IHttpResponse DoLogin(LoginInputModel model)
        {
            bool isLogged = _userService.UserLogin(model, Db);

            if (!isLogged)
            {
                var errorMessage = "Invalid username or password.";
                return View("error", new ErrorViewModel(errorMessage));
            }

            var cookieContent = UserCookieService.GetUserCookie(model.Username);

            Request.Session.AddParameter(".auth_cake", cookieContent);

            Response.Cookies.Add(new HttpCookie(".auth_cake", $"{cookieContent}; {GlobalConstants.HttpOnly}", 7));

            return View("/Home/Index", Model);

        }


        [HttpGet("/Users/Register")]
        public IHttpResponse Register(HomeViewModel model)
        {
            return View("Users/Register", model);
        }

        [HttpPost("/Users/Register")]
        public IHttpResponse DoRegister(RegisterInputModel model)
        {
            string errorMessage = string.Empty;

            if (string.IsNullOrWhiteSpace(model.Username) || model.Username.Length < 4)
            {
                errorMessage = "Please, provide valid username with length 4 or more symbols";
            }

            if (string.IsNullOrWhiteSpace(model.Password) || model.Password.Length < 6)
            {
                errorMessage = "Please, provide valid password with length 6 or more symbols";
            }

            if (!model.Password.Equals(model.ConfirmPassword))
            {
                errorMessage = "Passwords do not match.";
            }

            bool isCreated = _userService.Create(model, Db);

            if (!isCreated)
            {
                errorMessage = $"User with username: {model.Username} already exists.";
            }

            if (!errorMessage.Equals(string.Empty))
            {
                return View("/error", new ErrorViewModel(errorMessage));
            }

            return View("/Users/Registered", Model);
        }


        [HttpGet("/Users/Logout")]
        public IHttpResponse Logout()
        {
            if (User != null)
            {
                var cookie = Request.Cookies.GetCookie(".auth_cake");

                cookie.Delete();

                Response.Cookies.Add(cookie);

                Request.Session.ClearParameters();
            }

            if (Request.Session.ContainsParameter(".auth_cake"))
            {
                Request.Session.ClearParameters();

            }

            return View("/Home/Index", Model);
        }
    }
}
