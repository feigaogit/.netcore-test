using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CoreApi.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CoreApi.Controllers
{
    [Produces("application/json")]
    [Route("api/product")]
    public class MaterialController : Controller
    {
        [Route("{productId}/materials")]
        public IActionResult GetMaterials(int productId)
        {
            var product = ProductService.Current().Products.SingleOrDefault(t => t.Id == productId);
            if (product == null)
            {
                return NotFound();
            }

            return Ok(product.Materials);
        }

        [Route("{productId}/materials/{id}")]
        public IActionResult GetMaterila(int productId, int id)
        {
            var product = ProductService.Current().Products.SingleOrDefault(t => t.Id == productId);
            if (product == null)
            {
                return NotFound();
            }

            var materila = product.Materials.SingleOrDefault(t => t.Id == id);
            if (materila == null)
            {
                return NotFound();
            }

            return Ok(materila);
        }
    }
}