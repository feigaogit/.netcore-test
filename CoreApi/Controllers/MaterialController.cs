using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using CoreApi.Dtos;
using CoreApi.Repositories;
using CoreApi.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CoreApi.Controllers
{
    [Produces("application/json")]
    [Route("api/product")]
    public class MaterialController : Controller
    {
        private readonly IProductRepository _productRepository;

        public MaterialController(IProductRepository productRepository)
        {
            this._productRepository = productRepository;
        }

        [Route("{productId}/materials")]
        public IActionResult GetMaterials(int productId)
        {
            var isExist = _productRepository.ProductExist(productId);
            if (!isExist)
            {
                return NotFound();
            }

            var materials = _productRepository.GetMaterialsForProduct(productId);
            //var results = materials.Select(material => new MaterialDto
            //{
            //    Id = material.Id,
            //    Name = material.Name
            //}).ToList();
            //使用automapper
            var results = Mapper.Map<IEnumerable<MaterialDto>>(materials);

            return Ok(results);
        }

        [Route("{productId}/materials/{id}")]
        public IActionResult GetMateril(int productId, int id)
        {
            var isExist = _productRepository.ProductExist(productId);
            if (!isExist)
            {
                return NotFound();
            }

            var material = _productRepository.GetMaterialForProduct(productId, id);
            if (material == null)
            {
                return NotFound();
            }
            //var result = new MaterialDto
            //{
            //    Id = material.Id,
            //    Name = material.Name
            //};
            var result = Mapper.Map<MaterialDto>(material);
            return Ok(result);
        }
    }
}