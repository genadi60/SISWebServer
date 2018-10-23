namespace CakesWebApp.Services
{
    using System.Collections.Generic;
    using System.Linq;

    using Contracts;
    using Data;
    using InputModels.Product;
    using Models;
    using ViewModels.Product;
    
    public class ProductService : IProductService
    {
        public void Create(AddProductInputModel model)
        {
            using (var db = new CakesDbContext())
            {
                var product = new Product
                {
                    Name = model.Name,
                    Price = model.Price,
                    ImageUrl = model.ImageUrl
                };

                db.Add(product);
                db.SaveChanges();
            }
        }
        
        public ICollection<ProductViewModel> All(string searchTerm = null)
        {
            using (var db = new CakesDbContext())
            {
                var resultsQuery = db.Products.AsQueryable();

                if (!string.IsNullOrEmpty(searchTerm))
                {
                    resultsQuery = resultsQuery
                        .Where(pr => pr.Name.ToLower().Contains(searchTerm.ToLower()));
                }

                return resultsQuery
                    .Select(pr => new ProductViewModel
                    {
                        Id = pr.Id,
                        Name = pr.Name,
                        Price = pr.Price
                    })
                    .ToList();
            }

        }

        public ProductDetailsViewModel Find(int id)
        {
            using (var db = new CakesDbContext())
            {
                return db.Products
                    .Where(pr => pr.Id == id)
                    .Select(pr => new ProductDetailsViewModel
                    {
                        Name = pr.Name,
                        Price = pr.Price,
                        ImageUrl = pr.ImageUrl
                    })
                    .FirstOrDefault();
            }
        }

        public bool Exists(int id)
        {
            using (var db = new CakesDbContext())
            {
                return db.Products.Any(pr => pr.Id == id);
            }
        }

        public ICollection<ProductViewModel> FindProductsInCart(IEnumerable<int> ids)
        {
            using (var db = new CakesDbContext())
            {
                var products = db.Products.ToList();
                var productsInCart = new List<ProductViewModel>();

                foreach (var id in ids)
                {
                    var productViewInCart = products
                        .Where(p => p.Id == id)
                        .Select(p => new ProductViewModel
                        {
                            Id = p.Id,
                            Name = p.Name,
                            Price = p.Price
                        })
                        .FirstOrDefault();

                    productsInCart.Add(productViewInCart);

                }

                return productsInCart;
            }
        }

        public ICollection<ProductShowViewModel> ShowAll()
        {
            using (var db = new CakesDbContext())
            {
                return db.Products
                    .Select(pr => new ProductShowViewModel
                    {
                        Id = pr.Id,
                        Name = pr.Name,
                        ImageUrl = pr.ImageUrl
                    })
                    .ToList();
            }
        }
    }
}
