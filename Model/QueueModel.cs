namespace MessagingWithStorageQueue.Model
{
    public class QueueModel
    {
        public string Name { get; set; } = string.Empty;
        public ProductModel Message { get; set; } = new ProductModel();
    }
}
