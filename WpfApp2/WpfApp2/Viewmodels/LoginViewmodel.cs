using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using WpfApp2.Models;
using WpfApp2.Viewmodels.Commands;
using WpfApp2.Views;

namespace WpfApp2.Viewmodels
{
    public class LoginViewmodel : BaseViewmodel, INotifyPropertyChanged
    {
        private Thread connectionThread;
        public User User { get; set; }
        public Connection Connection { get; private set; }

        public event PropertyChangedEventHandler PropertyChanged;

        public ICommand ExitWindowCommand { get; set; }
        public ICommand OpenWindowCommand { get; set; }
        public ICommand ConnectCommand { get; set; }
        public ICommand ListenCommand { get; set; }

        public LoginViewmodel()
        {
            this.ExitWindowCommand = new ExitWindowCommand(this);
            this.OpenWindowCommand = new OpenWindowCommand(this);
            this.ListenCommand = new ListenCommand(this);
            this.ConnectCommand = new ConnectCommand(this);
            this.User = new User();
        }

        public void Connect()
        {
            if(connectionThread != null)
            {
                Debug.WriteLine("Thread aborted to make room for new thread");
                //Freezes UI because abort is waiting for TcpClient _client = Server.AcceptTcpClient(); to finish in the threadfunction
                connectionThread.Abort();
                Debug.WriteLine("Thread aborted");
            }
            connectionThread = new Thread(new ThreadStart(ConnectThread));
            connectionThread.Start();
        }
        public void Listen()
        {
            if(connectionThread != null)
            {
                Debug.WriteLine("Starting thread abortion to make room for new thread");
                //Freezes UI because abort is waiting for TcpClient _client = Server.AcceptTcpClient(); to finish in the threadfunction
                connectionThread.Abort();
                Debug.WriteLine("Thread aborted");
            }
            connectionThread = new Thread(new ThreadStart(ListenThread));
            connectionThread.Start();
        }
        public void ConnectThread()
        {
            try
            {
                Connection = new Connection();
                IPAddress serverIP = IPAddress.Parse(User.IP);
                Connection.Client.Connect(serverIP, User.Port);

                ConnectionJSON json = new ConnectionJSON(User.Username, "EstablishConnection", "request");

                string jsonString = JsonSerializer.Serialize(json);

                // Encode the string to bytearray
                byte[] data = Encoding.UTF8.GetBytes(jsonString);

                // Get a client stream for reading and writing.
                NetworkStream stream = Connection.Client.GetStream();

                //// Send the message to the connected TcpServer. 
                stream.Write(data, 0, data.Length);

                // Buffer to store the response bytes.
                data = new Byte[256];

                // String to store the response UTF8 representation.
                String responseData = String.Empty;

                //// Read the first batch of the TcpServer response bytes.
                Int32 bytes = stream.Read(data, 0, data.Length);
                responseData = System.Text.Encoding.UTF8.GetString(data, 0, bytes);
                ConnectionJSON responseJson = JsonSerializer.Deserialize<ConnectionJSON>(responseData);
                Console.WriteLine($"Received: {responseData}");
                if (responseJson.ConnectionTypeValue == "Deny")
                {
                    // go back to connection window if denied
                    MessageBox.Show("Connection denied by host", "Alert", MessageBoxButton.OK);
                    Connection.Client.Close();
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
        public void ListenThread()
        {
            try
            {
                Connection = new Connection();

                IPAddress localAddr = IPAddress.Parse("127.0.0.1");

                // TcpListener server = new TcpListener(port);
                Connection.Server = new TcpListener(localAddr, User.Port);

                // Start listening for client requests.
                Connection.Server.Start();

                // Buffer for reading data
                Byte[] bytes = new Byte[256];
                String data = null;

                Debug.Write("Waiting for a connection... ");
                // Enter the listening loop.
                while (true)
                {
                    if (Connection.Server.Pending())
                    {
                        Connection.Client = Connection.Server.AcceptTcpClient();

                        data = null;

                        // Get a stream object for reading and writing
                        NetworkStream stream = Connection.Client.GetStream();

                        int i;
                        i = stream.Read(bytes, 0, bytes.Length);

                        data = Encoding.UTF8.GetString(bytes, 0, i);
                        Debug.WriteLine($"Received: {data}");
                        ConnectionJSON jsonData = JsonSerializer.Deserialize<ConnectionJSON>(data);

                        // Creates message box to inform user of connected 
                        MessageBoxResult result = MessageBox.Show($"User {jsonData.Username} wishes to connect, Accept?", "Alert", MessageBoxButton.YesNo);

                        if (result == MessageBoxResult.Yes)
                        {
                            // Send back a response.                        
                            ConnectionJSON json = new ConnectionJSON(User.Username, "EstablishConnection", "Accept");
                            string response = JsonSerializer.Serialize(json);
                            byte[] msg = Encoding.UTF8.GetBytes(response);
                            stream.Write(msg, 0, msg.Length);
                            Debug.WriteLine($"Sent: {data}");
                            break;
                        }
                        else if (result == MessageBoxResult.No)
                        {
                            // Send back a response.                        
                            ConnectionJSON json = new ConnectionJSON(User.Username, "EstablishConnection", "Deny");
                            string response = JsonSerializer.Serialize(json);
                            byte[] msg = Encoding.UTF8.GetBytes(response);
                            stream.Write(msg, 0, msg.Length);
                            Debug.WriteLine($"Sent: {data}");
                            Connection.Client.Close();
                            Debug.Write("Waiting for a connection... ");
                        }
                    }
               }
                //Connection has been made
                //StartChat();
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
                Connection.Server.Stop();
            }
        }

        public void StartChat()
        {
            ChatView newChat = new ChatView(Connection);
            newChat.Show();
        }


        public void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}