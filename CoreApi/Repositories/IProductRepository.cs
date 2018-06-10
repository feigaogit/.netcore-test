using CoreApi.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoreApi.Repositories
{
    public interface IProductRepository
    {
        //如果返回的是IQueryable，那么调用repository的地方还可以继续构建IQueryable，例如在真正的查询执行之前附加一个OrderBy或者Where方法。
        //但是这样做的话，也意味着你把持久化相关的代码给泄露出去了，这看起来是违反了repository pattern的目的。
        IEnumerable<Product> GetProducts();
        Product GetProduct(int productId, bool includeMaterials = false);
        IEnumerable<Material> GetMaterialsForProduct(int productId);
        Material GetMaterialForProduct(int productId, int materialId);
        bool ProductExist(int productId);
        void AddProduct(Product product);
        bool Save();
        void DeleteProduct(Product product);
    }
}
