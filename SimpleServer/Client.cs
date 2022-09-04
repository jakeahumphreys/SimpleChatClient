using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Net;
using System.Net.Sockets;
using Packets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;

namespace SimpleServer
{

    public class Client
    {
       private Socket socket;
       private NetworkStream stream;

       public String clientNickname { get; set; }
       public String clientStatus { get; set; }
       public BinaryReader reader { private set; get; }
       public BinaryWriter writer { private set; get; }
       private BinaryFormatter formatter;
       

        
        public Client(Socket socketin)
        {
            socket = socketin;
            
            stream = new NetworkStream(socket);
            formatter = new BinaryFormatter();
            reader = new BinaryReader(stream, Encoding.UTF8);
            writer = new BinaryWriter(stream, Encoding.UTF8);

        }

        public void send(Packet spacket)

        {
            Console.WriteLine("Serializing packet");
            MemoryStream memStream = new MemoryStream();
            formatter.Serialize(memStream, spacket);
            byte[] buffer = memStream.GetBuffer();
            writer.Write(buffer.Length);
            writer.Write(buffer);
            writer.Flush();

        }

        public void Close()
        {
            writer.Write(0);
            stream.Close();
            socket.Close();
        }
    }
}
