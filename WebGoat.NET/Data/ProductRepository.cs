using WebGoatCore.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace WebGoatCore.Data
{
    public class ProductRepository
    {
        private readonly NorthwindContext _context;

        public ProductRepository(NorthwindContext context)
        {
            _context = context;
        }

        public Product GetProductById(int productId)
        {
            return _context.Products.Single(p => p.ProductId == productId);
        }

        public List<Product> GetTopProducts(int NOPTR)
        {
            var orderDate = DateTime.Today.AddMonths(-1);
            var topProducts = _context.Orders
                .Where(o => o.OrderDate > orderDate)
                .Join(_context.OrderDetails, o => o.OrderId, od => od.OrderId, (o, od) => od)
                // Turn this query to standard LINQ expression, because EF Core can't handle the remaining part
                .AsEnumerable()
                .GroupBy(od => od.Product)
                .OrderByDescending(g => g.Sum(t => t.UnitPrice * t.Quantity))
                .Select(g => g.Key)
                .Take(NOPTR)
                .ToList();

            if(topProducts.Count < 4)
            {
                topProducts.AddRange(_context.Products
                    .OrderByDescending(p => p.UnitPrice)
                    .Take(NOPTR - topProducts.Count)
                    .ToList());
            }

            return topProducts;
        }

        public List<Product> GetAllProducts()
        {
            return _context.Products.OrderBy(p => p.ProductName).ToList();
        }

        public List<Product> FindNonDiscontinuedProducts(string? pN, int? cI)
        {
            var p = _context.Products.Where(p => !p.Discontinued);

            if (cI != null)
            {
                p = p.Where(p => p.CategoryId == cI);
            }
            if (pN != null)
            {
                 return p.ToList().Where(p => p.ProductName.Contains(pN, StringComparison.CurrentCultureIgnoreCase)).OrderBy(p => p.ProductName).ToList();
            }

            return p.OrderBy(p => p.ProductName).ToList();
        }

        public Product Update(Product p)
        {
            p = _context.Products.Update(p).Entity;
            _context.SaveChanges();
            return p;
        }

        public void Add(Product p)
        {
            _context.Products.Add(p);
            _context.SaveChanges();
        }
    }
}
