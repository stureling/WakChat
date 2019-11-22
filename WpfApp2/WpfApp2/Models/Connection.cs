using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Windows;
using WpfApp2.Models;

namespace WpfApp2.Viewmodels
{
    public class Connection
    {
        public Connection()
        {
            Client = new TcpClient();
        }

        public TcpClient Client { get; set; }

        public TcpListener Server { get; set; }
    }
}
