﻿namespace CakesWebApp.Services
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;

    using Contracts;
    using Data;
    using Models;
    using ViewModels.Product;

    public class ProductService : IProductService
    {
        public void Create(AddProductViewModel model)
        {
            using (var db = new CakesDbContext())
            {
                var product = new Product
                {
                    Name = model.Name,
                    Price = decimal.Parse(model.Price),
                    ImageUrl = WebUtility.HtmlDecode(model.ImageUrl)
                };

                db.Add(product);
                db.SaveChanges();
            }
        }
        
        public ICollection<ProductListingViewModel> All(string searchTerm = null)
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
                    .Select(pr => new ProductListingViewModel
                    {
                        Id = pr.Id.ToString(),
                        Name = pr.Name,
                        Price = pr.Price.ToString()
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
                        Price = pr.Price.ToString(),
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

        public ICollection<ProductInCartViewModel> FindProductsInCart(IEnumerable<int> ids)
        {
            using (var db = new CakesDbContext())
            {
                var products = db.Products.ToList();
                var productsInCart = new List<ProductInCartViewModel>();

                foreach (var id in ids)
                {
                    var productViewInCart = products
                        .Where(p => p.Id == id)
                        .Select(p => new ProductInCartViewModel
                        {
                            Id = p.Id.ToString(),
                            Name = p.Name,
                            Price = p.Price.ToString()
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
                        Id = pr.Id.ToString(),
                        Name = pr.Name,
                        ImageUrl = pr.ImageUrl
                    })
                    .ToList();
            }
        }
    }
}
