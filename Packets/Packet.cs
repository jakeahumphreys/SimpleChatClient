using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Packets;
namespace Packets
{
    [Serializable]
    public class Packet
    {
        public PacketType packetType = PacketType.EMPTY;
    }
}
