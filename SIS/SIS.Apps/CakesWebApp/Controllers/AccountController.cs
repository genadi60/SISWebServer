using CakesWebApp.InputModels.Account;
using CakesWebApp.Services.Contracts;
using CakesWebApp.ViewModels.Home;
using CakesWebApp.ViewModels.Shopping;
using SIS.HTTP.Common;
using SIS.HTTP.Cookies;
using SIS.HTTP.Responses.Contracts;
using SIS.MvcFramework.Attributes;
using SIS.MvcFramework.ViewModels;

namespace CakesWebApp.Controllers
{
    public class AccountController : BaseController
    {
        private readonly IAccountService _accountService;

        public AccountController(IAccountService accountService)
        {
            _accountService = accountService;
            Model = new HomeViewModel();
        }

        public HomeViewModel Model { get; set; }

        [HttpGet("/account/register")]
        public IHttpResponse Register()
        {
            if (User != null)
            {
                var errorMessage = "You are already registered.";
                return View("/error", new ErrorViewModel(errorMessage));
            }

            Model.Title = "Register";

            return View("/account/register", Model);
        }

        [HttpPost("/account/register")]
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

            bool isCreated = _accountService.Create(model, Db);

            if (!isCreated)
            {
                errorMessage = $"User with username: {model.Username} already exists.";
            }

            if (!errorMessage.Equals(string.Empty))
            {
                return View("/error", new ErrorViewModel(errorMessage));
            }

            return View("/account/registered", Model);
        }

        [HttpGet("/account/login")]
        public IHttpResponse Login()
        {
            if (User != null)
            {
                var message = "You are now logged!";
                var model = new ErrorViewModel(message);
                return View("/error", model);
            }

            Model.Title = "Login";

            return View("/account/login", Model);
        }

        [HttpPost("/account/login")]
        public IHttpResponse DoLogin(LoginInputModel model)
        {
            bool isLogged = _accountService.UserLogin(model, Db);

            if (!isLogged)
            {
                var errorMessage = "Invalid username or password.";
                return View("error", new ErrorViewModel(errorMessage));
            }

            var cookieContent = UserCookieService.GetUserCookie(model.Username);

            Request.Session.AddParameter(".auth_cake", cookieContent);

            Request.Session.AddParameter(ShoppingCartViewModel.SessionKey, new ShoppingCartViewModel());

            Response.Cookies.Add(new HttpCookie(".auth_cake", $"{cookieContent}; {GlobalConstants.HttpOnly}", 7));

            return View("/home/index", Model);

        }

        [HttpGet("/account/logout")]
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

            return View("/home/index", Model);
        }
    }
}
