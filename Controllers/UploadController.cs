using Microsoft.AspNetCore.Mvc;

namespace ushopDN.Controllers
{
    [Route("api/v1/upload")]
    [ApiController]
    public class UploadController(IWebHostEnvironment env, IConfiguration configuration) : ControllerBase
    {
        private readonly IWebHostEnvironment _env = env;
        private readonly IConfiguration _configuration = configuration;

        [HttpPost]
        public async Task<IActionResult> UploadFile(IFormFile product)
        {
            if (product.Length == 0)
                return BadRequest(new { success = 0, message = "No file uploaded" });

            var fileName = $"{product.Name}_{DateTime.Now.Ticks}{Path.GetExtension(product.FileName)}";
            var filePath = Path.Combine(_env.WebRootPath, "images", fileName);

            await using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await product.CopyToAsync(stream);
            }

            var isDev = _configuration["ServeMode"] == "dev";
            var imageUrl = isDev
                ? $"http://localhost:{_configuration["Port"]}/images/{fileName}"
                : $"https://ushop.cws-project.site/images/{fileName}";

            return Ok(new { success = 1, image_url = imageUrl });
        }
    }
}
