﻿using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Connections;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.Configuration;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

namespace ProductsService.Business.RabbitMQ;

public class RabbitMQPublisher : IRabbitMQPublisher, IDisposable
{
    private readonly IConfiguration _configuration;
    private IConnection? _connection;
    private IChannel? _channel;
    public RabbitMQPublisher(IConfiguration configuration)
    {
        _configuration = configuration;

        string hostName = _configuration["RabbitMQ_HostName"]!;
        string userName = _configuration["RabbitMQ_UserName"]!;
        string password = _configuration["RabbitMQ_Password"]!;
        string port = _configuration["RabbitMQ_Port"]!;

        ConnectionFactory connectionFactory = new ConnectionFactory
        {
            HostName = hostName,
            Port = Int32.Parse(port),
            UserName = userName,
            Password = password
        };

        Task.Run(async () => await CreateChannelAsync(connectionFactory));
    }

    private async Task CreateChannelAsync(ConnectionFactory connectionFactory)
    {
        _connection = await connectionFactory.CreateConnectionAsync();
        _channel = await _connection.CreateChannelAsync();
        return;

    }
    public void Publish<T>(Dictionary<string, object> headers, T message)
    {
        string messageJson = JsonSerializer.Serialize(message);
        byte[] messageBodyInBytes = Encoding.UTF8.GetBytes(messageJson);

        //Create exchange
        string exchangeName = _configuration["RabbitMQ_Products_Exchange"]!;
        _channel.ExchangeDeclareAsync(
            exchange: exchangeName,
            type: ExchangeType.Headers,
            durable: true);

        //Publish message
        var basicProperties = new BasicProperties();
        basicProperties.Headers = headers!;

        //Publish Message
        _channel.BasicPublishAsync(
            exchange: exchangeName,
            routingKey: string.Empty,
            basicProperties: basicProperties,
            mandatory: true,
            body: messageBodyInBytes);
    }

    public void Dispose()
    {
        _channel?.Dispose();
        _connection?.Dispose();
    }
}
