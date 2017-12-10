using System;
using System.Linq;
using System.Text;
using RabbitMQ.Client;

namespace RabbitMQ.Tutorials.WorkQueues.Producer
{
    public static class Program
    {
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
                    channel.QueueDeclare(queue: "task_queue", durable: false, exclusive: false, autoDelete: false, arguments: null);

                    while (true)
                    {
                        var message = GetMessage();
                        var body = Encoding.UTF8.GetBytes(message);

                        var properties = channel.CreateBasicProperties();
                        properties.Persistent = true;

                        channel.BasicPublish(exchange: "", routingKey: "task_queue", basicProperties: properties, body: body);
                        Console.WriteLine(" [x] Sent {0}", message);
                        Console.ReadLine();
                    }
                }
            }
        }

        private static string GetMessage()
        {
            var random = new Random();
            var chars = Enumerable.Range(0, 10)
                                  .Select(_ => (char) random.Next(46, 57))
                                  .ToArray();
            var word = new string(chars);
            return word;
        }
    }
}
