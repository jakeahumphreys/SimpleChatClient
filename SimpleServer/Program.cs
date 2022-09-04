using SimpleServer.Services;

namespace SimpleServer
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var server = new ChatService("127.0.0.1",4444);
            server.Start();
        }
    }
}
