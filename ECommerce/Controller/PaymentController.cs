using E_Commerce.Date;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace ECommerce.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        private readonly AppDbContext _context;
        public PaymentController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost("{orderId}")]
        [Authorize]
        public async Task<IActionResult> Pay(int orderId)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;//get user id from token claims
            var order = await _context.Orders.FirstOrDefaultAsync(o=>o.Id == orderId && o.UserId == userId);

            if (order == null)
                return NotFound();

            if (order.Status == "Paid")
                return BadRequest("Order already paid");

            order.Status = "Paid";

            await _context.SaveChangesAsync();

            return Ok("Payment successful");
        }


        [HttpDelete("{orderId}")]
        [Authorize]
        public async Task<IActionResult> CancelPayment(int orderId)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;//get user id from token claims
            var order = await _context.Orders.FirstOrDefaultAsync(o=>o.Id == orderId && o.UserId == userId);
            if (order == null)
                return NotFound();
            if (order.Status != "Paid")
                return BadRequest("Order not paid yet");
            //restore stock quantity for each order item
            var orderItems = await _context.OrderItems.Where(oi => oi.OrderId == orderId).ToListAsync();

            foreach (var item in orderItems)
            {
                var product = await _context.Products.FindAsync(item.ProductId);
                if (product != null)
                {
                    product.Stock += item.Quantity;
                }
            }


            order.Status = "Cancelled";
            await _context.SaveChangesAsync();
            return Ok("Payment cancelled");
        }







        [HttpGet("{orderId}")]
        [Authorize]
        public async Task<IActionResult> GetPaymentStatus(int orderId)
        {
            var order = await _context.Orders.FindAsync(orderId);
            if (order == null)
                return NotFound();
            return Ok(new { orderId = order.Id, status = order.Status });
        }
    }
}
