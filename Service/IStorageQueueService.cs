using MessagingWithStorageQueue.Model;

namespace MessagingWithStorageQueue.Service
{
    public interface IStorageQueueService
    {
        Task<string?> CreateQueueAsync(string queueName);
        Task<object?> AddMessageAsync(ProductModel product, string queueName);
        Task<object?> PeekNextMessageAsync(string queueName);
        Task<object?> ReadMessageAsync(string queueName);
        Task<object?> DeleteQueueAsync(string queueName);
    }
}
