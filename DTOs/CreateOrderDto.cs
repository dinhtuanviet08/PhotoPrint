namespace PhotoPrintAPI.DTOs
{
    public class CreateOrderDto
    {
        public string Username { get; set; }
        public string ImageUrl { get; set; }
        public int Quantity { get; set; }
        public string Size { get; set; }
    }
}
