using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
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
        private Connection connection;
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
        public ObservableCollection<MessageJSON> Messages { get; set; }
        public ICommand ExitWindowCommand { get; set; }
        public ICommand OpenWindowCommand { get; set; }
        public ICommand SendCommand { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        public ChatViewmodel(Connection connection, User user, Window window): base(window)
        {
            this.ExitWindowCommand = new ExitWindowCommand(this);
            this.OpenWindowCommand = new OpenWindowCommand(this);
            this.SendCommand = new SendCommand(this);
            this.connection = connection;
            this.User = user;
            this.ThisMsg = "";
            this.Messages = new ObservableCollection<MessageJSON>();
            connection.startReciving(DisplayMessage);
        }

        public void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }

        public void SendMessage()
        {
            MessageJSON mess = new MessageJSON() { ConnectionType = "Message", ConnectionTypeValue = ThisMsg, Username = User.Username, Time = DateTime.Now };
            connection.Send(mess);
            Messages.Add(mess);
            ThisMsg = "";
        }
        public void DisplayMessage(MessageJSON messagee)
        {
            Messages.Add(messagee);
        }
    }
}
