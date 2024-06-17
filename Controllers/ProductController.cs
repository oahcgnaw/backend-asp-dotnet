using Microsoft.AspNetCore.Mvc;
using ushopDN.Data;
using ushopDN.Models;
using MongoDB.Driver;

namespace ushopDN.Controllers
{
    [Route("api/v1/")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ProductController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> PostProduct([FromBody] Product product)
        {
            await _context.Products.InsertOneAsync(product);
            return Ok(new { success = true, name = product.Name });
        }

        [HttpGet("product/{id}")]
        public async Task<IActionResult> GetProduct(string id)
        {
            var product = await _context.Products.Find(p => p.Id == id).FirstOrDefaultAsync();
            if (product == null)
            {
                return NotFound(new { success = false, message = "Product not found" });
            }
            return Ok(product);
        }

        [HttpDelete("product/{id}")]
        public async Task<IActionResult> DeleteProduct(string id)
        {
            var result = await _context.Products.DeleteOneAsync(p => p.Id == id);
            if (result.DeletedCount == 0)
            {
                return NotFound(new { success = false, message = "Product not found" });
            }
            return Ok(new { success = true });
        }

        [HttpGet("product")]
        public async Task<IActionResult> GetProducts()
        {
            var products = await _context.Products.Find(_ => true).ToListAsync();
            return Ok(products);
        }

        [HttpGet("product/category/{category}")]
        public async Task<IActionResult> GetProductsByCategory(string category, [FromQuery] int page = 1)
        {
            const int PageSize = 10;
            var products = await _context.Products
                .Find(p => p.Category == category)
                .Skip(PageSize * (page - 1))
                .Limit(PageSize)
                .ToListAsync();

            var total = await _context.Products.CountDocumentsAsync(p => p.Category == category);
            return Ok(new { products, total, page });
        }

        [HttpGet("popular/{category}")]
        public async Task<IActionResult> GetPopularProduct(string category)
        {
            var products = await _context.Products
                .Find(p => p.Category == category)
                .Limit(4)
                .ToListAsync();
            return Ok(products);

        }

        [HttpGet("newcollections")]
        public async Task<IActionResult> GetNewCollections()
        {
            var products = await _context.Products
                .Find(_ => true)
                .SortByDescending(p => p.Date)
                .Limit(8)
                .ToListAsync();
            return Ok(products);
        }

        [HttpGet("newcollections/{category}")]
        public async Task<IActionResult> GetNewCollectionsByCategory(string category)
        {
            var products = await _context.Products
                .Find(p => p.Category == category)
                .SortByDescending(p => p.Date)
                .Limit(4)
                .ToListAsync();
            return Ok(products);
        }

    }
}
