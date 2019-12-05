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
        private ObservableCollection<Conversation> _conversations;
        public ObservableCollection<Conversation> Conversations
        {
            get
            {
                return _conversations;
            }
            set
            {
                _conversations = value;
                OnPropertyChanged("Conversations");
            }
        }
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
                OnPropertyChanged("Filter");
                FilterCollection();
            }
        }
        public event PropertyChangedEventHandler PropertyChanged;

        public History _history;

        public ICommand ExitWindowCommand { get; set; }
        public ICommand OpenWindowCommand { get; set; }
        public ICommand OpenHistoryCommand { get; set; }

        public MainViewmodel(): base()
        {
            ExitWindowCommand = new ExitWindowCommand(this);
            OpenWindowCommand = new NewConnectionCommand(this);
            OpenWindowCommand = new OpenWindowCommand(this);
            OpenHistoryCommand = new OpenHistoryCommand(this);
            Conversations = new ObservableCollection<Conversation>();
            ConversationList = new List<Conversation>();
            _history = new History();
            foreach(var item in _history.Histories)
            {
                ConversationList.Add(item);
            }
            Filter = "";
        }
        private void FilterCollection()
        {
            IEnumerable<Conversation> queryConversations;
            if(Filter != String.Empty)
            {
                Debug.WriteLine("Saddle up");
                queryConversations = from conv in ConversationList
                                     where conv.ID.Contains(Filter)
                                     select conv;
            }
            else
            {
                Debug.WriteLine("Saddle down");
                Debug.WriteLine(ConversationList.Count());
                queryConversations = from conv in ConversationList
                                     select conv;
            }
            Debug.WriteLine(queryConversations.Count());
            Conversations = new ObservableCollection<Conversation>(queryConversations);
        }

        public void OpenHistory(Conversation convo)
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
