using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using WpfApp2.Models;
using WpfApp2.Viewmodels.Commands;

namespace WpfApp2.Viewmodels
{
    public class LoginViewmodel : BaseViewmodel, INotifyPropertyChanged
    {
        public User user;
        private bool _enableConnect;
        private bool _enableListen;

        private Thread connectionThread;
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
            this.user = new User();
        }

        public bool EnableConnect
        {
            get
            {
                return _enableConnect;
            }
            set
            {
                if (String.IsNullOrWhiteSpace(user.Username) || String.IsNullOrWhiteSpace(user.IP) || String.IsNullOrWhiteSpace(user.Port.ToString()))
                {
                    _enableConnect = false;
                }
                else
                {
                    _enableConnect = true;
                }
            }
        }

        public bool EnableListen
        {
            get
            {
                return _enableListen;
            }
            set
            {
                if (String.IsNullOrWhiteSpace(user.Username) || String.IsNullOrWhiteSpace(user.Port.ToString()))
                {
                    _enableListen = false;
                }
                else
                {
                    _enableListen = true;
                }
            }
        }

        public int Port
        {
            get
            {
                return user.Port;
            }
            set
            {
                user.Port = value;
                OnPropertyChanged("Port");
            }
        }

        public void Connect()
        {
            if(connectionThread != null)
            {
                Debug.WriteLine("Thread aborted to make room for new thread");
                connectionThread.Abort();
            }
            connectionThread = new Thread(new ThreadStart(ConnectThread));
            connectionThread.Start();
        }
        public void Listen()
        {
            if(connectionThread != null)
            {
                Debug.WriteLine("Thread aborted to make room for new thread");
                connectionThread.Abort();
            }
            connectionThread = new Thread(new ThreadStart(ListenThread));
            connectionThread.Start();
        }
        public void ConnectThread()
        {
            Connection connection = new Connection();
            connection.Connect(user.Port, user.IP, user.Username);
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
            connection.Listen(user.Port);
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

        public string Username
        {
            get
            {
                return user.Username;
            }
            set
            {
                user.Username = value;
                OnPropertyChanged("Username");
            }
        }
        public string IP
        {
            get
            {
                return user.IP;
            }
            set
            {
                user.IP = value;
                OnPropertyChanged("IP");
            }
        }

        public void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}