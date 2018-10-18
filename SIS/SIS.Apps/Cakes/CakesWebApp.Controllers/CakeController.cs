namespace CakesWebApp.Controllers
{
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Net;
    using System.Text;
    
    using Services;
    using Services.Contracts;
    using SIS.HTTP.Requests.Contracts;
    using SIS.HTTP.Responses.Contracts;
    using ViewModels;
    using ViewModels.Product;

    public class CakeController : BaseController
    {
        private readonly IProductService _productService = new ProductService();
        public CakeController()
        {
        }
        
        public IHttpResponse AddCake()
        {
            if (!IsAuthenticated())
            {
                ViewData["visible"] = "bloc";
                ViewData["title"] = "Login";
                return FileViewResponse("account/login");
            }
            
            ViewData["show"] = "none";
            ViewData["title"] = "Add Cake";

            return FileViewResponse("products/add");
        }

        public IHttpResponse DoAddCake()
        {
            if (!IsAuthenticated())
            {
                ViewData["visible"] = "bloc";
                ViewData["title"] = "Login";
                return FileViewResponse("account/login");
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

            return FileViewResponse("products/add");
        }

        public IHttpResponse Search()
        {
            if (!IsAuthenticated())
            {
                ViewData["visible"] = "bloc";
                ViewData["title"] = "Login";
                return FileViewResponse("account/login");
            }

            const string searchTermKey = "searchTerm";

            var parameters = Request.QueryData;
            var shoppingCart = Request.Session.GetParameter<ShoppingCartViewModel>(ShoppingCartViewModel.SessionKey);

            ViewData["title"] = "Search Cake";

            if (parameters.Count == 0 && !shoppingCart.ProductIds.Any())
            {
                ViewData["showCart"] = "none";
                ViewData["searchTerm"] = null;
                ViewData["results"] = null;
                return FileViewResponse("products/search");
            }

            ViewData["results"] = null;

            var searchTerm = parameters.ContainsKey(searchTermKey)
                ? (string)parameters[searchTermKey]
                : null;

           ViewData[searchTermKey] = searchTerm;

            var allProducts = _productService.All(searchTerm);

            if (!allProducts.Any())
            {
                ViewData["results"] = "No cakes found";
            }
            else
            {
                var sb = new StringBuilder();
                sb.AppendLine(@"<div class=""container-fluid"">");

                foreach (var model in allProducts)
                {
                    sb.AppendLine($@"<div class=""form-group row""> 
                        <div class=""col-sm-3""></div>
                        <a class=""btn btn-outline-primary col-sm-4"" href=""/details?id={model.Id}"">{model.Name}</a>
                        <div class=""col-sm-1"">
                        <p>${model.Price}</p>
                        </div>
                        <form method=""get"" action=""shopping/add"" class=""col-sm-1"">
                            <button type=""submit"" name=""id"" class=""btn btn-outline-primary"" value=""{model.Id}"">Order</button>
                        </form>
                        <div class=""col-sm-3""></div>
                        </div>");
                }

                sb.AppendLine("</div>");

                var result = sb.ToString().Trim();
                ViewData["results"] = result;
            }
            
            ViewData["showCart"] = "none";

            

            if (shoppingCart.ProductIds.Any())
            {
                var totalProducts = shoppingCart.ProductIds.Count;
                var totalProductsText = totalProducts != 1 ? "products" : "product";

                ViewData["showCart"] = "block";
                ViewData["products"] = $"{totalProducts} {totalProductsText}";
            }

            return FileViewResponse("products/search");
        }

        public IHttpResponse CakeDetails()
        {
            if (!IsAuthenticated())
            {
                ViewData["visible"] = "bloc";
                ViewData["title"] = "Login";
                return FileViewResponse("account/login");
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
                return FileViewResponse("home/index");
            }

            var name = cake.Name.Trim();
            var price = cake.Price;
            var pictureUrl = cake.ImageUrl;

            ViewData["cakeName"] = name;
            ViewData["cakeId"] = cakeId;
            ViewData["cakePrice"] = price.ToString(CultureInfo.InvariantCulture);
            ViewData["pictureUrl"] = pictureUrl;
            ViewData["title"] = "Cake Details";
            
            return FileViewResponse("products/cakeDetails");
        }

        public IHttpResponse GetCakes()
        {
            IsAuthenticated();

            var cakes = _productService.ShowAll();

            if (cakes == null || cakes.Count == 0)
            {
                ViewData["title"] = "Home";
                return FileViewResponse("home/index");
            }

            var sb = new StringBuilder();
            foreach (var cake in cakes)
            {
                sb.AppendLine($"<div><img src=\"{cake.ImageUrl}\" alt=\"Cake\" width=\"500\" height=\"400\"></div><br/><p><h3><em>{cake.Name}</em></h3></p><br/>");
            }

            ViewData["imgSources"] = sb.ToString().Trim();
            ViewData["title"] = "All Cakes";

            return FileViewResponse("products/cakes");
        }
    }
}
