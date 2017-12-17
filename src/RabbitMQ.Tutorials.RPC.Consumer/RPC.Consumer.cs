using System;

namespace RabbitMQ.Tutorials.RPC.Consumer
{
    public static class Program
    {
        public static void Main()
        {
            Console.Title = "RPC.Consumer";

            var rpcClient = new RpcClient();

            Console.WriteLine(" [x] Requesting fib(30)");
            var response = rpcClient.Call("30");

            Console.WriteLine(" [.] Got '{0}'", response);
            rpcClient.Close();
        }
    }
}
