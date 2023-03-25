namespace MessagingWithStorageQueue.Model
{
    public class ProductModel
    {
        public ProductModel()
        {
            Id= Guid.NewGuid();
        }

        public Guid Id { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
    }
}
