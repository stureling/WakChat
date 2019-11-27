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
        public void Connect(User user)
        {
            if(connectionThread != null)
            {
                Debug.WriteLine("Thread aborted to make room for new thread");
                connectionThread.Abort();
                Debug.WriteLine("Thread aborted");
            }
            connectionThread = new Thread(() => ConnectThread(user));
            connectionThread.Start();
        }
        
        public void Listen(User user)
        {
            if(connectionThread != null)
            {
                Debug.WriteLine("Starting thread abortion to make room for new thread");
                connectionThread.Abort();
                Debug.WriteLine("Thread aborted");
            }
            connectionThread = new Thread(() => ListenThread(user));
            connectionThread.Start();
        }
        
        private void ConnectThread(User user)
        {
            try
            {
                IPAddress serverIP = IPAddress.Parse(user.IP);
                Client.Connect(serverIP, user.Port);

                MessageJSON json = new MessageJSON(user.Username, "EstablishConnection", "request");
                string jsonString = JsonSerializer.Serialize(json);

                // Encode the string to bytearray
                byte[] data = Encoding.UTF8.GetBytes(jsonString);

                // Get a client stream for reading and writing.
                NetworkStream stream = Client.GetStream();

                //// Send the message to the connected TcpServer. 
                stream.Write(data, 0, data.Length);

                // Buffer to store the response bytes.
                data = new Byte[256];

                // String to store the response UTF8 representation.
                String responseData = String.Empty;

                //// Read the first batch of the TcpServer response bytes.
                Int32 bytes = stream.Read(data, 0, data.Length);
                responseData = System.Text.Encoding.UTF8.GetString(data, 0, bytes);
                MessageJSON responseJson = JsonSerializer.Deserialize<MessageJSON>(responseData);
                Console.WriteLine($"Received: {responseData}");

                if (responseJson.ConnectionTypeValue == "Deny")
                {
                    // go back to connection window if denied
                    MessageBox.Show("Connection denied by host", "Alert", MessageBoxButton.OK);
                    Client.Close();
                }
                else if (responseJson.ConnectionTypeValue == "Accept")
                {
                    // continue to next window
                    //StartChat();
                    MessageBox.Show("Connection accepted by host, continue to chat window", "Alert", MessageBoxButton.OK);
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

        private void ListenThread(User user)
        {
            try
            {
                IPAddress localAddr = IPAddress.Parse("127.0.0.1");
                
                // TcpListener server = new TcpListener(port);
                Server = new TcpListener(localAddr, user.Port);
                
                // Start listening for client requests.
                Server.Start();
                
                // Buffer for reading data
                Byte[] bytes = new Byte[256];
                String data = null;

                Debug.Write("Waiting for a connection... ");

                // Enter the listening loop.
                while (true)
                {
                    if (Server.Pending())
                    {
                        Client = Server.AcceptTcpClient();
                        
                        data = null;

                        // Get a stream object for reading and writing
                        NetworkStream stream = Client.GetStream();

                        int i;
                        
                        i = stream.Read(bytes, 0, bytes.Length);

                        
                        data = Encoding.UTF8.GetString(bytes, 0, i);
                        Debug.WriteLine($"Received: {data}");
                        MessageJSON jsonData = JsonSerializer.Deserialize<MessageJSON>(data);

                        // Creates message box to inform user of connected 
                        MessageBoxResult result = MessageBox.Show($"User {jsonData.Username} wishes to connect, Accept?", "Alert", MessageBoxButton.YesNo);
                        
                        if (result == MessageBoxResult.Yes)
                        {
                            // Send back a response.                        
                            MessageJSON json = new MessageJSON(user.Username, "EstablishConnection", "Accept");
                            string response = JsonSerializer.Serialize(json);
                            byte[] msg = Encoding.UTF8.GetBytes(response);
                            stream.Write(msg, 0, msg.Length);
                            Debug.WriteLine($"Sent: {data}");
                            break;
                        }
                        
                        else if (result == MessageBoxResult.No)
                        {
                            // Send back a response.                        
                            MessageJSON json = new MessageJSON(user.Username, "EstablishConnection", "Deny");
                            string response = JsonSerializer.Serialize(json);
                            byte[] msg = Encoding.UTF8.GetBytes(response);
                            stream.Write(msg, 0, msg.Length);
                            Debug.WriteLine($"Sent: {data}");
                            
                            Client.Close();
                            Debug.Write("Waiting for a connection... ");
                        }
                    }
                }
                MessageBox.Show("Connection made, continue to chat window", "Alert", MessageBoxButton.OK);
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
        public void Send(User user, string message)
        {
            MessageJSON json = new MessageJSON(user.Username, "Message", message);
            string jsonString = JsonSerializer.Serialize(json);

            NetworkStream stream = Client.GetStream();

            byte[] msg = Encoding.UTF8.GetBytes(jsonString);
            stream.Write(msg, 0, msg.Length);

        }
        public void ReciveLoop()
        {

            Byte[] bytes = new Byte[256];
            NetworkStream stream = Client.GetStream();
            int i;
            string data;

            while ((i = stream.Read(bytes, 0, bytes.Length)) != 0)
            {
                data = Encoding.UTF8.GetString(bytes, 0, i);
                Console.WriteLine($"Received: {data}");

                byte[] msg = Encoding.UTF8.GetBytes(data);
            }
        }
    }    
}
