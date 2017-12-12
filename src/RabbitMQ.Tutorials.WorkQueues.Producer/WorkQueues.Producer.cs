using System;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using RabbitMQ.Client;

namespace RabbitMQ.Tutorials.WorkQueues.Producer
{
    public static class Program
    {
        [SuppressMessage("ReSharper", "FunctionNeverReturns")]
        public static void Main(string[] args)
        {
            Console.Title = "WorkQueues.Producer";

            var factory = new ConnectionFactory()
            {
                HostName = "localhost"
            };

            using (var connection = factory.CreateConnection())
            {
                using (var channel = connection.CreateModel())
                {
                    channel.QueueDeclare(queue: "task_queue", durable: true, exclusive: false, autoDelete: false, arguments: null);

                    int index = 0;
                    while (true)
                    {
                        var message = index.ToString();
                        var body = Encoding.UTF8.GetBytes(message);

                        var properties = channel.CreateBasicProperties();
                        properties.Persistent = true;

                        channel.BasicPublish(exchange: "", routingKey: "task_queue", basicProperties: properties, body: body);
                        Console.Write(" [x] Sent {0}", message);
                        Console.ReadLine();

                        index++;
                    }
                }
            }
        }
    }
}
