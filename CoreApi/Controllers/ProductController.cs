using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CoreApi.Dtos;
using CoreApi.Repositories;
using CoreApi.Services;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace CoreApi.Controllers
{
    [Route("api/[controller]")]
    public class ProductController : Controller
    {
        private ILogger<ProductController> _logger;
        private readonly IMailService _localMailService;
        private readonly IProductRepository _productRepository;

        public ProductController(ILogger<ProductController> logger, IMailService localMailService, IProductRepository productRepository)
        {
            this._logger = logger;
            this._localMailService = localMailService;
            this._productRepository = productRepository;
        }

        [HttpGet]
        public IActionResult GetProducts()
        {
            var products = _productRepository.GetProducts();
            var results = new List<ProductWithoutMaterialDto>();
            foreach (var product in products)
            {
                results.Add(new ProductWithoutMaterialDto
                {
                    Id = product.Id,
                    Name = product.Name,
                    Price = product.Price,
                    Description = product.Description
                });
            }

            return Ok(results);
        }

        [HttpGet]
        [Route("{id}", Name = "GetProduct")]
        public IActionResult GetProduct(int id, bool includeMaterial = false)
        {
            var product = _productRepository.GetProduct(id, includeMaterial);
            if (product == null)
            {
                return NotFound();
            }

            if (includeMaterial)
            {
                var productWithMaterialResult = new ProductDto
                {
                    Id = product.Id,
                    Name = product.Name,
                    Price = product.Price,
                    Description = product.Description
                };

                foreach (var material in product.Materials)
                {
                    productWithMaterialResult.Materials.Add(new MaterialDto
                    {
                        Id = material.Id,
                        Name = material.Name
                    });
                }

                return Ok(productWithMaterialResult);
            }

            var onlyProductResult = new ProductDto
            {
                Id = product.Id,
                Name = product.Name,
                Price = product.Price,
                Description = product.Description
            };
            return Ok(onlyProductResult);
        }

        [HttpPost]
        public IActionResult Post([FromBody]ProductCreation product)
        {
            if (product == null)
            {
                return BadRequest();
            }

            //Data Annotation 
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            //复杂验证，自定义
            if (product.Name == "name")
            {
                ModelState.AddModelError("Name", "产品名称不能是‘name’");
                return BadRequest(ModelState);
            }

            //获取目前存在的最大id
            var maxId = ProductService.Current().Products.Max(t => t.Id);

            var newProduct = new ProductDto
            {
                Id = ++maxId,
                Name = product.Name,
                Price = product.Price,
                Description = product.Description
            };

            ProductService.Current().Products.Add(newProduct);

            return CreatedAtRoute("GetProduct", new { id = newProduct.Id }, newProduct);
        }

        [HttpPut("{id}")]
        public IActionResult Put(int id, [FromBody] ProductModification product)
        {
            if (product == null)
            {
                return BadRequest();
            }

            //Data Annotation 
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            //复杂验证，自定义
            if (product.Name == "name")
            {
                ModelState.AddModelError("Name", "产品名称不能是‘name’");
                return BadRequest(ModelState);
            }

            var model = ProductService.Current().Products.SingleOrDefault(t => t.Id == id);
            if (model == null)
            {
                return NotFound();
            }

            model.Name = product.Name;
            model.Price = product.Price;
            model.Description = product.Description;

            //return Ok();
            //PUT建议返回NoContent()
            return NoContent();
        }

        [HttpPatch("{id}")]
        public IActionResult Patch(int id, [FromBody] JsonPatchDocument<ProductModification> patchBoc)
        {
            if (patchBoc == null)
            {
                return BadRequest();
            }

            var model = ProductService.Current().Products.SingleOrDefault(t => t.Id == id);
            if (model == null)
            {
                return NotFound();
            }

            var toPatch = new ProductModification
            {
                Name = model.Name,
                Description = model.Description,
                Price = model.Price
            };

            patchBoc.ApplyTo(toPatch, ModelState);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (toPatch.Name == "产品")
            {
                ModelState.AddModelError("Name", "产品的名称不可以是'产品'二字");
            }
            TryValidateModel(toPatch);
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            model.Name = toPatch.Name;
            model.Description = toPatch.Description;
            model.Price = toPatch.Price;

            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var model = ProductService.Current().Products.SingleOrDefault(t => t.Id == id);

            if (model == null)
            {
                return NotFound();
            }

            ProductService.Current().Products.Remove(model);

            _localMailService.Send("Product Deleted", $"Id为{id}的产品被删除了");

            return NoContent();
        }
    }
}