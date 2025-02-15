﻿using System.Text;
using System.Text.Json;
using AutoMapper;
using CustomerService.DTOs.Response;
using CustomerService.Repositories;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace CustomerService.Messaging.RPC;

public class CustomerRpcServer(IServiceScopeFactory scopeFactory, IMapper mapper) : IAsyncDisposable
{
    private IConnection? _connection;
    private IChannel? _channel;

    private const string QueueName = "customer_rpc_queue";

    public async Task StartAsync()
    {
        var factory = new ConnectionFactory
        {
            HostName = "localhost",
        };

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

            if (Guid.TryParse(requestString, out var customerId))
            {
                var customerDto = await GetCustomerDtoAsync(customerId);
                if (customerDto != null)
                {
                    jsonResponse = JsonSerializer.Serialize(customerDto);
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error processing request: {ex.Message}");
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

    private async Task<CustomerResponseDto?> GetCustomerDtoAsync(Guid customerId)
    {
        try
        {
            using var scope = scopeFactory.CreateScope();
            var customerRepository = scope.ServiceProvider.GetRequiredService<ICustomerRepository>();

            var customerEntity = await customerRepository.GetCustomerByIdAsync(customerId);

            return customerEntity is null ? null : mapper.Map<CustomerResponseDto>(customerEntity);
        }
        catch
        {
            return null;
        }
    }

    public async ValueTask DisposeAsync()
    {
        if (_channel is not null)
        {
            await _channel.CloseAsync();
        }

        if (_connection is not null)
        {
            await _connection.CloseAsync();
        }
    }
}