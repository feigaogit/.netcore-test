using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
            var materials = _productRepository.GetMaterialsForProduct(productId);
            var results = materials.Select(material => new MaterialDto
            {
                Id = material.Id,
                Name = material.Name
            }).ToList();

            return Ok(results);
        }

        [Route("{productId}/materials/{id}")]
        public IActionResult GetMaterila(int productId, int id)
        {
            var material = _productRepository.GetMaterialForProduct(productId, id);
            if (material == null)
            {
                return NotFound();
            }
            var result = new MaterialDto
            {
                Id = material.Id,
                Name = material.Name
            };
            return Ok(result);
        }
    }
}