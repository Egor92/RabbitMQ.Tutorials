using System;
using System.Collections.Concurrent;
using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace RabbitMQ.Tutorials.RPC.Consumer
{
    public class RpcClient
    {
        private readonly IConnection _connection;
        private readonly IModel _channel;
        private readonly string _replyQueueName;
        private readonly EventingBasicConsumer _consumer;
        private readonly BlockingCollection<string> _responseQueue = new BlockingCollection<string>();
        private readonly IBasicProperties _properties;

        public RpcClient()
        {
            var factory = new ConnectionFactory()
            {
                HostName = "localhost"
            };

            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();
            var queueDeclare = _channel.QueueDeclare();
            _replyQueueName = queueDeclare.QueueName;
            _consumer = new EventingBasicConsumer(_channel);

            _properties = _channel.CreateBasicProperties();
            var correlationId = Guid.NewGuid()
                                    .ToString();
            _properties.CorrelationId = correlationId;
            _properties.ReplyTo = _replyQueueName;

            _consumer.Received += (model, ea) =>
            {
                var body = ea.Body;
                var response = Encoding.UTF8.GetString(body);
                if (ea.BasicProperties.CorrelationId == correlationId)
                {
                    _responseQueue.Add(response);
                }
            };
        }

        public string Call(string message)
        {
            var messageBytes = Encoding.UTF8.GetBytes(message);
            _channel.BasicPublish(exchange: "", routingKey: "rpc_queue", basicProperties: _properties, body: messageBytes);

            _channel.BasicConsume(consumer: _consumer, queue: _replyQueueName, autoAck: true);

            return _responseQueue.Take();
        }

        public void Close()
        {
            _connection.Close();
        }
    }
}