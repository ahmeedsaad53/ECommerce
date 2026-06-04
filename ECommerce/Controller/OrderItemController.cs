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
    public class OrderItemController : ControllerBase
    {
        private readonly AppDbContext _context;
        public OrderItemController(AppDbContext context)
        {
            _context = context;
        }
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> AddItem(CreateOrderItemDTO dto)
        {
            if (dto == null) return BadRequest();

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;//get the id from the token 

            if (userId == null) return Unauthorized();

            var product=await _context.Products.FindAsync(dto.ProductId);

            if (product == null) return BadRequest();

            if(product.Stock < dto.Quantity) return BadRequest("Not enough stock");
              
            var order = await _context.Orders
                .FirstOrDefaultAsync(o => o.Id == dto.OrderId && o.UserId == userId);

            if (order == null) return NotFound("Order not found or not yours");
            var orderItem = new OrderItem
            {
                OrderId = dto.OrderId,
                ProductId = product.Id,
                Quantity = dto.Quantity,
                Price = product.Price
            };
            _context.OrderItems.Add(orderItem);
            await _context.SaveChangesAsync();
            return Ok(orderItem);
        }


        [HttpGet("order/{orderId}")]
        [Authorize]
        public async Task<IActionResult> GetOrderByID(int orderId)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;//get user id from token claims

            if (userId == null) return Unauthorized();
            var item = await _context.OrderItems.Where(o => o.OrderId == orderId && o.Order.UserId == userId).Select(o => new OrderItemDTO
            {
                Id = o.Id,
                OrderId = o.OrderId,
                ProductId = o.ProductId,
                Price = o.Price,
                Quantity = o.Quantity,

            }).ToListAsync();
            if (item == null) return BadRequest();
            return Ok(item);
        }
        [HttpDelete("{id:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteItem(int id)
        {
            var item=await _context.OrderItems.FindAsync(id);
            if (item == null) return NotFound();
            _context.OrderItems.Remove(item);
            await _context.SaveChangesAsync();
            return Ok();
        }        
    }
}
