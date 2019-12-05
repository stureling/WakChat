using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp2.Models
{
    public class Conversation
    {
        public List<Packet> Packet { get; set; }
        public string ID { get; set; }

        
        public Conversation()
        {
            /*Packet = new List<Packet>();
            ID = "";*/
        }
        public Conversation(List<Packet> packets, String username)
        {
            String date = DateTime.Now.Date.ToShortDateString();
            ID = username + " " + date;
            Packet = packets;
        }
    }
}
