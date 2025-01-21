using System.Text;
using System.Text.Json;
using AutoMapper;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using PriceService.DTOs.Response;
using PriceService.Repositories;
using Shared.Models;

namespace PriceService.Messaging;

public class ProductRpcServer(IServiceScopeFactory scopeFactory, IMapper mapper) : IAsyncDisposable
{
    private IConnection? _connection;
    private IChannel? _channel;

    private const string QueueName = "product_rpc_queue";

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

        await _channel.BasicQosAsync(0, 1, false);

        var consumer = new AsyncEventingBasicConsumer(_channel);
        consumer.ReceivedAsync += HandleRpcRequestAsync;

        await _channel.BasicConsumeAsync(
            queue: QueueName,
            autoAck: false,
            consumer: consumer
        );

        Console.WriteLine($"[ProductRpcServer] Listening on '{QueueName}'");
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

            var request = JsonSerializer.Deserialize<ProductRequest>(requestString);
            if (request != null)
            {
                var productDto = await GetProductDtoAsync(request.ProductId, request.EffectiveDate);
                if (productDto != null)
                {
                    jsonResponse = JsonSerializer.Serialize(productDto);
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[ProductRpcServer] Error: {ex.Message}");
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

    private async Task<ProductResponseDto?> GetProductDtoAsync(Guid productId, DateTime effectiveDate)
    {
        try
        {
            using var scope = scopeFactory.CreateScope();
            var productRepository = scope.ServiceProvider.GetRequiredService<IProductRepository>();

            var product = await productRepository.GetProductWithPriceDateRange(productId, effectiveDate);
            return product == null ? null : mapper.Map<ProductResponseDto>(product);
        }
        catch
        {
            return null;
        }
    }

    public async ValueTask DisposeAsync()
    {
        if (_channel != null) await _channel.CloseAsync();
        if (_connection != null) await _connection.CloseAsync();
    }
}