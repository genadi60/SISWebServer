﻿using System;

namespace CakesWebApp.Controllers
{
    using System.Globalization;
    using System.Linq;
    using System.Text;
    
    using InputModels.Product;
    using Services;
    using Services.Contracts;
    using SIS.HTTP.Responses.Contracts;
    using SIS.MvcFramework.Attributes;
    using ViewModels.Product;
    using ViewModels.Shopping;

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
        public IHttpResponse DoAddCake(AddProductInputModel model)
        {
            if (!IsAuthenticated())
            {
                ViewData["visible"] = "bloc";
                ViewData["title"] = "Login";
                return View("account/login");
            }

            if (string.IsNullOrWhiteSpace(model.Name) || !IsAuthenticated())
            {
                var errorMessage = "Please, provide valid product name.";
                return BadRequestError(errorMessage);
            }

            _productService.Create(model);

            ViewData["show"] = "display";
            ViewData["name"] = model.Name;
            ViewData["price"] = model.Price.ToString("F2");

            return View("products/add");
        }//

        [HttpGet("/search")]
        public IHttpResponse Search(SearchInputModel model)
        {
            if (!IsAuthenticated())
            {
                ViewData["visible"] = "bloc";
                ViewData["title"] = "Login";
                return View("account/login");
            }

            const string searchTermKey = "searchTerm";

            //var parameters = Request.QueryData;
            var shoppingCart = Request.Session.GetParameter<ShoppingCartViewModel>(ShoppingCartViewModel.SessionKey);

            var searchTerm = model.SearchTerm;
            
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

                foreach (var productViewModel in allProducts)
                {
                    sb.AppendLine($@"<tr ><td><a  class=""btn btn-outline-primary col-sm-12"" href=""/details?id={productViewModel.Id}"">{productViewModel.Name}</a></td><td><p>${productViewModel.Price}</p></td><td><form method=""get"" action=""shopping/add"" class=""col-sm-1""><button type=""submit"" name=""id"" class=""btn btn-outline-primary"" value=""{productViewModel.Id}""><i class=""fas fa-cart-plus""></i> Add to Cart</button></form></td></tr>");
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
        public IHttpResponse CakeDetails(ProductShowViewModel model)
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