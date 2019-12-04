using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.Json;
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
        public History history = new History();
   
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
        private String path = AppDomain.CurrentDomain.BaseDirectory + @"history\history.json";
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

        public override void ExitWindow()
        {
            List<Packet> lst = Messages.ToList();
            history.AppendToFile(lst);

            base.ExitWindow();
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
        public void DisplayMessage(Packet message)
        {
            Messages.Add(message);
        }
        public override void ExitWindow()
        {
            Connection.Abort();
            //Thread.Sleep(2000);
            CloseWindow();
        }
    }
}
