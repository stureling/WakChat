using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace WpfApp2.Models
{
    public class Packet
    {
        public string Username { get; set; }
        public string ConnectionType { get; set; }
        public string ConnectionTypeValue { get; set; }
        public DateTime Time { get; set; }
        public Packet() { }
        public string toString()
        {
            return JsonSerializer.Serialize(this);
        }
    }
}
