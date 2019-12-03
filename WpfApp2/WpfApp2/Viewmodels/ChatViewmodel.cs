using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using WpfApp2.Models;
using WpfApp2.Viewmodels.Commands;
using WpfApp2.Views;

namespace WpfApp2.Viewmodels
{
    public class ChatViewmodel : BaseViewmodel, INotifyPropertyChanged
    {
        private Connection Connection { get; set; }
        private string _user;

        public User User { get; set; }
   
        public string ThisMsg
        {
            get
            {
                return _user;
            }
            set
            {
                _user = value;
                OnPropertyChanged(ThisMsg);
            }
        }
        public ObservableCollection<Packet> Messages { get; set; }
        public ICommand ExitWindowCommand { get; set; }
        public ICommand OpenWindowCommand { get; set; }
        public ICommand SendCommand { get; set; }
        public event PropertyChangedEventHandler PropertyChanged;

        public ChatViewmodel(Connection connection, User user, Window window): base(window)
        {
            this.ExitWindowCommand = new ExitWindowCommand(this);
            this.OpenWindowCommand = new OpenWindowCommand(this);
            this.SendCommand = new SendCommand(this);
            this.Connection = connection;
            this.User = user;
            this.ThisMsg = "";
            this.Messages = new ObservableCollection<Packet>();
            connection.startReciving(DisplayMessage);
        }

        public void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
        public void SendMessage()
        {
            Packet mess = new Packet() { ConnectionType = "Message", ConnectionTypeValue = ThisMsg, Username = User.Username, Time = DateTime.Now };
            Connection.Send(mess);
            Messages.Add(mess);
            ThisMsg = "";
        }
        public void DisplayMessage(Packet messagee)
        {
            Messages.Add(messagee);
        }
        public override void ExitWindow()
        {
            Connection.Abort();
            //Thread.Sleep(2000);
            CloseWindow();
        }
    }
}
