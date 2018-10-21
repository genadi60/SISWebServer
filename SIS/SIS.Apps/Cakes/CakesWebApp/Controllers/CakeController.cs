using CakesWebApp.ViewModels;
using CakesWebApp.ViewModels.Product;

namespace CakesWebApp.Controllers
{
    using System.Globalization;
    using System.Linq;
    using System.Net;
    using System.Text;

    using Services;
    using Services.Contracts;
    using SIS.HTTP.Responses.Contracts;
    using SIS.MvcFramework.Attributes;
    using SIS.MvcFramework.Services.Contracts;

    public class CakeController : BaseController
    {
        private readonly IProductService _productService;

        public CakeController(ProductService productService)
        {
            _productService = productService;
        }

        [HttpGet("/add")]
        public IHttpResponse AddCake()
        {
            if (!IsAuthenticated())
            {
                ViewData["visible"] = "bloc";
                ViewData["title"] = "Login";
                return View("account/login");
            }

            ViewData["show"] = "none";
            ViewData["title"] = "Add Cake";

            return View("products/add");
        }

        [HttpPost("/add")]
        public IHttpResponse DoAddCake()
        {
            if (!IsAuthenticated())
            {
                ViewData["visible"] = "bloc";
                ViewData["title"] = "Login";
                return View("account/login");
            }

            var name = Request.FormData["name"].ToString().Trim();
            var price = decimal.Parse(Request.FormData["price"].ToString());
            var urlString = WebUtility.HtmlDecode(Request.FormData["imageUrl"].ToString());

            if (string.IsNullOrWhiteSpace(name) || !IsAuthenticated())
            {
                var errorMessage = "Please, provide valid product name.";
                return BadRequestError(errorMessage);
            }

            var addProductModel = new AddProductViewModel
            {
                Name = name,
                Price = price,
                ImageUrl = urlString
            };

            _productService.Create(addProductModel);

            ViewData["show"] = "display";
            ViewData["name"] = name;
            ViewData["price"] = price.ToString("F");

            return View("products/add");
        }

        [HttpGet("/search")]
        public IHttpResponse Search()
        {
            if (!IsAuthenticated())
            {
                ViewData["visible"] = "bloc";
                ViewData["title"] = "Login";
                return View("account/login");
            }

            const string searchTermKey = "searchTerm";

            var parameters = Request.QueryData;
            var shoppingCart = Request.Session.GetParameter<ShoppingCartViewModel>(ShoppingCartViewModel.SessionKey);

            var searchTerm = parameters.ContainsKey(searchTermKey)
                ? (string)parameters[searchTermKey]
                : null;

            bool isSearchKey = Request.Url.Contains(searchTermKey);

            if (isSearchKey)
            {
                ViewData[searchTermKey] = searchTerm;
            }

            ViewData["title"] = "Search Cake";

            if (searchTerm == null && string.IsNullOrEmpty(ViewData[searchTermKey]))
            {
                ViewData["showCart"] = "none";
                ViewData["results"] = @"<h2 class=""text text-info"">Please, make a Choice.</h2>";
                return View("products/search");
            }

            searchTerm = searchTerm ?? ViewData[searchTermKey];
            ViewData[searchTermKey] = searchTerm;

            var allProducts = _productService.All(searchTerm);

            if (!allProducts.Any())
            {
                ViewData["results"] = @"<h2 class=""text text-info"">No cakes found.</h2>";
                ViewData["showCart"] = "none";
            }
            else
            {
                var sb = new StringBuilder();
                sb.AppendLine(@"<div class=""row""><div class=""col-sm-3""></div><table class=""table table-bordered table-striped col-sm-6 ""><tbody><tr><th scope=""col-sm-4"">Name</th><th scope=""col-sm-1"">Price</th><th scope=""col-sm-1"">Order</th></tr>");

                foreach (var model in allProducts)
                {
                    sb.AppendLine($@"<tr ><td><a  class=""btn btn-outline-primary col-sm-12"" href=""/details?id={model.Id}"">{model.Name}</a></td><td><p>${model.Price:F2}</p></td><td><form method=""get"" action=""shopping/add"" class=""col-sm-1""><button type=""submit"" name=""id"" class=""btn btn-outline-primary"" value=""{model.Id}"">Order</button></form></td></tr>");
                }

                sb.AppendLine(@"</tbody></table><div class=""col-sm-3""></div></div>");

                var result = sb.ToString().Trim();

                ViewData["results"] = result;
            }

            var totalProducts = shoppingCart.ProductIds.Count;
            var totalProductsText = totalProducts != 1 ? "products" : "product";

            ViewData["showCart"] = "block";
            ViewData["products"] = $"{totalProducts} {totalProductsText}";

            return View("products/search");
        }

        [HttpGet("/details")]
        public IHttpResponse CakeDetails()
        {
            if (!IsAuthenticated())
            {
                ViewData["visible"] = "bloc";
                ViewData["title"] = "Login";
                return View("account/login");
            }

            var cakeId = Request.QueryData["id"].ToString().Trim();
            var id = int.Parse(cakeId);


            if (string.IsNullOrWhiteSpace(cakeId))

            {
                var message = "Invalid request.";
                return BadRequestError(message);
            }

            var cake = Db.Products
                .FirstOrDefault(c => c.Id == id);

            if (cake == null)
            {
                ViewData["title"] = "Home";
                return View("home/index");
            }

            var name = cake.Name.Trim();
            var price = cake.Price;
            var pictureUrl = cake.ImageUrl;

            ViewData["cakeName"] = name;
            ViewData["cakeId"] = cakeId;
            ViewData["cakePrice"] = price.ToString(CultureInfo.InvariantCulture);
            ViewData["pictureUrl"] = pictureUrl;
            ViewData["title"] = "Cake Details";

            return View("products/cakeDetails");
        }

        [HttpGet("/cakes")]
        public IHttpResponse GetCakes()
        {
            IsAuthenticated();

            var cakes = _productService.ShowAll();

            if (cakes == null || cakes.Count == 0)
            {
                ViewData["title"] = "Home";
                return View("home/index");
            }

            var sb = new StringBuilder();
            foreach (var cake in cakes)
            {
                sb.AppendLine($"<div><img src=\"{cake.ImageUrl}\" alt=\"Cake\" width=\"500\" height=\"400\"></div><br/><p><h3><em>{cake.Name}</em></h3></p><br/>");
            }

            ViewData["imgSources"] = sb.ToString().Trim();
            ViewData["title"] = "All Cakes";

            return View("products/cakes");
        }
    }
}