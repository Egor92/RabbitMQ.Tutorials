using System;
using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace RabbitMQ.Tutorials.Topics.Consumer
{
    public static class Program
    {
        public static void Main()
        {
            Console.Title = "Topics.Consumer";

            Console.WriteLine("Input bindingKeys:");
            var separator = new[]
            {
                ' '
            };
            string[] bindingKeys = Console.ReadLine()
                                          .Split(separator, StringSplitOptions.RemoveEmptyEntries);

            var factory = new ConnectionFactory
            {
                HostName = "localhost"
            };

            using (var connection = factory.CreateConnection())
            {
                using (var channel = connection.CreateModel())
                {
                    channel.ExchangeDeclare("topic_logs", "topic");
                    var queueDeclareOk = channel.QueueDeclare();
                    var queueName = queueDeclareOk.QueueName;

                    foreach (var bindingKey in bindingKeys)
                    {
                        channel.QueueBind(queue: queueName, exchange: "topic_logs", routingKey: bindingKey);
                    }

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
