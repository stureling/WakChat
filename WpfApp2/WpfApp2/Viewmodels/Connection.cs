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
            Success = false;
        }

        public TcpClient Client { get; set; }

        public TcpListener Server { get; set; }

        public bool Success { get; private set; }

        public void Listen(int port)
        {
            try
            {
                IPAddress localAddr = IPAddress.Parse("127.0.0.1");

                // TcpListener server = new TcpListener(port);
                Server = new TcpListener(localAddr, port);

                // Start listening for client requests.
                Server.Start();

                // Buffer for reading data
                Byte[] bytes = new Byte[256];
                String data = null;

                // Enter the listening loop.
                while (true)
                {
                    Debug.Write("Waiting for a connection... ");

                    //Accept a connection
                    Client = Server.AcceptTcpClient();

                    data = null;

                    // Get a stream object for reading and writing
                    NetworkStream stream = Client.GetStream();

                    int i;
                    i = stream.Read(bytes, 0, bytes.Length);

                    data = Encoding.UTF8.GetString(bytes, 0, i);
                    Debug.WriteLine($"Received: {data}");

                    // Creates message box to inform user of connected 
                    MessageBoxResult result = MessageBox.Show($"User {data} wishes to connect, Accept?", "Alert", MessageBoxButton.YesNo);

                    if (result == MessageBoxResult.Yes)
                    {
                        // Send back a response.                        
                        byte[] msg = System.Text.Encoding.UTF8.GetBytes("accept");
                        stream.Write(msg, 0, msg.Length);
                        Debug.WriteLine($"Sent: {data}");
                        Success = true;
                        break;
                    }
                    else if (result == MessageBoxResult.No)
                    {
                        // Send back a response.                        
                        byte[] msg = System.Text.Encoding.UTF8.GetBytes("deny");
                        stream.Write(msg, 0, msg.Length);
                        Debug.WriteLine($"Sent: {data}");
                        Success = false;
                        Client.Close();
                    }
                }
            }
            catch (SocketException e)
            {
                Console.WriteLine("SocketException: {0}", e);
            }
            finally
            {
                // Stop listening for new clients.
                Server.Stop();
            }
        }

        public void Connect(int port, string ipAddress, string username)
        {
            try
            {
                IPAddress serverIP = IPAddress.Parse(ipAddress);
                Client = new TcpClient();
                Client.Connect(serverIP, port);

                ConnectionJSON json = new ConnectionJSON(username, "EstablishConnection", "request");

                string jsonString = JsonSerializer.Serialize(json);

                // Encode the string to bytearray
                byte[] data = Encoding.UTF8.GetBytes(username);

                // Get a client stream for reading and writing.
                NetworkStream _stream = Client.GetStream();

                //// Send the message to the connected TcpServer. 
                _stream.Write(data, 0, data.Length);

                // Buffer to store the response bytes.
                data = new Byte[256];

                // String to store the response UTF8 representation.
                String responseData = String.Empty;

                //// Read the first batch of the TcpServer response bytes.
                Int32 bytes = _stream.Read(data, 0, data.Length);
                responseData = Encoding.UTF8.GetString(data, 0, bytes);
                Console.WriteLine($"Received: {responseData}");
                if (responseData == "deny")
                {
                    // go back to connection window if denied
                    MessageBox.Show("Connection denied by host", "Alert", MessageBoxButton.OK);
                    Client.Close();
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
        }
        public void Send(string message)
        {
            NetworkStream _stream = Client.GetStream();
        }
    }
}
