using Azure.Storage.Queues;
using Azure.Storage.Queues.Models;
using MessagingWithStorageQueue.Model;
using Microsoft.Extensions.Options;
using System.Text.Json;

namespace MessagingWithStorageQueue.Service
{
    public class StorageQueueService : IStorageQueueService
    {
        private readonly QueueOptions _queueOptions;

        public StorageQueueService(IOptions<QueueOptions> options)
        {
            _queueOptions = options.Value;
        }

        /// <summary>
        /// Inspencionar as mensagens da fila sem as remover
        /// </summary>
        /// <param name="queueName">Nome da fila</param>
        /// <returns>Uma lista de menssagens</returns>
        public async Task<object?> PeekNextMessageAsync(string queueName)
        {
            QueueClient queueClient = new QueueClient(_queueOptions.ConnectionString, queueName);

            if (queueClient.Exists())
            {
                // Inspenciona as mensagens 
                PeekedMessage[] peekedMessages = await queueClient.PeekMessagesAsync();

                var product = peekedMessages.FirstOrDefault()?.MessageText;

                var response = product != null ? JsonSerializer.Deserialize<ProductModel>(product) : null;

                return response;
            }

            return null;
        }

        /// <summary>
        /// Recebe as mensagens
        /// </summary>
        /// <param name="queueName">Nome da fila</param>
        /// <returns>A próxima mensagem</returns>
        public async Task<object?> ReadMessageAsync(string queueName)
        {
            QueueClient queueClient = new QueueClient(_queueOptions.ConnectionString, queueName);

            if (queueClient.Exists())
            {
                // Recebe as mensagens 
                QueueMessage[] message = await queueClient.ReceiveMessagesAsync();

                var product = message.FirstOrDefault()?.MessageText;

                ProductModel? response = null;
                if (product != null)
                {
                    response = JsonSerializer.Deserialize<ProductModel>(product);
                    await queueClient.DeleteMessageAsync(message[0].MessageId, message[0].PopReceipt);
                }

                return response;
            }

            return null;
        }

        /// <summary>
        /// Adiciona mensagens na fila
        /// </summary>
        /// <param name="product">Objeto a ser inserido na fila</param>
        /// <param name="queueName">Nome da fila que vai receber a mensagem</param>
        /// <returns>Id da mensagem</returns>
        public async Task<object?> AddMessageAsync(ProductModel product, string queueName)
        {
            // Criar uma instância de QueueClient
            QueueClient queueClient = new QueueClient(_queueOptions.ConnectionString, queueName);

            // Cria uma queue caso não exista 
            await queueClient.CreateIfNotExistsAsync();

            if (queueClient.Exists())
            {
                var message = JsonSerializer.Serialize(product);
                var response = await queueClient.SendMessageAsync(message);

                return new { messageId = response.Value.MessageId };
            }

            return null;
        }

        /// <summary>
        /// Cria uma nova Fila 
        /// </summary>
        /// <param name="queueName">Nome da fila</param>
        /// <returns>Nome da fila</returns>
        public async Task<string?> CreateQueueAsync(string queueName)
        {
            // Cria uma instância de QueueClient que será utilizada para manipular a Queue
            QueueClient queueClient = new QueueClient(_queueOptions.ConnectionString, queueName);

            // Cria a Queue se não existir
            var response = await queueClient.CreateIfNotExistsAsync();

            if(response.IsError){
                return null;
            }

            return queueName;
        }

        /// <summary>
        /// Deletar uma queue
        /// </summary>
        /// <param name="queueName">Nome da queue</param>
        /// <returns></returns>
        public async Task<object?> DeleteQueueAsync(string queueName)
        {
            QueueClient queueClient = new QueueClient(_queueOptions.ConnectionString, queueName);

            if (queueClient.Exists())
            {
                // Deleta uma fila
                var response = await queueClient.DeleteAsync();

                return response.Status;
            }

            return null;
        }
    }
}
