using System;
using System.Linq;
using CakesWebApp.Models;
using Services;
using SIS.HTTP.Common;
using SIS.HTTP.Cookies;
using SIS.HTTP.Enums;
using SIS.HTTP.Requests.Contracts;
using SIS.HTTP.Responses;
using SIS.HTTP.Responses.Contracts;
using SIS.WebServer.Results;

namespace CakesWebApp.Controllers
{
    public class AccountController : BaseController
    {
        private IHashService _hashService;
        private IUserCookieService _userCookieService;

        public AccountController()
        {
            _hashService = new HashService();
            _userCookieService = new UserCookieService();
        }

        public IHttpResponse Register(IHttpRequest request)
        {
            return View("Register");
        }

        public IHttpResponse DoRegister(IHttpRequest request)
        {
            var userName = request.FormData["username"].ToString().Trim();
            var password = request.FormData["password"].ToString();
            var confirmPassword = request.FormData["confirmPassword"].ToString();
            var errorMessage = string.Empty;
            //1.Validate!
            //2.Generate password hash.
            //3.Create user.
            //4.Redirect to home page.

            //1.
            if (string.IsNullOrWhiteSpace(userName) || userName.Length < 4)
            {
                errorMessage = "Please, provide valid username with length 4 or more symbols";
                return BadRequestError(errorMessage);
            }

            if (Db.Users.Any(u => u.Username.Equals(userName)))
            {
                errorMessage = $"User with username: {userName} already exists.";
                return BadRequestError(errorMessage);
            }

            if (string.IsNullOrWhiteSpace(password) || password.Length < 6)
            {
                errorMessage = "Please, provide valid password with length 6 or more symbols";
                return BadRequestError(errorMessage);
            }

            if (!password.Equals(confirmPassword))
            {
                errorMessage = "Passwords do not match.";
                return BadRequestError(errorMessage);
            }

            //2.
            var hashedPassword = _hashService.Hash(password);

            //3.
            var user = new User
            {
                Name = userName,
                Username = userName,
                Password = hashedPassword
            };

            Db.Users.Add(user);
            try
            {
                Db.SaveChanges();
            }
            catch (Exception e)
            {
                //TODO Log error
                return ServerError(e.Message);
            }
            
            //4.
            return new RedirectResult("/");
        }

        public IHttpResponse Login(IHttpRequest request)
        {
            return View("Login");
        }

        public IHttpResponse DoLogin(IHttpRequest request)
        {
            var userName = request.FormData["username"].ToString().Trim();
            var password = request.FormData["password"].ToString();
            var errorMessage = string.Empty;

            var hashedPassword = _hashService.Hash(password);

            var user = Db.Users
                .FirstOrDefault(u => u.Username.Equals(userName) && u.Password.Equals(hashedPassword));

            if (user == null)
            {
                errorMessage = "Invalid username or password.";
                return BadRequestError(errorMessage);
            }

            var response = new RedirectResult("/");

            var cookieContent = _userCookieService.GetUserCookie(userName);
            
            response.Cookies.Add(new HttpCookie(".auth_cake", $"{cookieContent}; {GlobalConstants.HttpOnly}" , 7));

            return response;
        }

        public IHttpResponse Logout(IHttpRequest request)
        {
            if (!request.Cookies.ContainsCookie(".auth_cake"))
            {
                return new RedirectResult("/");
            }
            
            var cookie = request.Cookies.GetCookie(".auth_cake");
            cookie.Delete();
            var response = new RedirectResult("/");
            response.Cookies.Add(cookie);

            return response;
        }
    }
}
