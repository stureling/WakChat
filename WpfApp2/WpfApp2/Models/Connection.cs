using System;
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
        }
        private Thread connectionThread;
        private  User User { get; set; }
        public TcpClient Client { get; set; }
        public TcpListener Server { get; set; }
        public bool Listening { get; set; }

        public void Connect(User user, Action callbackSuccess = null)
        {
            User = user;
            if(connectionThread != null)
            {
                Debug.WriteLine("Thread aborted to make room for new thread");
                connectionThread.Abort();
                Debug.WriteLine("Thread aborted");
            }
            connectionThread = new Thread(() => ConnectThread(callbackSuccess));
            connectionThread.Start();
        }
        
        public void Listen(User user, Action callbackSuccess = null)
        {
            User = user;
            if(connectionThread != null)
            {
                Debug.WriteLine("Starting thread abortion to make room for new thread");
                connectionThread.Abort();
                Debug.WriteLine("Thread aborted");
            }
            connectionThread = new Thread(() => ListenThread(callbackSuccess));
            connectionThread.Start();
        }
        
        private void ConnectThread(Action callbackSuccess = null)
        {
            try
            {
                IPAddress serverIP = IPAddress.Parse(User.IP);
                Client.Connect(serverIP, User.Port);

                Packet json = new Packet() { ConnectionTypeValue = "request", ConnectionType ="EstablishConnection", Username = User.Username, Time = DateTime.Now };
                Send(json);

                String responseData = String.Empty;
                responseData = Recieve();
                Packet responseJson = JsonSerializer.Deserialize<Packet>(responseData);
                Console.WriteLine($"Received: {responseData}");

                if (responseJson.ConnectionTypeValue == "Deny")
                {
                    MessageBox.Show("Connection denied by host", "Alert", MessageBoxButton.OK);
                    Client.Close();
                }
                else if (responseJson.ConnectionTypeValue == "Accept")
                {
                    if (callbackSuccess != null)
                    {
                        Application.Current.Dispatcher.Invoke(callbackSuccess);
                    }
                }
                else
                {
                    Debug.WriteLine("wierd value");
                }
            }
            catch (ArgumentNullException e)
            {
                Console.WriteLine("ArgumentNullException: {0}", e);
            }
            catch (SocketException e)
            {
                Console.WriteLine("SocketException: {0}", e);
                MessageBox.Show("No host listening on port-IP combination", "Alert", MessageBoxButton.OK);
                //Tell the user that the connection was refused. Probably due to no host listening on the provided port and IP combination.
            }
            catch(ThreadAbortException e)
            {
                Debug.WriteLine(e);
            }
        }

        private void ListenThread(Action callbackSuccess)
        {
            try
            {
                IPAddress localAddr = IPAddress.Parse("127.0.0.1");
                Server = new TcpListener(localAddr, User.Port);
                Server.Start();
                
                // Buffer for reading data
                Byte[] bytes = new Byte[256];
                String data;

                Debug.Write("Waiting for a connection... ");
                Listening = true;
                while (Listening)
                {
                    if (Server.Pending())
                    {
                        Client = Server.AcceptTcpClient();
                        
                        data = null;

                        // Get a stream object for reading and writing
                        data = Recieve();
                        Debug.WriteLine($"Received: {data}");
                        Packet jsonData = JsonSerializer.Deserialize<Packet>(data);

                        // Creates message box to inform user of connected 
                        MessageBoxResult result = MessageBox.Show($"User {jsonData.Username} wishes to connect, Accept?", "Alert", MessageBoxButton.YesNo);
                        
                        if (result == MessageBoxResult.Yes)
                        {
                            Packet json = new Packet() { ConnectionTypeValue = "Accept", ConnectionType ="EstablishConnection", Username = User.Username, Time = DateTime.Now };
                            Send(json);
                            Listening = false;
                        }
                        
                        else if (result == MessageBoxResult.No)
                        {
                            Packet json = new Packet() { ConnectionTypeValue = "Deny", ConnectionType ="EstablishConnection", Username = User.Username, Time = DateTime.Now };
                            Send(json);
                            Debug.WriteLine($"Sent: {data}");
                            Client.Close();
                            Debug.Write("Waiting for a connection... ");
                        }
                    }
                }
                if (callbackSuccess != null)
                {
                    Application.Current.Dispatcher.Invoke(callbackSuccess);
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
            finally
            {
                Server.Stop();
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
            Byte[] bytes = new Byte[256];
            i = Client.GetStream().Read(bytes, 0, bytes.Length);
            return Encoding.UTF8.GetString(bytes, 0, i);
        }
        public void startReciving(Action<Packet> action)
        {
            connectionThread = new Thread(() => ReciveLoop(action));
            connectionThread.Start();
        }
        public void ReciveLoop(Action<Packet> action)
        {
            try
            {
                NetworkStream stream = Client.GetStream(); 
                string data;
                while (Client.Connected)
                {
                    Debug.WriteLine("Checking for message...");
                    if (stream.DataAvailable)
                    {
                        Debug.WriteLine("Message pending!");
                        data = Recieve();
                        Packet converted_data = JsonSerializer.Deserialize<Packet>(data);
                        if (converted_data.ConnectionType == "Disconnect")
                        {
                            Client.Close();
                            MessageBoxResult result = MessageBox.Show($"User disconnected", "Alert", MessageBoxButton.OK);
                        }
                        else if (converted_data.ConnectionType == "Message")
                        {
                            Application.Current.Dispatcher.Invoke(() => { action(converted_data); });
                        }
                        else
                        {
                            Debug.WriteLine("Detta var ju inte bra, ett meddelande hade fel header");

                        }
                    }
                    Thread.Sleep(500);
                }
            }
            catch (SocketException e)
            {
                Console.WriteLine("SocketException: {0}", e);
            }
            catch(ThreadAbortException e)
            {
                Debug.WriteLine(e);
                Debug.Write("yeehaw");
            }
            catch(ObjectDisposedException e)
            {
                Debug.Write("yeehaw");
            }
        }
        public void PacketHandler(Packet packet)
        {
            
        }
        public void Abort()
        {
            if (Client.Connected)
            {
            Packet p = new Packet() { ConnectionType = "Disconnect", ConnectionTypeValue = "Disconnect", Time = DateTime.Now, Username = User.Username };
            Send(p);
            }
            connectionThread.Abort();
        }
    }    
}
