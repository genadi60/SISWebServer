using CakesWebApp.ViewModels;
using CakesWebApp.ViewModels.Account;

namespace CakesWebApp.Controllers
{
    using System;
    using System.Linq;

    using Services;
    using Services.Contracts;
    using SIS.HTTP.Common;
    using SIS.HTTP.Cookies;
    using SIS.HTTP.Responses.Contracts;
    using SIS.MvcFramework.Attributes;

    public class AccountController : BaseController
    {
        
        private readonly IUserService _userService;
        
        public AccountController(UserService userService)
        {
            _userService = userService;
        }

        [HttpGet("/register")]
        public IHttpResponse Register()
        {
            SetDefaultViewData();
            ViewData["title"] = "Register";
            return View("account/register");
        }

        [HttpPost("/register")]
        public IHttpResponse DoRegister(RegisterViewModel model)
        {
            SetDefaultViewData();
            //var name = model.Name;
            //var userName = model.Username;
            //var password = model.Password;
            //var confirmPassword = model.ConfirmPassword;
            string errorMessage;
            
            //1.Validate!
            //2.Generate password hash.
            //3.Create user.
            //4.Redirect to home page.

            //1.
            if (string.IsNullOrWhiteSpace(model.Username) || model.Username.Length < 4)
            {
                errorMessage = "Please, provide valid username with length 4 or more symbols";
                return BadRequestError(errorMessage);
            }

            if (Db.Users.Any(u => u.Username.Equals(model.Username)))
            {
                errorMessage = $"User with username: {model.Username} already exists.";
                return BadRequestError(errorMessage);
            }

            if (string.IsNullOrWhiteSpace(model.Password) || model.Password.Length < 6)
            {
                errorMessage = "Please, provide valid password with length 6 or more symbols";
                return BadRequestError(errorMessage);
            }

            if (!model.Password.Equals(model.ConfirmPassword))
            {
                errorMessage = "Passwords do not match.";
                return BadRequestError(errorMessage);
            }

            //2.
            var hashedPassword = HashService.Hash(model.Password);
            model.Password = hashedPassword;

            //3.
            try
            {
                _userService.Create(model);
            }
            catch (Exception e)
            {
                //TODO Log error
                return ServerError(e.Message);
            }
            
            //4.
            ViewData["title"] = "Home";
            return View("home/index");
        }

        [HttpGet("/login")]
        public IHttpResponse Login()
        {
            if (Request.Cookies.ContainsCookie(".auth_cake"))
            {
                
                //ViewData["title"] = "Home";
                return BadRequestError("You are now logged!");
            }

            SetDefaultViewData();
            ViewData["title"] = "Login";

            return View("account/login");
        }

        [HttpPost("/login")]
        public IHttpResponse DoLogin(LoginViewModel model)
        {
            //string username = null;
            //string password = null;
            //if (Request.FormData.ContainsKey("username"))
            //{
            //    username = Request.FormData["username"].ToString().Trim();
            //}

            //if (Request.FormData.ContainsKey("password"))
            //{
            //    password = Request.FormData["password"].ToString();
            //}
            
            if (string.IsNullOrWhiteSpace(model.Username) || string.IsNullOrWhiteSpace(model.Password)
                                                    || string.IsNullOrEmpty(model.Username) || string.IsNullOrEmpty(model.Password))
            {
                var errorMessage = "Invalid username or password.";
                return BadRequestError(errorMessage);
            }

            var hashedPassword = HashService.Hash(model.Password);

            using (Db)
            {
                var user = Db.Users.FirstOrDefault(u => u.Username.Equals(model.Username));
                bool isEqual = user != null && user.Username.Equals(model.Username);

                if (!Db.Users.Any(u => u.Password.Equals(hashedPassword)) || !isEqual)
                {
                    var errorMessage = "Invalid username or password.";
                    return BadRequestError(errorMessage);
                }

                ViewData["greeting"] = user.Name;
            }
            

            Request.Session.AddParameter(".auth_cake", model.Username);

            Request.Session.AddParameter(ShoppingCartViewModel.SessionKey, new ShoppingCartViewModel());

            ViewData["authenticated"] = "bloc";
            ViewData["notAuthenticated"] = "none";
            ViewData["title"] = "Home";
            ViewData["searchTerm"] = null;

            var cookieContent = UserCookieService.GetUserCookie(model.Username);

            Response.Cookies.Add(new HttpCookie(".auth_cake", $"{cookieContent}; {GlobalConstants.HttpOnly}", 7));

            return View("/");

        }

        [HttpGet("/logout")]
        public IHttpResponse Logout()
        {
            if (!Request.Cookies.ContainsCookie(".auth_cake"))
            {
                SetDefaultViewData();
                ViewData["title"] = "Home";
                return View("home/index");
            }

            var cookie = Request.Cookies.GetCookie(".auth_cake");
            cookie.Delete();

            SetDefaultViewData();

            ViewData["title"] = "Home";
            ViewData["notAuthenticated"] = "bloc";

            var response = View("home/index");
            response.Cookies.Add(cookie);

            return response;
        }

        [HttpGet("/profile")]
        public IHttpResponse GetProfile()
        {
            if (!IsAuthenticated())
            {
                ViewData["visible"] = "bloc";
                ViewData["title"] = "Login";
                return View("account/login");
            }

            using (Db)
            {
                var username = User;

                if (username == null)
                {
                    return View("home/index");
                }

                var model = Db.Users
                    .Where(u => u.Username.Equals(username))
                    .Select(u => new ProfileViewModel
                    {
                        Name = u.Name,
                        Username = u.Username,
                        RegistrationDate = u.DateOfRegistration.ToString("dd-MM-yyyy"),
                        TotalOrders = u.Orders.Count.ToString()
                    })
                    .First();

                if (model == null)
                {
                    return View("home/index");
                }

                //var ordersCount = Db.Orders
                //    .Count(o => o.UserId == user.Id);
                string content = $"<p>Name: {model.Name}</p>" +
                                 $"<p>Username: {model.Username}</p>" +
                                 $"<p>Registered on: {model.RegistrationDate}</p>" +
                                 $"<p>Orders Count: {model.TotalOrders}</p>";
                ViewData["show"] = "bloc";
                ViewData["profile"] = content;
                ViewData["title"] = "My Profile";

                return View("account/profile");
            }
        }
    }
}
