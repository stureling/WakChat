using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
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
    public class Connection
    {
        public Connection()
        {
            Client = new TcpClient();
            Listening = false;
            Actions = new Hashtable();
            Actions["ConnectionRequest"] = (Action<Packet>) QueryUserOnConnect;
            Actions["ConnectionDeny"] = (Action<Packet>) ConnectionDeny;
        }
        private Thread connectionThread;
        private  User User { get; set; }
        public Hashtable Actions { get; set; }
        public TcpClient Client { get; set; }
        public TcpListener Server { get; set; }
        public bool Listening { get; set; }

        public void Connect(User user)
        {
            User = user;
            if(connectionThread != null)
            {
                Debug.WriteLine("Thread aborted to make room for new thread");
                connectionThread.Abort();
                Debug.WriteLine("Thread aborted");
            }
            connectionThread = new Thread(() => ConnectThread());
            connectionThread.Start();
        }
        
        public void Listen(User user)
        {
            User = user;
            if(connectionThread != null)
            {
                Debug.WriteLine("Starting thread abortion to make room for new thread");
                connectionThread.Abort();
                Debug.WriteLine("Thread aborted");
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

                Packet json = new Packet() { ConnectionTypeValue = "", ConnectionType ="ConnectionRequest", Username = User.Username, Time = DateTime.Now };
                Send(json);

                String responseData = String.Empty;
                responseData = Recieve();
                Packet response= JsonSerializer.Deserialize<Packet>(responseData);
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
                Debug.Write("Waiting for a connection... ");
                Listening = true;

                while (Listening)
                {
                    if (Server.Pending())
                    {
                        Client = Server.AcceptTcpClient();
                        data = null;
                        data = Recieve();
                        Packet packet = JsonSerializer.Deserialize<Packet>(data);
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
            string jsonString = JsonSerializer.Serialize(message);
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
                        Debug.WriteLine("Message pending!");
                        data = Recieve();
                        Packet converted_data = JsonSerializer.Deserialize<Packet>(data);
                        HandlePacket(converted_data);
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
            catch(ObjectDisposedException e)
            {
                Debug.WriteLine(e);
            }
        }
        public void HandlePacket(Packet packet)
        {
            Debug.WriteLine($"Handling packet: {JsonSerializer.Serialize(packet)}");
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
                Packet p = new Packet() { ConnectionType = "Disconnect", ConnectionTypeValue = "Disconnect", Time = DateTime.Now, Username = User.Username };
                Send(p);
            }
            if (connectionThread != null)
            {
                connectionThread.Abort();
            }
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
                Packet json = new Packet() { ConnectionTypeValue = "", ConnectionType ="ConnectionAccept", Username = User.Username, Time = DateTime.Now };
                Send(json);
                Listening = false;
                Action<Packet> action = (Action<Packet>)Actions["ConnectionAccept"];
                Application.Current.Dispatcher.Invoke(() => { action(packet); });
            }
            else if (result == MessageBoxResult.No)
            {
                Packet json = new Packet() { ConnectionTypeValue = "", ConnectionType ="ConnectionDeny", Username = User.Username, Time = DateTime.Now };
                Send(json);
                Client.Close();
            }
        }
    }    
}
