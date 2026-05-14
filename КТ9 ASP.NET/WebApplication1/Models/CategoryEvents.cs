namespace WebApplication1.Models
{
    public class CategoryCreated
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }
    public class CategoryUpdated
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }

    public class CategoryDeleted
    {
        public Guid Id { get; set; }
    }
}
