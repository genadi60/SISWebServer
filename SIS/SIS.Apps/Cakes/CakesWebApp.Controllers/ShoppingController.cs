namespace CakesWebApp.Controllers
{
    using System.Globalization;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    using Services;
    using Services.Contracts;
    using SIS.HTTP.Requests.Contracts;
    using SIS.HTTP.Responses.Contracts;
    using SIS.WebServer.Results;
    using ViewModels;
    using ViewModels.Product;


    public class ShoppingController : BaseController
    {
        private readonly IUserService _user;
        private readonly IProductService _product;
        private readonly IShoppingService _shopping;

        public ShoppingController(Dictionary<string, string> viewData) : base(viewData)
        {
            _user = new UserService();
            _product = new ProductService();
            _shopping = new ShoppingService();
        }

        public IHttpResponse AddToCart(IHttpRequest request)
        {
            var cakeId = request.FormData["id"].ToString().Trim();
            var id = int.Parse(cakeId);

            var userName = GetUserName(request);

            if (userName == null)
            {
                ViewData["title"] = "Login";
                return FileViewResponse("account/login");
            }

            var productExists = _product.Exists(id);

            if (!productExists)
            {
                return NotFound.PageNotFound();
            }

            var shoppingCart = request.Session.GetParameter<ShoppingCartViewModel>(ShoppingCartViewModel.SessionKey);
            var cartproductsIds = shoppingCart.ProductIds;

            cartproductsIds.Add(id);

            var sb = new StringBuilder();

            var cartProducts = Db.Products
                .Where(p => cartproductsIds.Contains(p.Id))
                .Select(p => new ProductListingViewModel
                {
                    Id = p.Id,
                    Name = p.Name,
                    Price = p.Price
                })
                .ToList();
            sb.AppendLine("<form>");
            foreach (var model in cartProducts)
            {
                sb.AppendLine($@"<div class=""form-group row""> 
                        <div class=""col-sm-3""></div>
                        <!--<div class=""col-sm-4"">-->
                            <a class=""btn btn-outline-primary col-sm-4"" href=""/details/{model.Id}"">{model.Name}</a>
                        <!--</div>-->
                        <div class=""col-sm-1"">
                        <p style=""text-align: centered"">${model.Price}</p>
                        </div>
                        <!--<div class=""col-sm-1"">
                            <form method=""post"" action=""shopping/add"">
                                <button type=""submit"" name=""id"" class=""btn btn-outline-primary"" value=""{model.Id}"">Order</button>
                            </form>
                        </div>-->
                        <div class=""col-sm-4""></div>
                        </div>");
            }

            sb.AppendLine("</form>");

            var sum = cartProducts.Sum(p => p.Price);

            ViewData["title"] = "My Cart";
            ViewData["cartItems"] = sb.ToString().Trim();
            ViewData["totalCost"] = sum.ToString(CultureInfo.InvariantCulture);

            return FileViewResponse("shopping/cart");

        }

        public IHttpResponse ShowCart(IHttpRequest request)
        {
            if (!request.Cookies.HasCookies() || !request.Cookies.ContainsCookie(".auth_cake"))
            {
                ViewData["title"] = "Login";
                return FileViewResponse("account/login");
            }

            var shoppingCart = request.Session.GetParameter<ShoppingCartViewModel>(ShoppingCartViewModel.SessionKey);

            if (!shoppingCart.ProductIds.Any())
            {
                ViewData["cartItems"] = "No items in your cart";
                ViewData["totalCost"] = "$0.00";
            }
            else
            {
                var productsInCart = _product
                    .FindProductsInCart(shoppingCart.ProductIds)
                    .ToList();

                var items = productsInCart
                    .Select(pr => $"<div>{pr.Name} - ${pr.Price:F2}</div><br />");

                var totalPrice = productsInCart
                    .Sum(pr => pr.Price);

                ViewData["cartItems"] = string.Join(string.Empty, items);
                ViewData["totalCost"] = $"{totalPrice:F2}";
            }

            ViewData["title"] = "My Cart";

            return FileViewResponse("shopping/cart");
        }

        public IHttpResponse FinishOrder(IHttpRequest request)
        {
            var username = GetUserName(request);

            if (username == null)
            {
                ViewData["title"] = "Login";
                return FileViewResponse("account/login");
            }

            var shoppingCart = request.Session.GetParameter<ShoppingCartViewModel>(ShoppingCartViewModel.SessionKey);

            var userId = _user.GetUserId(username);

            var productIds = shoppingCart.ProductIds;
            if (!productIds.Any())
            {
                return new RedirectResult("/");
            }

            if (userId != null) _shopping.CreateOrder(userId.Value, productIds);

            shoppingCart.ProductIds.Clear();

            ViewData["title"] = "Order Finished";

            return FileViewResponse("shopping/order");
        }

        public IHttpResponse MyOrders(IHttpRequest request)
        {
            if (!request.Cookies.HasCookies() || !request.Cookies.ContainsCookie(".auth_cake"))
            {
                ViewData["title"] = "Login";
                return FileViewResponse("account/login");
            }

            using (Db)
            {
                var userName = GetUserName(request);

                var orders = Db.Orders
                    .Where(o => o.User.Username.Equals(userName))
                    .ToList();

                var result = new StringBuilder();

                foreach (var order in orders)
                {
                    var sum = order.Products
                        .Sum(p => p.Product.Price);
                    result.AppendLine(
                        $@"<tr><td><a class=""btn"" href=""/list?id={order.Id}"">{order.Id}</a></td><td>{order.DateOfCreation}</td><td>${sum}</td></tr>");
                }

                ViewData["orders"] = result.ToString().Trim();
                ViewData["title"] = "My Orders";

                return FileViewResponse("shopping/my-orders");
            }
        }

        public IHttpResponse Details(IHttpRequest request)
        {
            if (!request.Cookies.HasCookies() || !request.Cookies.ContainsCookie(".auth_cake"))
            {
                ViewData["title"] = "Login";
                return FileViewResponse("account/login");
            }

            var orderParameter = request.QueryData["id"].ToString().Trim();
            var orderId = int.Parse(orderParameter);

            using (Db)
            {
                var order = Db.Orders.Find(orderId);
                    
                var products = order.Products
                    .Select(o => new ProductListingViewModel()
                    {
                        Id = o.ProductId,
                        Name = o.Product.Name,
                        Price = o.Product.Price
                    })
                    .ToList();

                var result = new StringBuilder();

                foreach (var model in products)
                {
                    result.AppendLine($@"<tr><td><a class=""btn"" href=""/details/{model.Id}"">{model.Name}</a></td><td>${model.Price}</td></tr>");
                }

                ViewData["products"] = result.ToString().Trim();
                ViewData["orderId"] = orderParameter;
                ViewData["orderDate"] = order.DateOfCreation.ToString("dd-MM-yyyy");
                ViewData["title"] = $"Order {orderId} Details";

                return FileViewResponse("shopping/details");
            }
        }
    }
}
