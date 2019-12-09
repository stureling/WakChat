//using Newtonsoft.Json;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace WpfApp2.Models
{
    public class History : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        //public List<Conversation> Histories { get; private set; }
        private String path = AppDomain.CurrentDomain.BaseDirectory + @"history\history.json";
        public List<Conversation> Histories;

        public History()
        {
            Histories = ReadFromFile();
        }

        public List<Conversation> ReadFromFile()
        {
            //List<Conversation> Histories;
            try
            {
                List<Conversation> lst = new List<Conversation>();
                using (StreamReader r = new StreamReader(path))
                {
                    string json = r.ReadToEnd();
                    lst = JsonConvert.DeserializeObject<List<Conversation>>(json, new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All });
                    //lst = JsonSerializer.Deserialize<List<Conversation>>(json);
                    Debug.WriteLine(lst);
                }
            return lst;
            }
            catch(Exception e)
            {
                Debug.WriteLine(e);
            }
            return new List<Conversation>();
        }

        public void AppendToFile(List<Packet> messages, String username)
        {
            Histories.Add(new Conversation(messages, username));
            File.WriteAllText(path, System.Text.Json.JsonSerializer.Serialize(Histories));
        }

        public void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
