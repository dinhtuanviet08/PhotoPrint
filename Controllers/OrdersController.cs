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
        public async Task<IActionResult> Post([FromForm] CreateOrderWithImageDto dto)
        {
            if (dto.Image == null || dto.Image.Length == 0)
                return BadRequest("Image file is required.");

            // Lưu ảnh vào thư mục wwwroot/images (bạn có thể thay đổi đường dẫn theo ý)
            var fileName = Guid.NewGuid() + Path.GetExtension(dto.Image.FileName);
            var filePath = Path.Combine("wwwroot/images", fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await dto.Image.CopyToAsync(stream);
            }

            var order = new Order
            {
                Username = dto.Username,
                ImageUrl = "/images/" + fileName, // hoặc lưu đường dẫn đầy đủ nếu cần
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
