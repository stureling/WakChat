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

namespace WpfApp2.Viewmodels
{
    class MainViewmodel : BaseViewmodel, INotifyPropertyChanged
    {
        public ObservableCollection<Conversation> Conversations { get; set; }
        private List<Conversation> ConversationList { get; set; }
        private string _filter;
        public string Filter
        {
            get
            {
                return _filter;
            }
            set
            {
                _filter = value;
                FilterCollection();
            }
        }
        public event PropertyChangedEventHandler PropertyChanged;

        public History _history;

        public ICommand ExitWindowCommand { get; set; }
        public ICommand OpenWindowCommand { get; set; }

        public MainViewmodel(): base()
        {
            ExitWindowCommand = new ExitWindowCommand(this);
            OpenWindowCommand = new NewConnectionCommand(this);
            Conversations = new ObservableCollection<Conversation>();
            ConversationList = new List<Conversation>();
            _history = new History();
            foreach(var item in _history.Histories)
            {
                ConversationList.Add(item);
                Debug.WriteLine(item);
            }
        }
        private void FilterCollection()
        {
            var queryConversations = from conv in ConversationList
                                       where conv.ID.Contains(Filter)
                                       select conv;
            Conversations = new ObservableCollection<Conversation>(queryConversations);
        }
        public void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
