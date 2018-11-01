namespace CakesWebApp.Controllers
{
    using System.Linq;
    using System.Text;
    
    using InputModels.Product;
    using Services;
    using Services.Contracts;
    using SIS.HTTP.Responses.Contracts;
    using SIS.MvcFramework.Attributes;
    using SIS.MvcFramework.ViewModels;
    using ViewModels;
    using ViewModels.Home;
    using ViewModels.Product;
    using ViewModels.Shopping;
    
    public class CakeController : BaseController
    {
        private readonly IProductService _productService;

        public CakeController(ProductService productService)
        {
            _productService = productService;
        }

        [HttpGet("/product/add")]
        public IHttpResponse AddCake(AddProductInputModel model)
        {
            return View("product/add", model);
        }

        [HttpPost("/product/add")]
        public IHttpResponse DoAddCake(AddProductInputModel model)
        {
            if (string.IsNullOrWhiteSpace(model.Name))
            {
                var errorMessage = "Please, provide valid product name.";
                return View("/error", new ErrorViewModel(errorMessage));
            }

            _productService.Create(model, Db);
            
            return View("product/add", model);
        }

        [HttpGet("/product/search")]
        public IHttpResponse Search(SearchInputModel model)
        {
            var shoppingCart = Request.Session.GetParameter<ShoppingCartViewModel>(ShoppingCartViewModel.SessionKey);

            var totalProducts = shoppingCart.ProductIds.Count;
            var totalProductsText = totalProducts != 1 ? "products" : "product";

            model.ProductsInCartCount = totalProducts;
            model.TextForCount = totalProductsText;

            model.ProductViewModels = _productService.SearchCakes(Db, model.SearchTerm);

            return View("/product/search", model);
        }

        [HttpGet("/product/cakeDetails")]
        public IHttpResponse CakeDetails(ProductViewModel model)
        {
            if (model.Id < 1)
            {
                var errorMessage = "Invalid request.";
                return View("/error", new ErrorViewModel(errorMessage));
            }

           var cake = _productService.Find(model.Id, Db);
            
            return View("product/cakeDetails", cake);
        }

        [HttpGet("/Product/Cakes")]
        public IHttpResponse GetCakes(AllProductsViewModel model)
        {
            var cakes = _productService.ShowAll(Db);

            if (cakes == null || cakes.Count == 0)
            {
                var textModel = new TextViewModel
                {
                    Message = "We are Sorry! No Cakes in Store.",
                    Title = "No Cakes"
                };
                return View("/text", textModel);
            }
            
            foreach (var cake in cakes)
            {
                model.AddViewModel(cake);
            }

            return View("/product/cakes", model);
        }
    }
}