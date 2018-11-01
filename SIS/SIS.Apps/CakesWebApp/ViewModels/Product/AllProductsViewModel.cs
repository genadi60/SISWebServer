using System.Collections.Generic;

namespace CakesWebApp.ViewModels.Product
{
    public class AllProductsViewModel
    {

        public AllProductsViewModel()
        {
            AllProducts = new List<ProductDetailsViewModel>();
        }

        public string Title { get; set; } = "The Cake";
        public ICollection<ProductDetailsViewModel> AllProducts { get; set; }

        public void AddViewModel(ProductDetailsViewModel model)
        {
            AllProducts.Add(model);
        }
    }
}
