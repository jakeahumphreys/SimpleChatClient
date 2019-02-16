using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Packets
{
    [Serializable]
    public class NickNamePacket:Packet
    {
        public String nickname;

       public NickNamePacket(String nickname)
        {
            this.packettype = PacketType.NICKNAME;
            this.nickname = nickname;
        }
    }
}
