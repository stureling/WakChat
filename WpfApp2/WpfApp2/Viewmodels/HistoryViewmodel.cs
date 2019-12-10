using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using WpfApp2.Models;
using WpfApp2.Viewmodels.Commands;

namespace WpfApp2.Viewmodels
{
    public class HistoryViewmodel: BaseViewmodel
    {
        public List<Packet> Convo { get; set; }
        public ObservableCollection<Packet> Coll { get; set; }
        public ICommand ExitWindowCommand { get; set; }

        public HistoryViewmodel(List<Packet> conversation, Window window) : base(window)
        {
            Convo = conversation;
            Coll = new ObservableCollection<Packet>();
            foreach(var item in Convo)
            {
                Debug.WriteLine(item);
                Coll.Add(item);
            }
            ExitWindowCommand = new ExitWindowCommand(this);
        }
    }
}
