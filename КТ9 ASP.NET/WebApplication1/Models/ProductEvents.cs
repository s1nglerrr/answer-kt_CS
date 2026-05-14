namespace WebApplication1.Models
{
    public class ProductCreated
    {
        public Guid Id { get; set; }
        public Guid CategoryId { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public string Description { get; set; }
    }
    public class ProductUpdated
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public decimal? Price { get; set; }
        public string Description { get; set; }
        public Guid? CategoryId { get; set; }
    }

    public class ProductDeleted
    {
        public Guid Id { get; set; }
    }

}
