using BusinessObjects;
using System.Collections.Generic;
using DataAccessLayer;

namespace Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly ProductDAO _productDAO = new ProductDAO();

        public void DeleteProduct(Product p) => _productDAO.DeleteProduct(p);
        public void SaveProduct(Product p) => _productDAO.SaveProduct(p);
        public void UpdateProduct(Product p) => _productDAO.UpdateProduct(p);
        public List<Product> GetProducts() => _productDAO.GetProduct();
        public Product GetProductById(int id) => _productDAO.GetProductById(id);
    }
}