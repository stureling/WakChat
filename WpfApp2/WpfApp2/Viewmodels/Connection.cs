using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace WpfApp2.Viewmodels
{
    class Connection
    {
        private TcpListener _server = null;
        private TcpClient _client;
        private NetworkStream _stream;

        public Connection()
        {

        }

        public TcpClient Client 
        {
            get
            {
                return _client;
            }

            set
            {
                _client = value;
            }
        }

        public TcpListener Server
        {
            get
            {
                return _server;
            }

            set
            {
                _server = value;
            }
        }

        public NetworkStream ServerStream
        {
            get
            {
                return _stream;
            }

            set
            {
                _stream = value;
            }
        }

        public void Listen(int port)
        {
            try
            {
                IPAddress localAddr = IPAddress.Parse("127.0.0.1");

                // TcpListener server = new TcpListener(port);
                _server = new TcpListener(localAddr, port);

                // Start listening for client requests.
                _server.Start();

                // Buffer for reading data
                Byte[] bytes = new Byte[256];
                String data = null;

                // Enter the listening loop.
                while (true)
                {
                    Console.Write("Waiting for a connection... ");

                    // Perform a blocking call to accept requests.
                    // You could also user server.AcceptSocket() here.
                    TcpClient _client = _server.AcceptTcpClient();

                    // Creates message box to inform user of connected 
                    MessageBoxResult result = MessageBox.Show("Potential friend found. Accept?", "Alert", MessageBoxButton.YesNo);

                    if (result == MessageBoxResult.No)
                    {
                        Console.WriteLine("Not accepted");
                        break;
                    }

                    Console.WriteLine("Connected!");

                    data = null;

                    // Get a stream object for reading and writing
                     _stream = _client.GetStream();

                    int i;

                    // Loop to receive all the data sent by the client.
                    while ((i = _stream.Read(bytes, 0, bytes.Length)) != 0)
                    {
                        // Translate data bytes to a ASCII string.
                        data = System.Text.Encoding.ASCII.GetString(bytes, 0, i);
                        Console.WriteLine("Received: {0}", data);

                        // Process the data sent by the client.
                        data = data.ToUpper();

                        byte[] msg = System.Text.Encoding.ASCII.GetBytes(data);

                        // Send back a response.
                        _stream.Write(msg, 0, msg.Length);
                        Console.WriteLine("Sent: {0}", data);
                    }

                    // Shutdown and end connection
                    _server.Stop();
                }
            }
            catch (SocketException e)
            {
                Console.WriteLine("SocketException: {0}", e);
            }
            finally
            {
                // Stop listening for new clients.
                _server.Stop();
            }


            Console.WriteLine("\nHit enter to continue...");
            Console.Read();
        }

        public void Connect(int port, String username)
        {
            try
            {
                // Create a TcpClient.
                // Note, for this client to work you need to have a TcpServer 
                // connected to the same address as specified by the server, port
                // combination.
                TcpClient _client = new TcpClient(username, port);

                // Translate the passed message into ASCII and store it as a Byte array.
                Byte[] data = System.Text.Encoding.ASCII.GetBytes(username);

                // Get a client stream for reading and writing.
                //  Stream stream = client.GetStream();

                NetworkStream _stream = _client.GetStream();

                // Send the message to the connected TcpServer. 
                _stream.Write(data, 0, data.Length);

                Console.WriteLine("Sent: {0}", username);

                // Receive the TcpServer.response.

                // Buffer to store the response bytes.
                data = new Byte[256];

                // String to store the response ASCII representation.
                String responseData = String.Empty;

                // Read the first batch of the TcpServer response bytes.
                Int32 bytes = _stream.Read(data, 0, data.Length);
                responseData = System.Text.Encoding.ASCII.GetString(data, 0, bytes);
                Console.WriteLine("Received: {0}", responseData);

                // Close everything.
                _stream.Close();
                _client.Close();
            }
            catch (ArgumentNullException e)
            {
                Console.WriteLine("ArgumentNullException: {0}", e);
            }
            catch (SocketException e)
            {
                Console.WriteLine("SocketException: {0}", e);
            }

            Console.WriteLine("\n Press Enter to continue...");
            Console.Read();
        }
    }
}
