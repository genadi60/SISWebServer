using System.Linq;
using MishMashWebApp.Models.Enums;
using SIS.MvcFramework;
using SIS.MvcFramework.ViewModel;

namespace MishMashWebApp.Controllers
{
    using InputModels.Users;
    using Services.Contracts;
    using SIS.HTTP.Common;
    using SIS.HTTP.Cookies;
    using SIS.HTTP.Responses.Contracts;
    using SIS.MvcFramework.Attributes;
    using ViewModels.Home;

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
        public IHttpResponse Login(LoginInputModel model)
        {
            return View("Users/Login", model);
        }

        [HttpPost("/Users/Login")]
        public IHttpResponse DoLogin(LoginInputModel model)
        {
            if (!_userService.UserIsAuthenticated(model, Db))
            {
                var errorMessage = "Invalid username or password.";
                return View("error", new ErrorViewModel(errorMessage));
            }


            var user = Db.Users.FirstOrDefault(u => u.Username.Equals(model.Username));

            var mvcUser = new MvcUserInfo { Username = user.Username, Role = user.Role.ToString(), Info = user.Email };

            var cookieContent = UserCookieService.GetUserCookie(mvcUser);

            Request.Session.AddParameter(".auth_cake", cookieContent);

            Response.Cookies.Add(new HttpCookie(".auth_cake", $"{cookieContent}; {GlobalConstants.HttpOnly}", 7));


            return View("/Home/Index");

        }


        [HttpGet("/Users/Register")]
        public IHttpResponse Register(RegisterInputModel model)
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

            if (!Db.Users.Any())
            {
                model.Role = Role.Admin;
            }

            bool isRegistered = _userService.Create(model, Db);

            if (!isRegistered)
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
            Request.Session.ClearParameters();

            if (User != null)
            {
                var cookie = Request.Cookies.GetCookie(".auth_cake");

                cookie.Delete();

                Response.Cookies.Add(cookie);
            }

            return Redirect("/");
        }
    }
}
