namespace CakesWebApp.Controllers
{
    using System.Linq;
    using System.Text;

    using InputModels.Shopping;
    using Services;
    using Services.Contracts;
    using SIS.HTTP.Responses.Contracts;
    using SIS.MvcFramework.Attributes;
    using ViewModels.Product;
    using ViewModels.Shopping;

    public class ShoppingController : BaseController
    {
        private readonly IUserService _user;
        private readonly IProductService _product;
        private readonly IShoppingService _shopping;
        
        public ShoppingController(UserService userService, ProductService productService, ShoppingService shoppingService)
        {
            _user = userService;
            _product = productService;
            _shopping = shoppingService;
        }

        [HttpGet("/shopping/add")]
        public IHttpResponse AddToCart(ProductViewModel model)
        {
            if (!IsAuthenticated())
            {
                ViewData["visible"] = "bloc";
                ViewData["title"] = "Login";
                return View("account/login");
            }

            var cakeId = Request.QueryData["id"].ToString().Trim();
            var id = int.Parse(cakeId);

            var productExists = _product.Exists(id);

            if (!productExists)
            {
                return NotFound();
            }

            var shoppingCart = Request.Session.GetParameter<ShoppingCartViewModel>(ShoppingCartViewModel.SessionKey);
            var cartProductIds = shoppingCart.ProductIds;

            cartProductIds.Add(id);

            return Redirect("cart");

        }

        [HttpGet("/shopping/cart")]
        public IHttpResponse ShowCart(ClearProductInputModel model)
        {
            if (!IsAuthenticated())
            {
                ViewData["visible"] = "bloc";
                ViewData["title"] = "Login";
                return View("account/login");
            }

            ViewData["cart"] = "none";

            var shoppingCart = Request.Session.GetParameter<ShoppingCartViewModel>(ShoppingCartViewModel.SessionKey);

            if (!shoppingCart.ProductIds.Any())
            {
                ViewData["showItems"] = "none";
                ViewData["cartItems"] = "No items in your cart";
                ViewData["totalCost"] = "$0.00";
               
            }
            else
            {
                if (model.ClearProduct != 0)
                {
                    var clearedProductId = model.ClearProduct;

                    var indexToClear = shoppingCart.ProductIds.IndexOf(clearedProductId);

                    shoppingCart.ProductIds.RemoveAt(indexToClear);

                    Request.QueryData.Remove("clearProduct");

                    return Redirect("cart");
                }

                var productsInCart = _product
                    .FindProductsInCart(shoppingCart.ProductIds)
                    .ToList();
                
                var sb = new StringBuilder();
                sb.Append(@"<div class=""row"">
                    <div class=""col-sm-3""></div>
                    <table class=""table table-bordered table-striped col-sm-6"">
                    <tbody>
                    <tr>
                    <th scope=""col"">Name</th>
                    <th scope=""col"">Price</th>
                    <th scope=""col"">Clear</th>
                    </tr>");
                foreach (var productViewModel in productsInCart)
                {
                    sb.Append($@"<tr><td>{productViewModel.Name}</td><td>${productViewModel.Price:F2}</td><td><a class=""btn btn-outline-primary"" href=""cart?clearProduct={productViewModel.Id}""><i class=""far fa-thumbs-down""></i> Clear</a></td></tr>");
                }

                sb.Append(@"</tbody></table><div class=""col-sm-3""></div></div>");
               
                var totalPrice = productsInCart
                    .Sum(pr => pr.Price);

                ViewData["showItems"] = "bloc";
                ViewData["cartItems"] = sb.ToString().Trim();
                ViewData["totalCost"] = $"{totalPrice:F2}";
            }

            ViewData["title"] = "My Cart";

            return View("shopping/cart");
        }

        [HttpPost("/shopping/order")]
        public IHttpResponse FinishOrder()
        {
            if (!IsAuthenticated())
            {
                ViewData["visible"] = "bloc";
                ViewData["title"] = "Login";
                return View("account/login");
            }

            var username = User;

            if (username == null)
            {
                ViewData["title"] = "Login";
                return View("account/login");
            }

            var shoppingCart = Request.Session.GetParameter<ShoppingCartViewModel>(ShoppingCartViewModel.SessionKey);

            var userId = _user.GetUserId(username);

            var productIds = shoppingCart.ProductIds;
            if (!productIds.Any())
            {
                return Redirect("/");
            }

            if (userId != null) _shopping.CreateOrder(userId.Value, productIds);

            shoppingCart.ProductIds.Clear();

            ViewData["title"] = "Order Finished";

            return View("shopping/order");
        }

        [HttpGet("/shopping/my-orders")]
        public IHttpResponse MyOrders()
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

                var orders = Db.Orders
                    .Where(o => o.User.Username.Equals(username))
                    .Select(o => new OrderViewModel
                    {
                        Id = o.Id,
                        DateOfCreation = o.DateOfCreation,
                        Products = o.Products
                    })
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

                return View("shopping/my-orders");
            }
        }

        [HttpGet("/list")]
        public IHttpResponse Details(OrderInputModel model)
        {
            if (!IsAuthenticated())
            {
                ViewData["visible"] = "bloc";
                ViewData["title"] = "Login";
                return View("account/login");
            }
            
            var username = User;

            using (Db)
            {
                var order = Db.Orders.FirstOrDefault(o => o.Id == model.Id && o.User.Username.Equals(username));

                if (order == null)
                {
                    return BadRequestError("This order not exists!");
                }

                var products = order.Products
                    .Select(o => new ProductViewModel()
                    {
                        Id = o.ProductId,
                        Name = o.Product.Name,
                        Price = o.Product.Price
                    })
                    .ToList();

                var result = new StringBuilder();

                foreach (var productViewModel in products)
                {
                    result.AppendLine($@"<tr><td><a class=""btn"" href=""/details?id={productViewModel.Id}"">{productViewModel.Name}</a></td><td>${productViewModel.Price}</td></tr>");
                }

                ViewData["products"] = result.ToString().Trim();
                ViewData["orderId"] = model.Id.ToString();
                ViewData["orderDate"] = order.DateOfCreation.ToString("dd-MM-yyyy");
                ViewData["title"] = $"Order {model.Id} Details";

                return View("shopping/details");
            }
        }
    }
}
