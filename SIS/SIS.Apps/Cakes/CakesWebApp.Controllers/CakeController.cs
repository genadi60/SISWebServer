namespace CakesWebApp.Controllers
{
    using System;
    using System.Linq;

    using Models;
    using SIS.HTTP.Requests.Contracts;
    using SIS.HTTP.Responses.Contracts;

    public class CakeController : BaseController
    {
        public IHttpResponse AddCake(IHttpRequest request)
        {
            ViewData["show"] = "none";
            return FileViewResponse("add.html");
        }

        public IHttpResponse DoAddCake(IHttpRequest request)
        {
            var name = request.FormData["cakeName"].ToString().Trim();
            var price = decimal.Parse(request.FormData["cakePrice"].ToString());
            var urlString = request.FormData["pictureUrl"].ToString();
            
            var errorMessage = string.Empty;

            if (string.IsNullOrWhiteSpace(name))
            {
                errorMessage = "Please, provide valid product name.";
                return BadRequestError(errorMessage);
            }

            if (Db.Products.Any(p => p.Name.Equals(name)))
            {
                errorMessage = $"Product with name: {name} already exists.";
                return BadRequestError(errorMessage);
            }

            var cake = new Product
            {
                Name = name,
                Price = price,
                ImageUrl = urlString
            };

            Db.Products.Add(cake);
            try
            {
                Db.SaveChanges();
            }
            catch (Exception e)
            {
                return ServerError(e.Message);
            }

            ViewData["show"] = "display";
            ViewData["name"] = name;
            ViewData["price"] = price.ToString("F");
            
            return FileViewResponse("add.html");
        }

        public IHttpResponse Search(IHttpRequest request)
        {
            var results = string.Empty;
            var searchKey = "searchKey";
            var urlParameters = request.QueryData;

            if (urlParameters.ContainsKey(searchKey))
            {
                var searchTerm = urlParameters[searchKey].ToString();
                
                var savedCakes = Db.Products
                    .Where(c => c.Name.ToLower().Contains(searchTerm.ToLower()))
                    .Select(c => $@"<div><a href=""/details?{c.Id}"">{c.Name}</a> ${c.Price} <input type=""submit"", name=""order"", value=""Order"">");

                results = string.Join(Environment.NewLine, savedCakes);
            }

            ViewData["result"] = results;

            return FileViewResponse("search.html");
        }

        public IHttpResponse Details(IHttpRequest request)
        {
            var cakeId = request.QueryData;
            return View("details");
        }
    }
}
