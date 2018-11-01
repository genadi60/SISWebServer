namespace CakesWebApp.Services.Contracts
{
    using System.Collections.Generic;

    using Data;
    using InputModels.Product;
    using ViewModels.Product;

    public interface IProductService
    {
        void Create(AddProductInputModel model, CakesDbContext context);

        ICollection<ProductViewModel> SearchCakes(CakesDbContext context, string searchTerm = null);

        ICollection<ProductDetailsViewModel> ShowAll(CakesDbContext context);

        ProductDetailsViewModel Find(int id, CakesDbContext context);

        bool Exists(int id, CakesDbContext context);

        ICollection<ProductViewModel> FindProductsInCart(IEnumerable<int> ids, CakesDbContext context);
    }
}
