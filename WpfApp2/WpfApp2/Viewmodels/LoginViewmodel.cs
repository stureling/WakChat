﻿using System;
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
        private User _user;
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
            this._user = new User();
        }

        public void Connect()
        {
            if(connectionThread != null)
            {
                Debug.WriteLine("Thread aborted to make room for new thread");
                //Freezes UI because abort is waiting for TcpClient _client = Server.AcceptTcpClient(); to finish in Connection.cs
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
            try
            {
                Connection connection = new Connection();
                connection.Connect(User.Port, User.IP, User.Username);
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
            catch(ThreadAbortException e)
            {
                Debug.WriteLine(e);
            }
        }
        public void ListenThread()
        {
            try
            {
                Connection connection = new Connection();
                connection.Listen(User.Port);
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
            catch(ThreadAbortException e)
            {
                Debug.WriteLine(e);
            }
        }

        public User User
        {
            get
            {
                return _user;
            }
            set
            {
                _user = value;
            }
        }

        public void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}