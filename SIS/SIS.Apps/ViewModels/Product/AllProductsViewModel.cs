namespace CakesWebApp.ViewModels.Product
{
    using System.Collections.Generic;

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
