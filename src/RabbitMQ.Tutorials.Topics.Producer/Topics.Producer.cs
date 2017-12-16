using System;
using System.Text;
using RabbitMQ.Client;

namespace RabbitMQ.Tutorials.Topics.Producer
{
    public static class Program
    {
        public static void Main()
        {
            Console.Title = "Topics.Producer";

            var factory = new ConnectionFactory()
            {
                HostName = "localhost"
            };

            using (var connection = factory.CreateConnection())
            {
                using (var channel = connection.CreateModel())
                {
                    channel.ExchangeDeclare("topic_logs", "topic");

                    while (true)
                    {
                        Console.Write("Input routingKey: ");
                        var routingKey = Console.ReadLine();

                        Console.Write("Input message: ");
                        var message = Console.ReadLine() ?? string.Empty;

                        var body = Encoding.UTF8.GetBytes(message);
                        channel.BasicPublish(exchange: "topic_logs", routingKey: routingKey, basicProperties: null, body: body);
                        Console.WriteLine(" [x] Sent '{0}': '{1}'", routingKey, message);
                    }
                }
            }
        }
    }
}
