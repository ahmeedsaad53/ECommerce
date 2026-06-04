using E_Commerce.Date;
using ECommerce.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace ECommerce.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly AppDbContext _context;
        public OrderController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> MakeNewOrderAsync(int cartid)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userId == null)
                return Unauthorized();

            var cart = await _context.Carts
                .FirstOrDefaultAsync(c => c.Id == cartid && c.UserId == userId);

            if (cart == null)
                return NotFound("Cart not found or not yours");

            var cartItems = await _context.CartItems
                .Where(c => c.CartId == cart.Id)
                .ToListAsync();

            if (!cartItems.Any())
                return BadRequest("Cart is empty");

            // ✅ stock check + reduce
            foreach (var item in cartItems)
            {
                var product = await _context.Products.FindAsync(item.ProductId);

                if (product.Stock < item.Quantity)
                    return BadRequest("Not enough stock");

                product.Stock -= item.Quantity;
            }

            // ✅ create order
            var order = new Order
            {
                UserId = cart.UserId,   // ✅ FIXED
                Status = "Pending",
                CreatedAt = DateTime.Now,
                TotalAmount = cartItems.Sum(i => i.Price * i.Quantity) // ✅ FIXED
            };

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            // ✅ create order items
            foreach (var item in cartItems)
            {
                _context.OrderItems.Add(new OrderItem
                {
                    OrderId = order.Id,
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                    Price = item.Price
                });
            }

            // ✅ clear cart
            _context.CartItems.RemoveRange(cartItems);

            await _context.SaveChangesAsync();

            return Ok(order);
        }








        [HttpGet]//get all orders only for admin
        [Authorize(Roles = "Admin")]
       public async Task<IActionResult> GetAllOrder()
        {
            var Orders = await _context.Orders.Select(p => new OrderDTO
            {
                Id = p.Id,
                UserId = p.UserId,
                TotalAmount = p.TotalAmount,
                Status = p.Status,
                CreatedAt = DateTime.Now
            }).ToListAsync();
            if (Orders.Count == 0) return BadRequest();
            return Ok(Orders);
        }








        [HttpGet("{id}")]//get order by id only for admin
        [Authorize(Roles = "Admin")]

        public async Task<IActionResult> GetOrderById(int id)
        {
            var Order = await _context.Orders.Where(o => o.Id == id).Select(p => new OrderDTO
            {
                Id = p.Id,
                UserId = p.UserId,
                TotalAmount = p.TotalAmount,
                Status = p.Status,
                CreatedAt = p.CreatedAt
            }).FirstOrDefaultAsync();
            if (Order == null) return BadRequest();
            return Ok(Order);

        }








        [HttpDelete("{id}")]//delete order by id only for admin
        [Authorize(Roles = "Admin")]

        public async Task<IActionResult> DeleteOrder(int id)
        {
            var Order = await _context.Orders.FindAsync(id);
            if (Order == null) return BadRequest();
            var orderItems = await _context.OrderItems.Where(oi => oi.OrderId == id).ToListAsync();
            foreach (var Item in orderItems)
            {
                var product = await _context.Products.FindAsync(Item.ProductId);
                if (product != null)
                {
                    product.Stock += Item.Quantity;
                }
            }
            _context.Orders.Remove(Order);
            await _context.SaveChangesAsync();
            return Ok();
        }








        [HttpPut("{id}")]//update order status by id only for admin
        [Authorize(Roles = "Admin")]

        public async Task<IActionResult> UpdateOrderStatus(int id, string status)
        {
            var Order = await _context.Orders.FindAsync(id);
            if (Order == null) return BadRequest();
            Order.Status = status;
            _context.Orders.Update(Order);
            await _context.SaveChangesAsync();
            return Ok(Order);
        }





        [HttpGet("user/{userId}")]//get orders by user id
        [Authorize]
        public async Task<IActionResult> GetOrdersByUserId(string userId)
        {
            var Orders=await _context.Orders.Where(o=>o.UserId==userId).Select(p=> new OrderDTO
            {
                Id = p.Id,
                UserId = p.UserId,
                TotalAmount = p.TotalAmount,
                Status = p.Status,
                CreatedAt = DateTime.Now


            }).ToListAsync();
            if(Orders==null) return BadRequest();
            return Ok(Orders);
        }






        [HttpGet("my-orders")]//get orders for the currently authenticated user
        [Authorize]
        public async Task<IActionResult> GetMyOrders()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;//get user id from token claims

            if (userId == null) return Unauthorized();

            var orders = await _context.Orders
                .Where(o => o.UserId == userId)
                .Select(o => new OrderDTO
                {
                    Id = o.Id,
                    UserId = o.UserId,
                    TotalAmount = o.TotalAmount,
                    Status = o.Status,
                    CreatedAt = o.CreatedAt
                })
                .ToListAsync();
            if (orders==null) return BadRequest();
            return Ok(orders);
        }


    }
}
