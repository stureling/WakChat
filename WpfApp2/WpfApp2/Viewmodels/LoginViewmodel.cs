using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using WpfApp2.Viewmodels.Commands;

namespace WpfApp2.Viewmodels
{
    public class LoginViewmodel : BaseViewmodel
    {

        private int _port;
        private string _username;
        private string _ip;
        private Thread connectionThread;
        public int Port
        {
            get 
            {
                return _port;
            }
            set
            {
                if (value < 1024 || value > 65535)
                {
                    MessageBoxResult result = MessageBox.Show("INVALID PORT", "ERROR", MessageBoxButton.OK);
                    _port = 0;
                }
                else
                {
                    _port = value;
                    Debug.WriteLine(_port);
                }
            }
        }

        public string User
        {
            get
            {
                return _username;
            }
            set
            {
                if (value.Length == 0)
                {
                    _username = "Anon";
                }

                else if (value.Length > 24)
                {
                    MessageBoxResult result = MessageBox.Show("USERNAME TOO BIG", "ERROR", MessageBoxButton.OK);
                    _username = "";
                }

                else
                {
                    _username = value;
                    Debug.WriteLine(_username);
                }
            }
        }

        public string IP
        {
            get
            {
                return _ip;
            }
            set
            {
                if (!Regex.Match(value, @"^(?:(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.){3}(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)$").Success)
                {
                    MessageBoxResult result = MessageBox.Show("INVALID IP", "ERROR", MessageBoxButton.OK);
                    _ip = "";
                }
                else
                {
                    _ip = value;

                    Debug.WriteLine(_ip);
                }
            }
        }

        public ICommand ExitWindowCommand { get; set; }
        public ICommand ConnectCommand { get; set; }
        public ICommand ListenCommand { get; set; }

        public LoginViewmodel()
        {
            this.ExitWindowCommand = new ExitWindowCommand(this);
            this.ListenCommand = new ListenCommand(this);
            this.ConnectCommand = new ConnectCommand(this);
        }

        public void Connect()
        {
            if(connectionThread != null)
            {
                connectionThread.Abort();
            }
            connectionThread = new Thread(new ThreadStart(ConnectThread));
            connectionThread.Start();
        }
        public void Listen()
        {
            if(connectionThread != null)
            {
                connectionThread.Abort();
            }
            connectionThread = new Thread(new ThreadStart(ListenThread));
            connectionThread.Start();
        }
        public void ConnectThread()
        {
            Connection connection = new Connection();
            connection.Connect(_port, _ip, _username);
            if (connection.Success)
            {
                //continue to chat screen
                MessageBox.Show("Continue to chat window", "Alert", MessageBoxButton.OK);
            }
            else
            {
                //stay on this screen
                MessageBox.Show("Staying on this screen", "Alert", MessageBoxButton.OK);
            }
        }
        public void ListenThread()
        {
            Connection connection = new Connection();
            connection.Listen(_port);
            if (connection.Success)
            {
                //continue to chat screen
                MessageBox.Show("Continue to chat window", "Alert", MessageBoxButton.OK);
            }
            else
            {
                //stay on this screen
                MessageBox.Show("Staying on this screen", "Alert", MessageBoxButton.OK);
            }
        }
    }
}
