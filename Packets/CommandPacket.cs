using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Packets
{
    [Serializable]
    public class CommandPacket : Packet
    {
        public string command;

        public CommandPacket(String command)
        {
            this.packettype = PacketType.COMMAND;
            this.command = command;
        }
    }
}
