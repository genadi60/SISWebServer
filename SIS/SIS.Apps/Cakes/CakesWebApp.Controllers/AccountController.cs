namespace CakesWebApp.Controllers
{
    using System;
    using System.Linq;

    using Services;
    using Services.Contracts;
    using SIS.HTTP.Common;
    using SIS.HTTP.Cookies;
    using SIS.HTTP.Responses.Contracts;
    using ViewModels;
    using ViewModels.Account;

    public class AccountController : BaseController
    {
        
        private readonly IUserService _userService;
        
        public AccountController()
        {
            _userService = new UserService();
        }

        public IHttpResponse Register()
        {
            SetDefaultViewData();
            ViewData["title"] = "Register";
            return View("account/register");
        }

        public IHttpResponse DoRegister()
        {
            SetDefaultViewData();
            var name = Request.FormData["name"].ToString().Trim();
            var userName = Request.FormData["username"].ToString().Trim();
            var password = Request.FormData["password"].ToString();
            var confirmPassword = Request.FormData["confirmPassword"].ToString();
            string errorMessage;

            var model = new RegisterViewModel
            {
                Name = name,
                Username = userName,
                Password = password,
                ConfirmPassword = confirmPassword
            };

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

            if (string.IsNullOrWhiteSpace(model.Password) || password.Length < 6)
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

        public IHttpResponse Login()
        {
            SetDefaultViewData();
            ViewData["title"] = "Login";

            return View("account/login");
        }

        public IHttpResponse DoLogin()
        {
            string username = null;
            string password = null;
            if (Request.FormData.ContainsKey("username"))
            {
                username = Request.FormData["username"].ToString().Trim();
            }

            if (Request.FormData.ContainsKey("password"))
            {
                password = Request.FormData["password"].ToString();
            }
            
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password)
                                                    || string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                var errorMessage = "Invalid username or password.";
                return BadRequestError(errorMessage);
            }

            var hashedPassword = HashService.Hash(password);

            using (Db)
            {
                var user = Db.Users.FirstOrDefault(u => u.Username.Equals(username));
                bool isEqual = user != null && user.Username.Equals(username);

                if (!Db.Users.Any(u => u.Password.Equals(hashedPassword)) || !isEqual)
                {
                    var errorMessage = "Invalid username or password.";
                    return BadRequestError(errorMessage);
                }
            }
            

            Request.Session.AddParameter(".auth_cake", username);

            Request.Session.AddParameter(ShoppingCartViewModel.SessionKey, new ShoppingCartViewModel());

            ViewData["authenticated"] = "bloc";
            ViewData["notAuthenticated"] = "none";
            ViewData["title"] = "Home";
            ViewData["greeting"] = username;
            ViewData["searchTerm"] = null;

            var cookieContent = UserCookieService.GetUserCookie(username);

            Response.Cookies.Add(new HttpCookie(".auth_cake", $"{cookieContent}; {GlobalConstants.HttpOnly}", 7));

            return View("/");

        }

        public IHttpResponse Logout()
        {
            if (!Request.Cookies.ContainsCookie(".auth_cake"))
            {
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

                var user = Db.Users
                    .FirstOrDefault(u => u.Username.Equals(username));

                if (user == null)
                {
                    return View("home/index");
                }

                var registrationDate = user.DateOfRegistration.ToString("dd-MM-yyyy");
                var ordersCount = Db.Orders
                    .Count(o => o.UserId == user.Id);
                string content = $"<p>Name: {user.Name}</p>" +
                                 $"<p>Username: {user.Username}</p>" +
                                 $"<p>Registered on: {registrationDate}</p>" +
                                 $"<p>Orders Count: {ordersCount}</p>";
                ViewData["show"] = "bloc";
                ViewData["profile"] = content;
                ViewData["title"] = "My Profile";

                return View("account/profile");
            }
        }
    }
}
