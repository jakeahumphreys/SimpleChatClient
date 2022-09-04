using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Collections.Generic;
using System.Threading;
using System.Runtime.Serialization.Formatters.Binary;
using Packets;
using System.Runtime.Serialization;

namespace SimpleServer
{
    class SimpleServer
    {
        private TcpListener tcpListener;
        private static List<Client> clients;
        private static BinaryFormatter formatter;
 
        public static String serverPrefix = "[Server]: ";
        public SimpleServer(string ipAddress, int port)
        {
           formatter = new BinaryFormatter();
            IPAddress address = IPAddress.Parse(ipAddress);
            tcpListener = new TcpListener(IPAddress.Parse(ipAddress), port);
            clients = new List<Client>();
        }

        public void Start()
        { 
            tcpListener.Start();
            Console.WriteLine("Listener Started.");

            while (true)
            {
                Socket socket = tcpListener.AcceptSocket();
                Console.WriteLine("Socket Accepted");
                Client client = new Client(socket);
                Console.WriteLine("Adding client");
                clients.Add(client);
                Console.WriteLine(clients.Count);
                Thread thread = new Thread(new ParameterizedThreadStart(ClientMethod));
                thread.Start(client);
            } 
        }
        public void Stop()
        {
            tcpListener.Stop();
            Console.WriteLine("Listener Stopped.");
        }

        private static void ClientMethod(object clientObj)
        {
            
            Client client = (Client)clientObj;

            int noOfIncomingBytes;
            while ((noOfIncomingBytes = client.reader.ReadInt32()) != 0)
            {
                Console.WriteLine(noOfIncomingBytes);

                byte[] bytes = client.reader.ReadBytes(noOfIncomingBytes);
                MemoryStream memStream = new MemoryStream(bytes);
                Packet packet = formatter.Deserialize(memStream) as Packet;

                Console.WriteLine(packet.packettype);

                
                if (packet.packettype == PacketType.DISCONNECT)
                {
                    clients.Remove(client);
                    HandlePacket(client, packet);
                    Console.WriteLine("got here");
                    break;
                }
                HandlePacket(client, packet);
            }
            
            clients.Remove(client);
            userList();
            client.Close();

        }   

        private static void HandlePacket(Client client, Packet packet)
        {
            Console.WriteLine("Transferred to handler");
            switch (packet.packettype)
            {
                case PacketType.USER:
                    client.clientNickname = ((UserPacket)packet).nickname;
                    client.clientStatus = ((UserPacket)packet).status;
                    userList();
                    break;
                case PacketType.CHATMESSAGE:
                    string message = ((ChatMessagePacket)packet).message;
                    ChatMessagePacket chatMessage = new ChatMessagePacket("[" + client.clientNickname +"]: " + message);
                    foreach (Client connectedClient in clients)
                    {
                        connectedClient.send(chatMessage);
                    }
                    break;
                case PacketType.SERVER_MESSAGE:
                    string servermessage = ((ServerMessagePacket)packet).message;
                    ServerMessagePacket serverMessage = new ServerMessagePacket(serverPrefix + servermessage);
                    foreach (Client connectedClient in clients)
                    {
                        connectedClient.send(serverMessage);
                    }
                    break;
                case PacketType.POKE:
                    Console.WriteLine("got poker");
                    string poker = ((PokePacket)packet).poker;
                    string pokee = ((PokePacket)packet).pokee;
                    foreach(Client connectedClient in clients)
                    {
                        if (connectedClient.clientNickname == pokee)
                        {
                            connectedClient.send(new PokePacket(poker,pokee));
                            Console.WriteLine(connectedClient.clientNickname);
                        }
                    }
                    break;
                case PacketType.COMMAND:
                    switch (((CommandPacket)packet).command)
                    {
                        case "!roll":
                            Random rnd = new Random();
                            int roll = rnd.Next(1, 99);
                            ServerMessagePacket rollres = new ServerMessagePacket(serverPrefix + client.clientNickname + " rolled: " + roll);
                            {
                                foreach (Client connectedClient in clients)
                                {
                                    connectedClient.send(rollres);
                                }
                            }
                            break;

                        default:
                            break;
                    }
                    break;
                case PacketType.DISCONNECT:
                    foreach (Client c in clients)
                    {
                        Console.WriteLine("Sending");
                        c.send(new ServerMessagePacket(serverPrefix + client.clientNickname + " has left the server."));
                    }
                    break;

            }

        }

        private static void userList()
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
                Console.WriteLine("Sending userlist");
                connectedClient.send(new UserListPacket(userlist));
            }
        }
    }
}
