namespace CakesWebApp.ViewModels.User
{
    using System.Collections.Generic;
    using System.Linq;

    using Product;

    public class ProductsInCartViewModel
    {
        public ProductsInCartViewModel()
        {
            ProductViewModels = new List<ProductViewModel>();
        }

        public string Title => "Your Cart";

        public decimal TotalCosts => ProductViewModels.Sum(p => p.Price);

        public ICollection<ProductViewModel> ProductViewModels { get; set; }

        public void Add(ProductViewModel model)
        {
            ProductViewModels.Add(model);
        }
    }
}
