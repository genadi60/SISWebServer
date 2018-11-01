using System.Collections.Generic;
using System.Linq;
using CakesWebApp.Data;
using CakesWebApp.InputModels.Product;
using CakesWebApp.Models;
using CakesWebApp.Services.Contracts;
using CakesWebApp.ViewModels.Product;

namespace CakesWebApp.Services
{
    public class ProductService : IProductService
    {
        public void Create(AddProductInputModel model, CakesDbContext context)
        {
            using (var db = context)
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
        
        public ICollection<ProductViewModel> SearchCakes(CakesDbContext context, string searchTerm = null)
        {
            using (var db = context)
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

        public ProductDetailsViewModel Find(int id, CakesDbContext context)
        {
            using (var db = context)
            {
                return db.Products
                    .Where(pr => pr.Id == id)
                    .Select(pr => new ProductDetailsViewModel
                    {
                        Id = pr.Id,
                        Name = pr.Name,
                        Price = pr.Price,
                        ImageUrl = pr.ImageUrl
                    })
                    .FirstOrDefault();
            }
        }

        public bool Exists(int id, CakesDbContext context)
        {
            using (var db = context)
            {
                return db.Products.Any(pr => pr.Id == id);
            }
        }

        public ICollection<ProductViewModel> FindProductsInCart(IEnumerable<int> ids, CakesDbContext context)
        {
            using (var db = context)
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

        public ICollection<ProductDetailsViewModel> ShowAll(CakesDbContext context)
        {
            using (var db = context)
            {
                return db.Products
                    .Select(pr => new ProductDetailsViewModel
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
