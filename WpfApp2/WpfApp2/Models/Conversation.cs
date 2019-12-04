using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp2.Models
{
    public class Conversation
    {
        public List<Packet> Convo { get; set; }

        Conversation()
        {
            this.Convo = new List<Packet>();
        }
        public Conversation(List<Packet> packets)
        {
            Convo = packets;
        }
    }
}
