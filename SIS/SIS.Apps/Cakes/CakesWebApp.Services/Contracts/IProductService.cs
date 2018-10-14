namespace CakesWebApp.Services.Contracts
{
    using System.Collections.Generic;

    using ViewModels.Product;

    public interface IProductService
    {
        void Create(AddProductViewModel model);

        ICollection<ProductListingViewModel> All(string searchTerm = null);

        ICollection<ProductShowViewModel> ShowAll();

        ProductDetailsViewModel Find(int id);

        bool Exists(int id);

        ICollection<ProductInCartViewModel> FindProductsInCart(IEnumerable<int> ids);
    }
}
