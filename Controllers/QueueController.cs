using MessagingWithStorageQueue.Model;
using MessagingWithStorageQueue.Service;
using Microsoft.AspNetCore.Mvc;

namespace MessagingWithStorageQueue.Controllers;

[ApiController]
[Route("[controller]")]
public class QueueController : ControllerBase
{
    private readonly IStorageQueueService _queueService;

    public QueueController(IStorageQueueService queueService)
    {
        _queueService = queueService;
    }

    /// <summary>
    /// Inspeciona mensagens na fila
    /// </summary>
    /// <param name="queueName">Nome da fila</param>
    /// <returns>Uma lista de mensagem</returns>
    [HttpGet("messages/{queueName}")]
    public async Task<ActionResult<string>> Get(string queueName)
    {
        var response = await _queueService.PeekNextMessageAsync(queueName);

        if(response is null)
            return BadRequest($"Erro ao inspecionar menssagens na Queue {queueName}");

        return Ok(response);
    }

    /// <summary>
    /// Recebe a próxima mensagem manipula e deleta
    /// </summary>
    /// <param name="queueName">Nome da fila</param>
    /// <returns>Uma lista de mensagem</returns>
    [HttpGet("receive/messages/{queueName}")]
    public async Task<ActionResult<string>> Receive(string queueName)
    {
        var response = await _queueService.ReadMessageAsync(queueName);

        if(response is null)
            return BadRequest($"Erro ao receber menssagens na Queue {queueName}");

        return Ok(response);
    }

    /// <summary>
    /// Cria uma nova queue
    /// </summary>
    /// <param name="queue"></param>
    /// <returns>Nome da fila</returns>
    [HttpPost("create/queue")]
    public async Task<ActionResult<string>> Post([FromBody] QueueParamModel queue)
    {
        var response = await _queueService.CreateQueueAsync(queue.Name);

        if (string.IsNullOrEmpty(response))
            return BadRequest($"Erro ao criar Queue {queue.Name}");

        return Ok(response);
    }

    /// <summary>
    /// Envia uma nova mensagem para a fila
    /// </summary>
    /// <param name="message"></param>
    /// <returns>Id da mensagem</returns>
    [HttpPost("send")]
    public async Task<ActionResult<string>> Send([FromBody] QueueModel message)
    {
        var response = await _queueService.AddMessageAsync(message.Message, message.Name);

        if(response is null)
            return BadRequest($"Erro ao criar Queue {message.Name}");

        return Ok(response);
    }

    /// <summary>
    /// Deleta uma fila
    /// </summary>
    /// <param name="queueName">Nome da fila</param>
    /// <returns>Id da mensagem</returns>
    [HttpDelete("delete/{queueName}")]
    public async Task<ActionResult<string>> Delete(string queueName)
    {
        var response = await _queueService.DeleteQueueAsync(queueName);

        if(response is null)
            return BadRequest($"Erro ao criar Queue {queueName}");

        return NoContent();
    }
}
    