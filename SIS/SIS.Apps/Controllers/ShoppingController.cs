using CakesWebApp.InputModels.Account;
using CakesWebApp.ViewModels;
using CakesWebApp.ViewModels.Home;
using CakesWebApp.ViewModels.User;
using SIS.MvcFramework.ViewModels;

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
            var id = model.Id;

            var shoppingCart = Request.Session.GetParameter<ShoppingCartViewModel>(ShoppingCartViewModel.SessionKey);
            var cartProductIds = shoppingCart.ProductIds;

            cartProductIds.Add(id);

            var cart = new ProductsInCartViewModel
            {
                ProductViewModels = _product.FindProductsInCart(cartProductIds, Db)
            };

            return View("/shopping/cart", cart);

        }

        [HttpGet("/shopping/cart")]
        public IHttpResponse ShowCart(ClearProductInputModel model)
        {
            var shoppingCart = Request.Session.GetParameter<ShoppingCartViewModel>(ShoppingCartViewModel.SessionKey);

            if (model.ClearProduct > 0)
            {
                var clearedProductId = model.ClearProduct;

                var indexToClear = shoppingCart.ProductIds.IndexOf(clearedProductId);

                shoppingCart.ProductIds.RemoveAt(indexToClear);

                Request.QueryData.Remove("clearProduct");
            }

            var cart = new ProductsInCartViewModel
            {
                ProductViewModels = _product.FindProductsInCart(shoppingCart.ProductIds, Db)
            };

            return View("/shopping/cart", cart);
        }

        [HttpPost("/shopping/finishOrder")]
        public IHttpResponse FinishOrder()
        {
            var username = User;
            
            if (username == null)
            {
               return View("account/login", new LoginInputModel());
            }

            var shoppingCart = Request.Session.GetParameter<ShoppingCartViewModel>(ShoppingCartViewModel.SessionKey);

            var userId = _user.GetUserId(username, Db);

            var productIds = shoppingCart.ProductIds;
            
            _shopping.CreateOrder(userId, productIds, Db);

            shoppingCart.ProductIds.Clear();

            
            return View("/text", new TextViewModel());
        }

        [HttpGet("/shopping/details")]
        public IHttpResponse Details(OrderViewModel model)
        {
            var orderViewModel = _shopping.GetOrderViewModel(model.Id, Db);
            
            if (orderViewModel == null)
            {
                var errorMessage = "This order not exists!";
                return View("/error", new ErrorViewModel(errorMessage));
            }

            return View("/shopping/orderDetails", orderViewModel);
        }
    }
}
