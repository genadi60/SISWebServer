using System.Collections.Generic;
using CakesWebApp.ViewModels.Product;

namespace CakesWebApp.InputModels.Product
{
    public class SearchInputModel
    {
        public SearchInputModel()
        {
            ProductViewModels = new List<ProductViewModel>();
        }

        public string Title => "Browse Cake";

        public string SearchTerm { get; set; }

        public ICollection<ProductViewModel> ProductViewModels { get; set; }

        public int ProductsInCartCount { get; set; }

        public string TextForCount { get; set; }
    }
}
