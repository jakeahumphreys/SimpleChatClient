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
        public String poker;
        public String pokee;
        public PokePacket(string poker,string pokee)
        {
            this.poker = poker;
            this.pokee = pokee;
            this.packettype = PacketType.POKE;
            
        }
    }
}
