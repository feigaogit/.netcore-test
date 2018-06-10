using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CoreApi.Entities;
using Microsoft.EntityFrameworkCore;

namespace CoreApi.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly MyContext _myContext;

        public ProductRepository(MyContext myContext)
        {
            _myContext = myContext;
        }

        public Material GetMaterialForProduct(int productId, int materialId)
        {
            return _myContext.Materials.FirstOrDefault(x => x.ProductId == productId && x.Id == materialId);
        }

        public IEnumerable<Material> GetMaterialsForProduct(int productId)
        {
            return _myContext.Materials.Where(x => x.ProductId == productId).ToList();
        }

        public Product GetProduct(int productId, bool includeMaterials = false)
        {
            if (includeMaterials)
            {
                return _myContext.Products.Include(t => t.Materials).FirstOrDefault(t => t.Id == productId);
            }

            return _myContext.Products.Find(productId);
        }

        public IEnumerable<Product> GetProducts()
        {
            return _myContext.Products.OrderBy(t => t.Name).ToList();
        }

        /// <summary>
        /// 产品是否存在
        /// </summary>
        /// <param name="productID"></param>
        /// <returns></returns>
        public bool ProductExist(int productID)
        {
            return _myContext.Products.Any(x => x.Id == productID);
        }

        /// <summary>
        /// 添加
        /// </summary>
        /// <param name="product"></param>
        public void AddProduct(Product product)
        {
            _myContext.Products.Add(product);
        }

        /// <summary>
        /// 保存更改
        /// </summary>
        /// <returns></returns>
        public bool Save()
        {
            return _myContext.SaveChanges() >= 0;
        }
        public void DeleteProduct(Product product)
        {
            _myContext.Products.Remove(product);
        }
    }
}
