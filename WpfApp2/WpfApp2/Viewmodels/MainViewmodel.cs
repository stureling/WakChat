using GalaSoft.MvvmLight.Messaging;
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
        private IMessenger _messengerInstance;
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
        protected IMessenger MessengerInstance
        {
            get
            {
                return _messengerInstance ?? Messenger.Default;
            }
            set
            {
                _messengerInstance = value;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public History _history;

        public ICommand ExitWindowCommand { get; set; }
        public ICommand OpenWindowCommand { get; set; }
        public ICommand OpenHistoryCommand { get; set; }

        public MainViewmodel(): base()
        {
            MessengerInstance.Register<NotificationMessage>(this, NotifyMe);
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
                queryConversations = from conv in ConversationList
                                     where conv.ID.Contains(Filter)
                                     select conv;
            }
            else
            {
                queryConversations = from conv in ConversationList
                                     select conv;
            }
            Conversations = new ObservableCollection<Conversation>(queryConversations);
        }

        public void OpenHistory(List<Packet> convo)
        {
            HistoryView history = new HistoryView(convo);
            history.Show();
        }
        public void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void NotifyMe(NotificationMessage notificationMessage)
        {
            string notification = notificationMessage.Notification;
            ConversationList.Clear();
            _history = new History();
            foreach (var item in _history.Histories)
            {
                ConversationList.Add(item);
            }
            FilterCollection();
        }
    }
}

