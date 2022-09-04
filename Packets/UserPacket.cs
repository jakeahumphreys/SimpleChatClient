using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Packets
{
    [Serializable]
    public class UserPacket:Packet
    {
        public string nickname;
        public string status;

        public UserPacket(string nickname, string status)
        {
            this.nickname = nickname;
            this.status = status;

            packetType = PacketType.USER;
        }
    }
}
