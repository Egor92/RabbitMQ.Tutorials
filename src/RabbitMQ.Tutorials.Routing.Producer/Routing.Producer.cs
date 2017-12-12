using System;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using RabbitMQ.Client;

namespace RabbitMQ.Tutorials.Routing.Producer
{
    public static class Program
    {
        [SuppressMessage("ReSharper", "FunctionNeverReturns")]
        public static void Main(string[] args)
        {
            Console.Title = "Routing.Producer";

            var factory = new ConnectionFactory
            {
                HostName = "localhost"
            };

            using (var connection = factory.CreateConnection())
            {
                using (var channel = connection.CreateModel())
                {
                    channel.ExchangeDeclare(exchange: "direct_logs", type: "direct");

                    while (true)
                    {
                        var message = Console.ReadLine() ?? string.Empty;
                        var type = double.TryParse(message, out var _)
                            ? "Number"
                            : "Not number";
                        var body = Encoding.UTF8.GetBytes(message);
                        channel.BasicPublish(exchange: "direct_logs", routingKey: type, basicProperties: null, body: body);
                        Console.WriteLine(" [x] Sent '{0}':'{1}'", type, message);
                    }
                }
            }
        }
    }
}
