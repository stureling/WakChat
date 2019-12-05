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
    class MainViewmodel : BaseViewmodel, INotifyPropertyChanged
    {
        public ObservableCollection<Conversation> Conversations { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        public History _history;

        public ICommand ExitWindowCommand { get; set; }
        public ICommand OpenWindowCommand { get; set; }
        public ICommand OpenHistoryCommand { get; set; }

        public MainViewmodel(): base()
        {
            ExitWindowCommand = new ExitWindowCommand(this);
            OpenWindowCommand = new OpenWindowCommand(this);
            OpenHistoryCommand = new OpenHistoryCommand(this);
            Conversations = new ObservableCollection<Conversation>();
            _history = new History();
            foreach(var item in _history.Histories)
            {
                Conversations.Add(item);
            }
        }

        public void OpenHistory(List<Packet> convo)
        {
            HistoryView history = new HistoryView(new HistoryViewmodel(convo));
            history.Show();
        }
        public void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
