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
        }
        private Thread connectionThread;
        public TcpClient Client { get; set; }

        public TcpListener Server { get; set; }
        public void Connect(User user, Action callbackSuccess = null)
        {
            if(connectionThread != null)
            {
                Debug.WriteLine("Thread aborted to make room for new thread");
                connectionThread.Abort();
                Debug.WriteLine("Thread aborted");
            }
            connectionThread = new Thread(() => ConnectThread(user, callbackSuccess));
            connectionThread.Start();
        }
        
        public void Listen(User user, Action callbackSuccess = null)
        {
            if(connectionThread != null)
            {
                Debug.WriteLine("Starting thread abortion to make room for new thread");
                connectionThread.Abort();
                Debug.WriteLine("Thread aborted");
            }
            connectionThread = new Thread(() => ListenThread(user, callbackSuccess));
            connectionThread.Start();
        }
        
        private void ConnectThread(User user, Action callbackSuccess = null)
        {
            try
            {
                IPAddress serverIP = IPAddress.Parse(user.IP);
                Client.Connect(serverIP, user.Port);
                Message outgoing = new Message() { Msg = "request", Username = user.Username, Time = DateTime.Now };

                MessageJSON json = new MessageJSON(outgoing, "EstablishConnection");
                Send(json);

                String responseData = String.Empty;
                responseData = Recieve();
                MessageJSON responseJson = JsonSerializer.Deserialize<MessageJSON>(responseData);
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

        private void ListenThread(User user, Action callbackSuccess)
        {
            try
            {
                IPAddress localAddr = IPAddress.Parse("127.0.0.1");
                Server = new TcpListener(localAddr, user.Port);
                Server.Start();
                
                // Buffer for reading data
                Byte[] bytes = new Byte[256];
                String data;

                Debug.Write("Waiting for a connection... ");
                bool loop = true;
                while (loop)
                {
                    if (Server.Pending())
                    {
                        Client = Server.AcceptTcpClient();
                        
                        data = null;

                        // Get a stream object for reading and writing
                        data = Recieve();
                        Debug.WriteLine($"Received: {data}");
                        MessageJSON jsonData = JsonSerializer.Deserialize<MessageJSON>(data);

                        // Creates message box to inform user of connected 
                        MessageBoxResult result = MessageBox.Show($"User {jsonData.Username} wishes to connect, Accept?", "Alert", MessageBoxButton.YesNo);
                        
                        if (result == MessageBoxResult.Yes)
                        {
                            Message outgoing = new Message() { Msg = "Accept", Username = user.Username, Time = DateTime.Now };                        
                            MessageJSON json = new MessageJSON(outgoing, "EstablishConnection");
                            Send(json);
                            loop = false;
                        }
                        
                        else if (result == MessageBoxResult.No)
                        {
                            Message outgoing = new Message() { Msg = "Deny", Username = user.Username, Time = DateTime.Now };              
                            MessageJSON json = new MessageJSON(outgoing, "EstablishConnection");
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
        public void Send(MessageJSON message)
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
        public void startReciving(Action<MessageJSON> callbackSuccess)
        {
            connectionThread = new Thread(() => ReciveLoop(callbackSuccess));
            connectionThread.Start();
        }
        public void ReciveLoop(Action<MessageJSON> callbackSuccess)
        {
            Debug.WriteLine("started looping");
            // RECIEVE DATA
            NetworkStream stream = Client.GetStream(); 
            string data;
            while (true)
            {
                if (stream.DataAvailable)
                {
                    data = Recieve();
                    MessageJSON converted_data = JsonSerializer.Deserialize<MessageJSON>(data);
                    //Gör om data till det object som förväntas och skicka det med en dispatcher till viewmodellen
                    Application.Current.Dispatcher.Invoke(() => { callbackSuccess(converted_data); });

                }
            }
        }
    }    
}
