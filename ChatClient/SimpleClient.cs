using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Threading;
using Packets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;

namespace SimpleClient
{
    public class SimpleClient
    {
        private TcpClient tcpClient;
        private NetworkStream stream;
        public BinaryWriter writer { get; private set; }
        private BinaryReader reader;
        private BinaryFormatter formatter;
        Thread GUIThread;
        ChatForm form;

       public SimpleClient()
        {
            formatter = new BinaryFormatter();
            form = new ChatForm(this);
            tcpClient = new TcpClient();
            GUIThread = new Thread(new ThreadStart(RunChatThread));
        }

        public bool Connect(string ipAddress, int port)
        {
            try
            {
                tcpClient.Connect(ipAddress, port);
                stream = tcpClient.GetStream();
                writer = new BinaryWriter(stream, Encoding.UTF8);
                reader = new BinaryReader(stream, Encoding.UTF8);
                GUIThread.Start();
            }catch(Exception ex)
            {
                Console.WriteLine(ex);
                return false;
            }
            return true;
        }

        public void Run()
        {
            
            Application.Run(form);
            GUIThread.Abort();
            tcpClient.Close();
        }

        void RunChatThread()
        {
            int noOfIncomingBytes;
            while ((noOfIncomingBytes = reader.ReadInt32()) != 0)
            {
                Console.WriteLine("banana");
                byte[] bytes = reader.ReadBytes(noOfIncomingBytes);
                MemoryStream memStream = new MemoryStream(bytes);
                Packet packet = formatter.Deserialize(memStream) as Packet;
                clientPacketHandle(packet);
            }
        }

        private void clientPacketHandle(Packet packet)
        {
            Console.WriteLine(packet.packetType);
            switch (packet.packetType)
            {
                
                case PacketType.CHATMESSAGE:
                    form.UpdateChatWindow(((ChatMessagePacket)packet).message);
                    break;
                case PacketType.SERVER_MESSAGE:
                    form.UpdateChatWindow(((ServerMessagePacket)packet).message);
                    break;
                case PacketType.USERLIST:
                    Console.WriteLine("Recieved userlist");
                    form.currentUsers = ((UserListPacket)packet).userlist;
                    form.DisplayUsers(((UserListPacket)packet).userlist);
                    break;
                case PacketType.POKE:
                    Console.WriteLine("Recieve Poke");
                    string pokeSender = ((PokePacket)packet).sender;
                    string pokeRecipient = ((PokePacket)packet).recipient;
                    PokeForm pokeform = new PokeForm("Poked by: " + pokeSender + "!");
                    pokeform.ShowDialog();
                    break;
                default:
                    form.UpdateChatWindow("[ERROR]: Recieved a packet of unknown type.");
                    Console.WriteLine("Unknown packet type");
                    break;
            }
        }

        public void SendMessage(Packet packet)
        {
            MemoryStream memStream = new MemoryStream();
            formatter.Serialize(memStream, packet);
            byte[] buffer = memStream.GetBuffer();
            writer.Write(buffer.Length);
            writer.Write(buffer);
            writer.Flush();
        }
        public void ProcessServerResponse()
        {
            Console.WriteLine("[Server]:" + reader.Read());
            Console.WriteLine(); //flush??
        }

    }
}
