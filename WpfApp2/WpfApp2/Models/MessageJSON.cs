using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace WpfApp2.Models
{
    class MessageJSON
    {
        public string Username { get; set; }
        public string ConnectionType { get; set; }
        public string ConnectionTypeValue { get; set; }
        public DateTime Time { get; set; }
        public MessageJSON() { }
        public MessageJSON(string username, string connectionType, string connectionTypeValue)
        {
            Username = username;
            ConnectionType = connectionType;
            ConnectionTypeValue = connectionTypeValue;
            Time = DateTime.Now;
        }
        public MessageJSON(string username, DateTime time, string connectionType, string connectionTypeValue)
        {
            Username = username;
            ConnectionType = connectionType;
            ConnectionTypeValue = connectionTypeValue;
            Time = time;
        }
        public string toString()
        {
            return JsonSerializer.Serialize(this);
        }
    }
}
