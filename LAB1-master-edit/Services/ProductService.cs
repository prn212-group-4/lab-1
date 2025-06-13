using BusinessObjects;
using Repositories;
using System.Collections.Generic;

namespace Services
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository iproductRepository;

        public ProductService()
        {
            iproductRepository = new ProductRepository();
        }

        public void DeleteProduct(Product p)
        {
            iproductRepository.DeleteProduct(p);
        }

        public Product GetProductById(int id)
        {
            return iproductRepository.GetProductById(id);
        }

        public List<Product> GetProducts()
        {
            return iproductRepository.GetProducts();
        }

        public void SaveProduct(Product p)
        {
            iproductRepository.SaveProduct(p);
        }

        public void UpdateProduct(Product p)
        {
            iproductRepository.UpdateProduct(p);
        }
    }
}