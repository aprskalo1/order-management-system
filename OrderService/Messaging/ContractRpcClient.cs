using System.Collections.Concurrent;
using System.Text;
using System.Text.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Shared.Models;

namespace OrderService.Messaging;

public class ContractRpcClient : IAsyncDisposable
{
    private IConnection? _connection;
    private IChannel? _channel;
    private string? _replyQueueName;

    private readonly ConnectionFactory _factory = new()
    {
        HostName = "localhost",
    };

    private readonly ConcurrentDictionary<string, TaskCompletionSource<string>> _callbackMapper
        = new();

    private const string RequestQueueName = "contract_rpc_queue";

    public async Task StartAsync()
    {
        _connection = await _factory.CreateConnectionAsync();
        _channel = await _connection.CreateChannelAsync();

        var queueOk = await _channel.QueueDeclareAsync();
        _replyQueueName = queueOk.QueueName;

        var consumer = new AsyncEventingBasicConsumer(_channel);
        consumer.ReceivedAsync += OnResponseReceivedAsync;

        await _channel.BasicConsumeAsync(
            queue: _replyQueueName,
            autoAck: true,
            consumer: consumer
        );
    }

    public async Task<ContractData?> GetLastContractAsync(Guid customerId)
    {
        if (_channel is null)
            throw new InvalidOperationException("Channel not initialized. Call StartAsync first.");

        var correlationId = Guid.NewGuid().ToString();
        var props = new BasicProperties
        {
            CorrelationId = correlationId,
            ReplyTo = _replyQueueName
        };

        var messageBytes = Encoding.UTF8.GetBytes(customerId.ToString());

        var tcs = new TaskCompletionSource<string>(TaskCreationOptions.RunContinuationsAsynchronously);
        _callbackMapper[correlationId] = tcs;

        await _channel.BasicPublishAsync(
            exchange: string.Empty,
            routingKey: RequestQueueName,
            mandatory: true,
            basicProperties: props,
            body: messageBytes
        );

        var jsonResponse = await tcs.Task;
        if (string.IsNullOrEmpty(jsonResponse))
        {
            return null;
        }

        var contractData = JsonSerializer.Deserialize<ContractData>(jsonResponse);
        return contractData;
    }

    private Task OnResponseReceivedAsync(object sender, BasicDeliverEventArgs ea)
    {
        var body = ea.Body.ToArray();
        var response = Encoding.UTF8.GetString(body);
        var correlationId = ea.BasicProperties.CorrelationId;

        if (!string.IsNullOrEmpty(correlationId)
            && _callbackMapper.TryRemove(correlationId, out var tcs))
        {
            tcs.SetResult(response);
        }

        return Task.CompletedTask;
    }

    public async ValueTask DisposeAsync()
    {
        if (_channel != null)
            await _channel.CloseAsync();
        if (_connection != null)
            await _connection.CloseAsync();
    }
}