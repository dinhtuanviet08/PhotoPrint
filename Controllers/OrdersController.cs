using Microsoft.AspNetCore.Mvc;
using PhotoPrintAPI.DTOs;
using PhotoPrintAPI.Models;
using PhotoPrintAPI.Services;

namespace PhotoPrintAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrdersController : ControllerBase
    {
        private readonly OrderService _orderService;

        public OrdersController(OrderService orderService)
        {
            _orderService = orderService;
        }

        [HttpGet]
        public async Task<ActionResult<List<Order>>> Get() =>
            await _orderService.GetAllAsync();

        [HttpGet("{id:length(24)}")]
        public async Task<ActionResult<Order>> Get(string id)
        {
            var order = await _orderService.GetByIdAsync(id);
            if (order == null) return NotFound();
            return order;
        }

        [HttpPost]
        public async Task<IActionResult> Post(CreateOrderDto dto)
        {
            var order = new Order
            {
                Username = dto.Username,
                ImageUrl = dto.ImageUrl,
                Quantity = dto.Quantity,
                Size = dto.Size
            };
            await _orderService.CreateAsync(order);
            return CreatedAtAction(nameof(Get), new { id = order.Id }, order);
        }

        [HttpPut("{id:length(24)}")]
        public async Task<IActionResult> Update(string id, Order updatedOrder)
        {
            var existingOrder = await _orderService.GetByIdAsync(id);

            if (existingOrder is null)
            {
                return NotFound();
            }

            updatedOrder.Id = existingOrder.Id;
            await _orderService.UpdateAsync(id, updatedOrder);

            return NoContent();
        }

        [HttpDelete("{id:length(24)}")]
        public async Task<IActionResult> Delete(string id)
        {
            var order = await _orderService.GetByIdAsync(id);

            if (order is null)
            {
                return NotFound();
            }

            await _orderService.RemoveAsync(id);

            return NoContent();
        }
    }
}
