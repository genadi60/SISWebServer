namespace CakesWebApp.Services.Contracts
{
    using System.Collections.Generic;

    using InputModels.Product;
    using ViewModels.Product;

    public interface IProductService
    {
        void Create(AddProductViewModel model);

        ICollection<ProductViewModel> All(string searchTerm = null);

        ICollection<ProductShowViewModel> ShowAll();

        ProductDetailsViewModel Find(int id);

        bool Exists(int id);

        ICollection<ProductViewModel> FindProductsInCart(IEnumerable<int> ids);
    }
}
