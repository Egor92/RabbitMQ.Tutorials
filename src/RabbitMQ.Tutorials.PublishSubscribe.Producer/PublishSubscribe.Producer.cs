using System;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using RabbitMQ.Client;

namespace RabbitMQ.Tutorials.PublishSubscribe.Producer
{
    public static class Program
    {
        [SuppressMessage("ReSharper", "FunctionNeverReturns")]
        public static void Main(string[] args)
        {
            Console.Title = "PublishSubscribe.Producer";

            var factory = new ConnectionFactory
            {
                HostName = "localhost"
            };

            using (var connection = factory.CreateConnection())
            {
                using (var channel = connection.CreateModel())
                {
                    channel.ExchangeDeclare(exchange: "logs", type: "fanout");

                    while (true)
                    {
                        var message = Console.ReadLine() ?? string.Empty;
                        var body = Encoding.UTF8.GetBytes(message);
                        channel.BasicPublish(exchange: "logs", routingKey: "", basicProperties: null, body: body);
                        Console.WriteLine(" [x] Sent {0}", message);
                    }
                }
            }
        }
    }
}
