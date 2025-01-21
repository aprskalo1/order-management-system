using System.Text;
using System.Text.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using ContractService.DTOs.Response;
using ContractService.Services;

namespace ContractService.Messaging.RPC;

public class ContractRpcServer(IServiceScopeFactory scopeFactory) : IAsyncDisposable
{
    private IConnection? _connection;
    private IChannel? _channel;

    private const string QueueName = "contract_rpc_queue";

    public async Task StartAsync()
    {
        var factory = new ConnectionFactory { HostName = "localhost" };
        _connection = await factory.CreateConnectionAsync();
        _channel = await _connection.CreateChannelAsync();

        await _channel.QueueDeclareAsync(
            queue: QueueName,
            durable: false,
            exclusive: false,
            autoDelete: false,
            arguments: null
        );

        await _channel.BasicQosAsync(prefetchSize: 0, prefetchCount: 1, global: false);

        var consumer = new AsyncEventingBasicConsumer(_channel);
        consumer.ReceivedAsync += HandleRpcRequestAsync;

        await _channel.BasicConsumeAsync(
            queue: QueueName,
            autoAck: false,
            consumer: consumer
        );

        Console.WriteLine($"[ContractRpcServer] Listening on '{QueueName}'...");
    }

    private async Task HandleRpcRequestAsync(object sender, BasicDeliverEventArgs ea)
    {
        var jsonResponse = string.Empty;
        var correlationId = ea.BasicProperties.CorrelationId ?? string.Empty;

        var replyProps = new BasicProperties
        {
            CorrelationId = correlationId
        };

        try
        {
            var body = ea.Body.ToArray();
            var requestString = Encoding.UTF8.GetString(body);

            if (Guid.TryParse(requestString, out Guid customerId))
            {
                using var scope = scopeFactory.CreateScope();
                var contractService = scope.ServiceProvider.GetRequiredService<IContractService>();

                var contractDto = await TryGetLastContractAsync(contractService, customerId);

                if (contractDto != null)
                {
                    jsonResponse = JsonSerializer.Serialize(contractDto);
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[ContractRpcServer] Error: {ex.Message}");
        }
        finally
        {
            var responseBytes = Encoding.UTF8.GetBytes(jsonResponse);

            if (_channel != null && ea.BasicProperties.ReplyTo != null)
            {
                await _channel.BasicPublishAsync(
                    exchange: string.Empty,
                    routingKey: ea.BasicProperties.ReplyTo,
                    mandatory: true,
                    basicProperties: replyProps,
                    body: responseBytes
                );
            }

            if (_channel != null)
            {
                await _channel.BasicAckAsync(deliveryTag: ea.DeliveryTag, multiple: false);
            }
        }
    }

    private static async Task<ContractResponseDto?> TryGetLastContractAsync(
        IContractService contractService,
        Guid customerId)
    {
        try
        {
            return await contractService.GetLastContractAsync(customerId);
        }
        catch
        {
            return null;
        }
    }

    public async ValueTask DisposeAsync()
    {
        if (_channel != null)
            await _channel.CloseAsync();
        if (_connection != null)
            await _connection.CloseAsync();
    }
}