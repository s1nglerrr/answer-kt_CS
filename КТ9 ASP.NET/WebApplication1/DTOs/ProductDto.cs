namespace WebApplication1.DTOs
{
    public class CreateProductDto
    {
        public Guid CategoryId { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public string Description { get; set; }
    }

    public class UpdateProductDto
    {
        public string Name { get; set; }
        public decimal? Price { get; set; }
        public string Description { get; set; }
        public Guid? CategoryId { get; set; }
    }
}
