using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Packets
{
    [Serializable]
    public class ChatMessagePacket:Packet
    {
        public string message;

        public ChatMessagePacket(String message)
        {
            this.packetType = PacketType.CHATMESSAGE;
            this.message = message;
        }
    }
}
