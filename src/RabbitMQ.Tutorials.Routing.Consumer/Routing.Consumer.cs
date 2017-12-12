using System;
using System.Collections.Generic;
using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace RabbitMQ.Tutorials.Routing.Consumer
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            Console.Title = "Routing.Consumer";

            List<string> types = new List<string>();

            Console.WriteLine("Subscribe to messages with numbers? [Y/N]");
            var consoleKeyInfo = Console.ReadKey();
            if (consoleKeyInfo.Key == ConsoleKey.Y)
            {
                types.Add("Number");
            }
            Console.WriteLine();

            Console.WriteLine("Subscribe to messages without numbers? [Y/N]");
            consoleKeyInfo = Console.ReadKey();
            if (consoleKeyInfo.Key == ConsoleKey.Y)
            {
                types.Add("Not number");
            }
            Console.WriteLine();

            var factory = new ConnectionFactory()
            {
                HostName = "localhost"
            };

            using (var connection = factory.CreateConnection())
            {
                using (var channel = connection.CreateModel())
                {
                    channel.ExchangeDeclare(exchange: "direct_logs", type: "direct");

                    var queueName = channel.QueueDeclare()
                                           .QueueName;

                    foreach (var type in types)
                    {
                        channel.QueueBind(queue: queueName, exchange: "direct_logs", routingKey: type);
                    }

                    Console.WriteLine(" [*] Waiting for messages.");

                    var consumer = new EventingBasicConsumer(channel);
                    consumer.Received += (model, ea) =>
                    {
                        var body = ea.Body;
                        var message = Encoding.UTF8.GetString(body);
                        var routingKey = ea.RoutingKey;
                        Console.WriteLine(" [x] Received '{0}':'{1}'", routingKey, message);
                    };
                    channel.BasicConsume(queue: queueName, autoAck: true, consumer: consumer);

                    Console.WriteLine(" Press [enter] to exit.");
                    Console.ReadLine();
                }
            }
        }
    }
}
