using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using WpfApp2.Models;

namespace WpfApp2.Viewmodels
{
    public class Connection: INotifyPropertyChanged
    {
        public Connection()
        {
            Client = new TcpClient();
            Listening = false;
            Actions = new Hashtable();
            Actions["ConnectionRequest"] = (Action<Packet>) QueryUserOnConnect;
            Actions["ConnectionDeny"] = (Action<Packet>) ConnectionDeny;
            Actions["Disconnect"] = (Action<Packet>) Disconnect;
        }
        private Thread connectionThread;
        private  User User { get; set; }
        public Hashtable Actions { get; set; }
        public TcpClient Client { get; set; }
        public TcpListener Server { get; set; }
        private bool _listening;

        public event PropertyChangedEventHandler PropertyChanged;

        public bool Listening { 
            get
            {
                return _listening;
            }
            set
            {
                _listening = value;
                OnPropertyChanged("Listening");
            }
        }

        public void Connect(User user)
        {
            User = user;
            if(connectionThread != null)
            {
                connectionThread.Abort();
            }
            connectionThread = new Thread(() => ConnectThread());
            connectionThread.Start();
        }
        
        public void Listen(User user)
        {
            User = user;
            if(connectionThread != null)
            {
                connectionThread.Abort();
            }
            connectionThread = new Thread(() => ListenThread());
            connectionThread.Start();
        }
        
        private void ConnectThread()
        {
            try
            {
                IPAddress serverIP = IPAddress.Parse(User.IP);
                Client.Connect(serverIP, User.Port);

                ConnectionPacket request = new ConnectionPacket("ConnectionRequest", User.Username);
                Send(request);

                String responseData = String.Empty;
                responseData = Recieve();
                var response = Newtonsoft.Json.JsonConvert.DeserializeObject<ConnectionPacket>(responseData, new JsonSerializerSettings() { TypeNameHandling = TypeNameHandling.Auto });
                HandlePacket(response);
                Console.WriteLine($"Received: {responseData}");
            }
            catch (ArgumentNullException e)
            {
                Console.WriteLine("ArgumentNullException: {0}", e);
            }
            catch (SocketException e)
            {
                Console.WriteLine("SocketException: {0}", e);
                MessageBox.Show("No host listening on port-IP combination", "Alert", MessageBoxButton.OK);
            }
            catch(ThreadAbortException e)
            {
                Debug.WriteLine(e);
            }
        }

        private void ListenThread()
        {
            try
            {
                IPAddress localAddr = IPAddress.Parse("127.0.0.1");
                Server = new TcpListener(localAddr, User.Port);
                Server.Start();
                String data;
                Listening = true;

                while (Listening)
                {
                    if (Server.Pending())
                    {
                        Client = Server.AcceptTcpClient();
                        data = null;
                        data = Recieve();
                        var packet = Newtonsoft.Json.JsonConvert.DeserializeObject<ConnectionPacket>(data, new JsonSerializerSettings() { TypeNameHandling = TypeNameHandling.Auto });
                        HandlePacket(packet);
                    }
                }
            }
            catch (SocketException e)
            {
                Console.WriteLine("SocketException: {0}", e);
            }
            catch(ThreadAbortException e)
            {
                Debug.WriteLine(e);
            }
        }
        public void Send(Packet message)
        {
            string jsonString = Newtonsoft.Json.JsonConvert.SerializeObject(message);
            byte[] msg = Encoding.UTF8.GetBytes(jsonString);
            Client.GetStream().Write(msg, 0, msg.Length);
        }
        public string Recieve()
        {
            int i;
            Byte[] bytes = new Byte[133769420];
            i = Client.GetStream().Read(bytes, 0, bytes.Length);
            return Encoding.UTF8.GetString(bytes, 0, i);
        }
        public void startReciving()
        {
            connectionThread = new Thread(() => ReciveLoop());
            connectionThread.Start();
        }
        public void ReciveLoop()
        {
            try
            {
                NetworkStream stream = Client.GetStream(); 
                string data;
                while (Client.Connected)
                {
                    if (stream.DataAvailable)
                    {
                        data = Recieve();
                        Packet packet = Newtonsoft.Json.JsonConvert.DeserializeObject<Packet>(data, new JsonSerializerSettings() { TypeNameHandling = TypeNameHandling.Auto });
                        HandlePacket(packet);
                    }
                }
                MessageBox.Show("User disconnected", "Alert", MessageBoxButton.OK);
            }
            catch (SocketException e)
            {
                Console.WriteLine("SocketException: {0}", e);
            }
            catch(ThreadAbortException e)
            {
                Debug.WriteLine(e);
            }
            catch(ObjectDisposedException e)
            {
                Debug.WriteLine(e);
            }
        }
        public void HandlePacket(Packet packet)
        {
            if (Actions.Contains(packet.ConnectionType))
            {
                Action<Packet> action = (Action<Packet>) Actions[packet.ConnectionType];
                Application.Current.Dispatcher.Invoke(() => { action(packet); });
            }
            else
            {
                Debug.WriteLine("No function specified for the packettype");
            }
        }
        public void Abort()
        {
            if (Client.Connected)
            {
                ConnectionPacket disconnecter = new ConnectionPacket("Disconnect", User.Username);
                Send(disconnecter);
                Client.Close();
            }
            if (connectionThread != null)
            {
                connectionThread.Abort();
            }
        }
        public void Disconnect(Packet packet)
        {
            Client.Close();
        }
        public void ConnectionDeny(Packet packet)
        {
            MessageBox.Show("Connection denied by host", "Alert", MessageBoxButton.OK);
            Client.Close();
            Client = new TcpClient();
        }
        public void QueryUserOnConnect(Packet packet)
        {
            MessageBoxResult result = MessageBox.Show($"User {packet.Username} wishes to connect, Accept?", "Alert", MessageBoxButton.YesNo);
            if (result == MessageBoxResult.Yes)
            {
                ConnectionPacket response = new ConnectionPacket("ConnectionAccept", User.Username);
                Send(response);
                Listening = false;
                Action<Packet> action = (Action<Packet>)Actions["ConnectionAccept"];
                Application.Current.Dispatcher.Invoke(() => { action(packet); });
            }
            else if (result == MessageBoxResult.No)
            {
                ConnectionPacket response = new ConnectionPacket("ConnectionDeny", User.Username);
                Send(response);
                Client.Close();
            }
        }
        public void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }    
}
