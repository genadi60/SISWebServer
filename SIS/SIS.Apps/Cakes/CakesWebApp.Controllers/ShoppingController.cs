namespace CakesWebApp.Controllers
{
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
        
        public ShoppingController()
        {
            _user = new UserService();
            _product = new ProductService();
            _shopping = new ShoppingService();
        }

        public IHttpResponse AddToCart()
        {
            if (!IsAuthenticated())
            {
                ViewData["visible"] = "bloc";
                ViewData["title"] = "Login";
                return FileViewResponse("account/login");
            }

            var cakeId = Request.QueryData["id"].ToString().Trim();
            var id = int.Parse(cakeId);

            var userName = GetUsername();

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

            var shoppingCart = Request.Session.GetParameter<ShoppingCartViewModel>(ShoppingCartViewModel.SessionKey);
            var cartproductsIds = shoppingCart.ProductIds;

            cartproductsIds.Add(id);

            return new RedirectResult("cart");

        }

        public IHttpResponse ShowCart()
        {
            if (!IsAuthenticated())
            {
                ViewData["visible"] = "bloc";
                ViewData["title"] = "Login";
                return FileViewResponse("account/login");
            }

            if (!Request.Cookies.HasCookies() || !Request.Cookies.ContainsCookie(".auth_cake"))
            {
                ViewData["title"] = "Login";
                return FileViewResponse("account/login");
            }

            var shoppingCart = Request.Session.GetParameter<ShoppingCartViewModel>(ShoppingCartViewModel.SessionKey);

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

                var sb = new StringBuilder();

                foreach (var model in productsInCart)
                {
                    sb.AppendLine($@"<tr><td>{model.Name}</td><td>${model.Price:F2}</td></tr>");
                }
               
                var totalPrice = productsInCart
                    .Sum(pr => pr.Price);

                ViewData["cartItems"] = sb.ToString().Trim();
                ViewData["totalCost"] = $"{totalPrice:F2}";
            }

            ViewData["title"] = "My Cart";

            return FileViewResponse("shopping/cart");
        }

        public IHttpResponse FinishOrder()
        {
            if (!IsAuthenticated())
            {
                ViewData["visible"] = "bloc";
                ViewData["title"] = "Login";
                return FileViewResponse("account/login");
            }

            var username = GetUsername();

            if (username == null)
            {
                ViewData["title"] = "Login";
                return FileViewResponse("account/login");
            }

            var shoppingCart = Request.Session.GetParameter<ShoppingCartViewModel>(ShoppingCartViewModel.SessionKey);

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

        public IHttpResponse MyOrders()
        {
            if (!IsAuthenticated())
            {
                ViewData["visible"] = "bloc";
                ViewData["title"] = "Login";
                return FileViewResponse("account/login");
            }

            using (Db)
            {
                var username = GetUsername();

                var orders = Db.Orders
                    .Where(o => o.User.Username.Equals(username))
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

        public IHttpResponse Details()
        {
            if (!IsAuthenticated())
            {
                ViewData["visible"] = "bloc";
                ViewData["title"] = "Login";
                return FileViewResponse("account/login");
            }

            var orderParameter = Request.QueryData["id"].ToString().Trim();
            var orderId = int.Parse(orderParameter);
            var username = GetUsername();

            using (Db)
            {
                var order = Db.Orders.FirstOrDefault(o => o.Id == orderId && o.User.Username.Equals(username));

                if (order == null)
                {
                    return BadRequestError("This order not exists!");
                }

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
                    result.AppendLine($@"<tr><td><a class=""btn"" href=""/details?id={model.Id}"">{model.Name}</a></td><td>${model.Price}</td></tr>");
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
