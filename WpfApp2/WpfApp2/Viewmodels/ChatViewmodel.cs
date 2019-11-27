using System;
using System.Collections.Generic;
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
        public User User { get; set; }
        public string _msg;
        public ICommand ExitWindowCommand { get; set; }
        public ICommand OpenWindowCommand { get; set; }
        public ICommand SendCommand { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        public ChatViewmodel(Connection connection, User user)
        {
            this.ExitWindowCommand = new ExitWindowCommand(this);
            this.OpenWindowCommand = new OpenWindowCommand(this);
            this.connection = connection;
            this.User = user;
        }

        public void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }

        public void SendMessage()
        {
            connection.Send(User, _msg);
        }
    }
}
