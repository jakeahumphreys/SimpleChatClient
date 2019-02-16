using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Packets
{
    [Serializable]
    public class ServerMessagePacket : Packet
    {
        public string message;

        public ServerMessagePacket(String message)
        {
            this.packettype = PacketType.SERVER_MESSAGE;
            this.message = message;
        }
    }
}
