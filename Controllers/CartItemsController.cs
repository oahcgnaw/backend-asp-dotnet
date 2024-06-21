using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using ushopDN.Data;
using ushopDN.Models;
using MongoDB.Driver;
using ushopDN.Services;

namespace ushopDN.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class CartItemsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly JwtService _jwtService;
        private readonly ILogger<CartItemsController> _logger;

        public CartItemsController(ApplicationDbContext context, JwtService jwtService, ILogger<CartItemsController> logger)
        {
            _context = context;
            _jwtService = jwtService;
            _logger = logger;
        }


        private async Task<User?> GetUserByToken(string token)
        {
            if (string.IsNullOrEmpty(token))
                return null;

            var claims = _jwtService.ValidateToken(token);
            if (claims == null)
                return null;

            var userId = claims.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return null;

            return await _context.Users.Find(u => u.Id == userId).FirstOrDefaultAsync();
        }

        [HttpPost]
        public async Task<IActionResult> GetCartItems()
        {
            var token = Request.Headers["Auth-Token"].ToString();
            _logger.LogInformation($"Received request to get cart items.");

            var user = await GetUserByToken(token);
            if (user == null)
                return NotFound(new { error = "User not found" });

            return Ok(new { cartData = user.CartData });
        }

        [HttpPatch]
        public async Task<IActionResult> AddToCart([FromBody] Dictionary<string, string> body)
        {
            var token = Request.Headers["Auth-Token"].ToString();


            var user = await GetUserByToken(token);
            if (user == null)
                return NotFound(new { error = "User not found" });

            string itemID = body["itemID"];
            _logger.LogInformation($"Received request to add item to cart, itemId: {itemID}.");


            var cartData = user.CartData ?? new Dictionary<string, int>();

            cartData[itemID] = cartData.TryGetValue(itemID, out int value) ? value + 1 : 1;

            var update = Builders<User>.Update.Set(u => u.CartData, cartData);
            await _context.Users.UpdateOneAsync(u => u.Id == user.Id, update);

            return Ok(new { success = true, message = $"Item {itemID} added to cart" });
        }

        [HttpDelete("{itemId}")]
        public async Task<IActionResult> DeleteFromCart(string itemId)
        {
            var token = Request.Headers["Auth-Token"].ToString();
            _logger.LogInformation($"Received request to delete item from cart, itemId: {itemId}.");

            var user = await GetUserByToken(token);
            if (user == null)
                return NotFound(new { error = "User not found" });

            var cartData = user.CartData ?? new Dictionary<string, int>();
            if (cartData.ContainsKey(itemId))
            {
                cartData[itemId] -= 1;
                if (cartData[itemId] <= 0)
                {
                    cartData.Remove(itemId);
                }
            }

            var update = Builders<User>.Update.Set(u => u.CartData, cartData);
            await _context.Users.UpdateOneAsync(u => u.Id == user.Id, update);

            return Ok(new { success = true, message = $"Item {itemId} removed from cart" });
        }
    }
}
