using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace PhotoPrintAPI.DTOs
{
    public class CreateOrderWithImageDto
    {
        [Required]
        public string Username { get; set; }

        [Required]
        public int Quantity { get; set; }

        [Required]
        public string Size { get; set; }

        [Required]
        public IFormFile Image { get; set; }
    }
}
