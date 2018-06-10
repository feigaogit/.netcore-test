using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using CoreApi.Dtos;
using CoreApi.Entities;
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
            //var results = new List<ProductWithoutMaterialDto>();
            //foreach (var product in products)
            //{
            //    results.Add(new ProductWithoutMaterialDto
            //    {
            //        Id = product.Id,
            //        Name = product.Name,
            //        Price = product.Price,
            //        Description = product.Description
            //    });
            //}
            //使用autoMapper
            var results = AutoMapper.Mapper.Map<IEnumerable<ProductWithoutMaterialDto>>(products);

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
                //var productWithMaterialResult = new ProductDto
                //{
                //    Id = product.Id,
                //    Name = product.Name,
                //    Price = product.Price,
                //    Description = product.Description
                //};

                //foreach (var material in product.Materials)
                //{
                //    productWithMaterialResult.Materials.Add(new MaterialDto
                //    {
                //        Id = material.Id,
                //        Name = material.Name
                //    });
                //}

                //使用automapper
                var productWithMaterialResult = AutoMapper.Mapper.Map<ProductDto>(product);

                return Ok(productWithMaterialResult);
            }

            //var onlyProductResult = new ProductDto
            //{
            //    Id = product.Id,
            //    Name = product.Name,
            //    Price = product.Price,
            //    Description = product.Description
            //};

            //使用automapper
            var onlyProductResult = AutoMapper.Mapper.Map<ProductWithoutMaterialDto>(product);
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

            ////获取目前存在的最大id
            //var maxId = ProductService.Current().Products.Max(t => t.Id);

            //var newProduct = new ProductDto
            //{
            //    Id = ++maxId,
            //    Name = product.Name,
            //    Price = product.Price,
            //    Description = product.Description
            //};

            //ProductService.Current().Products.Add(newProduct);

            var newProduct = Mapper.Map<Product>(product);
            _productRepository.AddProduct(newProduct);
            if (!_productRepository.Save())
            {
                return StatusCode(500, "保存产品的时候出错");
            }

            var dto = Mapper.Map<ProductWithoutMaterialDto>(newProduct);

            //return CreatedAtRoute("GetProduct", new { id = newProduct.Id }, newProduct);
            return CreatedAtRoute("GetProduct", new { id = dto.Id }, dto);
        }

        [HttpPut("{id}")]
        public IActionResult Put(int id, [FromBody] ProductModification productModificationDto)
        {
            if (productModificationDto == null)
            {
                return BadRequest();
            }

            //Data Annotation 
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            //复杂验证，自定义
            if (productModificationDto.Name == "name")
            {
                ModelState.AddModelError("Name", "产品名称不能是‘name’");
                return BadRequest(ModelState);
            }

            var product = _productRepository.GetProduct(id);
            if (product == null)
            {
                return NotFound();
            }

            //把第一个对象相应的值赋给第二个对象上。这时候product的state就自动变成了modified了，所以直接sava即可。
            Mapper.Map(productModificationDto, product);
            if (!_productRepository.Save())
            {
                return StatusCode(500, "保存产品的时候出错");
            }
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

            var model = _productRepository.GetProduct(id);
            if (model == null)
            {
                return NotFound();
            }

            var toPatch = Mapper.Map<ProductModification>(model);
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

            Mapper.Map(toPatch, model);
            if (!_productRepository.Save())
            {
                return StatusCode(500, "更新的时候出错");
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var model = _productRepository.GetProduct(id);
            if (model == null)
            {
                return NotFound();
            }

            _productRepository.DeleteProduct(model);
            if (!_productRepository.Save())
            {
                return StatusCode(500, "删除的时候出错");
            }

            _localMailService.Send("Product Deleted", $"Id为{id}的产品被删除了");

            return NoContent();
        }
    }
}