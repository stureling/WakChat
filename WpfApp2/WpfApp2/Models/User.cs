using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;

namespace WpfApp2.Models
{
    public class User: INotifyPropertyChanged
    {
        private int _port;
        private string _username;
        private string _ip;

        public event PropertyChangedEventHandler PropertyChanged;

        public User() { }

        public int Port
        {
            get
            {
                return _port;
            }
            set
            {
                _port = value;
                OnPropertyChanged("Port");
            }
        }

        public string Username
        {
            get
            {
                return _username;
            }
            set
            {
                _username = value;
                OnPropertyChanged("Username");
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
                 _ip = value;
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
