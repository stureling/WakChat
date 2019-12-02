using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace WpfApp2.Models
{
    public class Message: INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public Message(){}
        public string Username { get; set; }
        public DateTime Time { get; set; }
        public string Msg { get; set; }
        public string toString()
        {
            return JsonSerializer.Serialize(this);
        }
        public void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}