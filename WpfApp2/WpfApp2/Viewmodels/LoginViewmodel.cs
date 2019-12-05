using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
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
using System.Windows.Media.Imaging;
using WpfApp2.Models;
using WpfApp2.Viewmodels.Commands;
using WpfApp2.Views;

namespace WpfApp2.Viewmodels
{
    public class LoginViewmodel : BaseViewmodel, INotifyPropertyChanged
    {
        public User User { get; set; }
        public Connection Connection { get; private set; }

        public event PropertyChangedEventHandler PropertyChanged;

        public ICommand ExitWindowCommand { get; set; }
        public ICommand OpenWindowCommand { get; set; }
        public ICommand ConnectCommand { get; set; }
        public ICommand ListenCommand { get; set; }

        public LoginViewmodel(Window window): base(window)
        {
            this.ExitWindowCommand = new ExitWindowCommand(this);
            this.OpenWindowCommand = new OpenWindowCommand(this);
            this.ConnectCommand = new ConnectCommand(this);
            this.ListenCommand = new ListenCommand(this);
            this.User = new User();
            this.Connection = new Connection();
            Connection.Actions["ConnectionAccept"] = (Action<Packet>) StartChat;
        }

        public void Connect()
        {
            Connection.Connect(User);
        }

        public void Listen()
        {
            Connection.Listen(User);
        }

        public void StartChat(Packet packet)
        {
            ChatView newChat = new ChatView(Connection, User);
            newChat.Show();
            base.ExitWindow();
        }
        public override void ExitWindow()
        {
            Connection.Abort();
            base.ExitWindow();
        }


        public void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}