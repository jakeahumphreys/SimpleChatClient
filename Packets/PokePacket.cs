using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Packets
{
    [Serializable]
    public class PokePacket : Packet
    {
        public String sender;
        public String recipient;
        public PokePacket(string sender,string recipient)
        {
            this.sender = sender;
            this.recipient = recipient;
            this.packettype = PacketType.POKE;
        }
    }
}
