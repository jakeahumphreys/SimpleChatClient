using System;
using System.Net;
using System.Net.Sockets;
using System.Collections.Generic;
using System.Threading;

namespace SimpleServer
{
    public class SimpleServer
    {
        private readonly TcpListener _tcpListener;
        private static List<Client> _clients;
        private readonly PacketHandler.PacketHandler _packetHandler;

        public SimpleServer(string ipAddress, int port)
        {
            _tcpListener = new TcpListener(IPAddress.Parse(ipAddress), port);
            _clients = new List<Client>();
            _packetHandler = new PacketHandler.PacketHandler(_clients);
        }

        public void Start()
        { 
            _tcpListener.Start();
            Console.WriteLine("Listener Started.");

            while (true)
            {
                if (_tcpListener.Pending())
                {
                    Socket socket = _tcpListener.AcceptSocket();
                    Client client = new Client(socket);
                    _clients.Add(client);
                
                    Console.WriteLine($"Added client successfully.");
                    _packetHandler.UpdateConnectedClients(_clients);
                
                    Thread thread = new Thread(new ParameterizedThreadStart(HandlePacketFromClient));
                    thread.Start(client);
                }
            } 
        }
        public void Stop()
        {
            _tcpListener.Stop();
            Console.WriteLine("Listener Stopped.");
        }

        private void HandlePacketFromClient(object clientObj)
        {
            Client client = (Client)clientObj;
            _packetHandler.HandleForClient(client);
        }
    }
}
