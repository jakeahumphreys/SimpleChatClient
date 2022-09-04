using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Collections.Generic;
using System.Threading;
using System.Runtime.Serialization.Formatters.Binary;
using Packets;

namespace SimpleServer
{
    public class SimpleServer
    {
        private TcpListener tcpListener;
        private static List<Client> clients;
        private static BinaryFormatter formatter;
 
        private PacketHandler.PacketHandler _packetHandler;

        public SimpleServer(string ipAddress, int port)
        {
            formatter = new BinaryFormatter();
            tcpListener = new TcpListener(IPAddress.Parse(ipAddress), port);
            clients = new List<Client>();
            _packetHandler = new PacketHandler.PacketHandler(clients);
        }

        public void Start()
        { 
            tcpListener.Start();
            Console.WriteLine("Listener Started.");

            while (true)
            {
                Socket socket = tcpListener.AcceptSocket();
                Client client = new Client(socket);
                clients.Add(client);
                
                Console.WriteLine($"Added client {client.clientNickname} successfully.");
                _packetHandler.UpdateConnectedClients(clients);
                
                Thread thread = new Thread(new ParameterizedThreadStart(HandlePacketFromClient));
                thread.Start(client);
            } 
        }
        public void Stop()
        {
            tcpListener.Stop();
            Console.WriteLine("Listener Stopped.");
        }

        private void HandlePacketFromClient(object clientObj)
        {
            Client client = (Client)clientObj;
            _packetHandler.HandleForClient(client);
        }
    }
}
