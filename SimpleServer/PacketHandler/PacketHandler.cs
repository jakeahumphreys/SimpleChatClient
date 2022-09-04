using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Packets;

namespace SimpleServer.PacketHandler
{
    public class PacketHandler
    {
        private List<Client> _connectedClients;
        
        private const string ServerPrefix = "[Server]: ";


        public PacketHandler(List<Client> connectedClients)
        {
            _connectedClients = connectedClients;
        }

        public void UpdateConnectedClients(List<Client> connectedClients)
        {
            _connectedClients = connectedClients;
        }

        public void HandleForClient(Client client)
        {
            int noOfIncomingBytes;
            while ((noOfIncomingBytes = client.reader.ReadInt32()) != 0)
            {
                Console.WriteLine(noOfIncomingBytes);

                byte[] bytes = client.reader.ReadBytes(noOfIncomingBytes);
                MemoryStream memStream = new MemoryStream(bytes);
                var formatter = new BinaryFormatter();
                Packet packet = formatter.Deserialize(memStream) as Packet;

                if (packet.packetType == PacketType.DISCONNECT)
                {
                    _connectedClients.Remove(client);
                    HandlePacket(client, packet);
                    break;
                }
                HandlePacket(client, packet);
            }
            
            _connectedClients.Remove(client);
            SendUpdatedUserList();
            client.Close();
        }

        private void HandlePacket(Client client, Packet packet)
        {
            switch (packet.packetType)
            {
                case PacketType.USER:
                    client.clientNickname = ((UserPacket) packet).nickname;
                    client.clientStatus = ((UserPacket) packet).status;
                    SendUpdatedUserList();
                    break;
                case PacketType.CHATMESSAGE:
                    string message = ((ChatMessagePacket) packet).message;
                    ChatMessagePacket chatMessage =
                        new ChatMessagePacket("[" + client.clientNickname + "]: " + message);
                    foreach (Client connectedClient in _connectedClients)
                    {
                        connectedClient.send(chatMessage);
                    }

                    break;
                case PacketType.SERVER_MESSAGE:
                    string servermessage = ((ServerMessagePacket) packet).message;
                    ServerMessagePacket serverMessage = new ServerMessagePacket(ServerPrefix + servermessage);
                    foreach (Client connectedClient in _connectedClients)
                    {
                        connectedClient.send(serverMessage);
                    }

                    break;
                case PacketType.POKE:
                    Console.WriteLine("got poker");
                    string pokeSender = ((PokePacket) packet).sender;
                    string pokeRecipient = ((PokePacket) packet).recipient;
                    foreach (Client connectedClient in _connectedClients)
                    {
                        if (connectedClient.clientNickname == pokeRecipient)
                        {
                            connectedClient.send(new PokePacket(pokeSender, pokeRecipient));
                            Console.WriteLine(connectedClient.clientNickname);
                        }
                    }

                    break;
                case PacketType.COMMAND:
                    switch (((CommandPacket) packet).command)
                    {
                        case "!roll":
                            Random rnd = new Random();
                            int roll = rnd.Next(1, 99);
                            ServerMessagePacket rollres =
                                new ServerMessagePacket(ServerPrefix + client.clientNickname + " rolled: " + roll);
                        {
                            foreach (Client connectedClient in _connectedClients)
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
                    foreach (Client c in _connectedClients)
                    {
                        Console.WriteLine("Sending");
                        c.send(new ServerMessagePacket(ServerPrefix + client.clientNickname + " has left the server."));
                    }

                    break;
            }
        }
        
        private void SendUpdatedUserList()
        {
            List<string> userlist = new List<string>();

            foreach (Client connectedClient in _connectedClients)
            {
                if(connectedClient.clientNickname != null)
                {
                    Console.WriteLine(connectedClient.clientNickname);
                    userlist.Add(connectedClient.clientNickname + "{" + connectedClient.clientStatus + "}");

                }
            }

            foreach(Client connectedClient in _connectedClients)
            {
                Console.WriteLine("Sending userlist");
                connectedClient.send(new UserListPacket(userlist));
            }
        }
    }
}