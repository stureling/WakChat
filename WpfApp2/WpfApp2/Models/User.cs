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
        public event PropertyChangedEventHandler PropertyChanged;

        public User()
        {
            this.Port = 0;
            this.IP = "";
            this.Username = "";

        }

        public int Port { get; set; }

        public string Username { get; set; }

        public string IP { get; set; }

        public void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}