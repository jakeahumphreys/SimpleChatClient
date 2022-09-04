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
 
        public static String serverPrefix = "[Server]: ";
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
                
                Thread thread = new Thread(new ParameterizedThreadStart(ClientMethod));
                thread.Start(client);
            } 
        }
        public void Stop()
        {
            tcpListener.Stop();
            Console.WriteLine("Listener Stopped.");
        }

        private void ClientMethod(object clientObj)
        {
            Client client = (Client)clientObj;

            int noOfIncomingBytes;
            while ((noOfIncomingBytes = client.reader.ReadInt32()) != 0)
            {
                Console.WriteLine(noOfIncomingBytes);

                byte[] bytes = client.reader.ReadBytes(noOfIncomingBytes);
                MemoryStream memStream = new MemoryStream(bytes);
                Packet packet = formatter.Deserialize(memStream) as Packet;

                Console.WriteLine(packet.packetType);
                if (packet.packetType == PacketType.DISCONNECT)
                {
                    clients.Remove(client);
                    _packetHandler.HandlePacket(client, packet);
                    break;
                }
                _packetHandler.HandlePacket(client, packet);
            }
            
            clients.Remove(client);
            SendUserListUpdate();
            client.Close();

        }

        private static void SendUserListUpdate()
        {
            List<string> userlist = new List<string>();

            foreach (Client connectedClient in clients)
            {
                if(connectedClient.clientNickname != null)
                {
                    Console.WriteLine(connectedClient.clientNickname);
                    userlist.Add(connectedClient.clientNickname + "{" + connectedClient.clientStatus + "}");

                }
            }

            foreach(Client connectedClient in clients)
            {
                connectedClient.send(new UserListPacket(userlist));
            }
        }
    }
}
