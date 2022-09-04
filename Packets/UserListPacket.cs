using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Packets
{
    [Serializable]
    public class UserListPacket : Packet
    {
        public List<string> userlist;
        public UserListPacket(List<string> list)
        {
           
            this.userlist = list;
            packetType = PacketType.USERLIST;
        }
    }

}
