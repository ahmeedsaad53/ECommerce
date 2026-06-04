using E_Commerce.Date;
using ECommerce.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace ECommerce.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class CartController : ControllerBase
    {
        private readonly AppDbContext _context;
        public CartController(AppDbContext context)
        {
            _context = context;
        }
        [HttpGet("MyCart")]
        [Authorize]
        public async Task<IActionResult> GetCartByID()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;//get user id from token claims

            if (userId == null) return Unauthorized();

            var cart = await _context.Carts.FirstOrDefaultAsync(c => c.UserId == userId);

            if (cart == null) return NotFound();
            return Ok(cart);
        }
        [HttpPost("AddToCart")]
        [Authorize]
        public async Task<IActionResult> AddToCart(CartItemDTO dto)
        {
            if (dto == null) return  BadRequest(); 
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;//get user id from token claims
            if (userIdClaim == null) return Unauthorized();
            if (!int.TryParse(userIdClaim, out int userId))
                return BadRequest("Invalid user ID");


            var cart = await _context.Carts.FirstOrDefaultAsync(c => c.UserId == userIdClaim);//get cart by  user id
            if (cart == null) return NotFound();


            var product = await _context.Products.FindAsync(dto.ProductId);//get product by id
            if (product == null) return NotFound();
            if (dto.Quantity <= 0) return BadRequest("Invalid quantity");//validate quantity
            if (dto.Quantity > product.Stock) return BadRequest("Not enough stock");//validate stock

            var existingItem = await _context.CartItems
                    .FirstOrDefaultAsync(c => c.CartId == cart.Id && c.ProductId == dto.ProductId);
 

            if (existingItem != null)
            {
                existingItem.Quantity += dto.Quantity;
            }
            else
            {
                var cartItemEntity = new CartItem
            {
                    CartId = cart.Id,
                ProductId = dto.ProductId,
                Quantity = dto.Quantity,
                Price = product.Price,
                Date = DateTime.Now
            };
                _context.CartItems.Add(cartItemEntity);
            }
            await _context.SaveChangesAsync();
            return Ok();
        }


        [HttpDelete("RemoveFromCart")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> RemoveFromCart(int cartItemId)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;//get user id from token claims

            if (userIdClaim == null) return Unauthorized();

            var cartItem = await _context.CartItems.Include(ci => ci.Cart)
                .FirstOrDefaultAsync(ci => ci.Id == cartItemId && ci.Cart.UserId == userIdClaim);//get cart item by id and user id

            if (cartItem == null) return NotFound();

            _context.CartItems.Remove(cartItem);
            await _context.SaveChangesAsync();
            return Ok();

        }



        }
}
