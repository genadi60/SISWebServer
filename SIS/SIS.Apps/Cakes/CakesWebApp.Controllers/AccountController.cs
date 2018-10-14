﻿namespace CakesWebApp.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Services;
    using Services.Contracts;
    using SIS.HTTP.Common;
    using SIS.HTTP.Cookies;
    using SIS.HTTP.Requests.Contracts;
    using SIS.HTTP.Responses.Contracts;
    using ViewModels;
    using ViewModels.Account;

    public class AccountController : BaseController
    {
        private readonly IHashService _hashService;
        private readonly IUserCookieService _userCookieService;
        private readonly IUserService _userService;

        public AccountController(Dictionary<string, string> viewData) : base(viewData)
        {
            _hashService = new HashService();
            _userCookieService = new UserCookieService();
            _userService = new UserService();
        }

        public IHttpResponse Register(IHttpRequest request)
        {
            SetDefaultViewData();
            ViewData["title"] = "Register";
            return FileViewResponse("account/register");
        }

        public IHttpResponse DoRegister(IHttpRequest request)
        {
            SetDefaultViewData();
            var name = request.FormData["name"].ToString().Trim();
            var userName = request.FormData["username"].ToString().Trim();
            var password = request.FormData["password"].ToString();
            var confirmPassword = request.FormData["confirmPassword"].ToString();
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
            var hashedPassword = _hashService.Hash(model.Password);
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
            return FileViewResponse("home/index");
        }

        public IHttpResponse Login(IHttpRequest request)
        {
            SetDefaultViewData();
            ViewData["title"] = "Login";

            return FileViewResponse("account/login");
        }

        public IHttpResponse DoLogin(IHttpRequest request)
        {
            var username = request.FormData["username"].ToString().Trim();
            var password = request.FormData["password"].ToString();

            var hashedPassword = _hashService.Hash(password);

            if (!Db.Users.Any(u => u.Username.Equals(username) && u.Password.Equals(hashedPassword)))
            {
                var errorMessage = "Invalid username or password.";
                return BadRequestError(errorMessage);
            }

            request.Session.AddParameter(".auth_cake", username);

            request.Session.AddParameter(ShoppingCartViewModel.SessionKey, new ShoppingCartViewModel());

            ViewData["authenticated"] = "bloc";
            ViewData["notAuthenticated"] = "none";
            ViewData["title"] = "Home";
            ViewData["greeting"] = username;
            ViewData["searchTerm"] = null;

            var response = FileViewResponse("home/index");

            var cookieContent = _userCookieService.GetUserCookie(username);

            response.Cookies.Add(new HttpCookie(".auth_cake", $"{cookieContent}; {GlobalConstants.HttpOnly}", 7));

            return response;

        }

        public IHttpResponse Logout(IHttpRequest request)
        {
            if (!request.Cookies.ContainsCookie(".auth_cake"))
            {
                ViewData["title"] = "Home";
                return FileViewResponse("home/index");
            }

            var cookie = request.Cookies.GetCookie(".auth_cake");
            cookie.Delete();

            SetDefaultViewData();

            ViewData["title"] = "Home";
            ViewData["notAuthenticated"] = "bloc";

            var response = FileViewResponse("home/index");
            response.Cookies.Add(cookie);

            return response;
        }

        public IHttpResponse GetProfile(IHttpRequest request)
        {
            if (!IsAuthenticated(request))
            {
                return FileViewResponse("account/login");
            }

            using (Db)
            {
                var username = GetUserName(request);

                if (username == null)
                {
                    return FileViewResponse("home/index");
                }

                var user = Db.Users
                    .FirstOrDefault(u => u.Username.Equals(username));

                if (user == null)
                {
                    return FileViewResponse("home/index");
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

                return FileViewResponse("account/profile");
            }
        }
    }
}
