using GalaSoft.MvvmLight.Messaging;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using WpfApp2.Models;
using WpfApp2.Viewmodels.Commands;
using WpfApp2.Views;

namespace WpfApp2.Viewmodels
{
    public class ChatViewmodel : BaseViewmodel, INotifyPropertyChanged
    {
        public Connection Connection { get; set; }
        private string _user;

        public User User { get; set; }
        public History history = new History();
   
        public string ThisMsg
        {
            get
            {
                return _user;
            }
            set
            {
                _user = value;
                OnPropertyChanged("ThisMsg");
            }
        }
        public ObservableCollection<Packet> Messages { get; set; }
        public ICommand ExitWindowCommand { get; set; }
        public ICommand OpenWindowCommand { get; set; }
        public ICommand SendCommand { get; set; }
        public ICommand SendImageCommand { get; set; }

        private IMessenger _messengerInstance;

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

        public ChatViewmodel(Connection connection, User user, Window window): base(window)
        {
            ExitWindowCommand = new ExitWindowCommand(this);
            OpenWindowCommand = new NewConnectionCommand(this);
            SendCommand = new SendCommand(this);
            SendImageCommand = new SendImageCommand(this);
            Connection = connection;
            User = user;
            ThisMsg = "";
            Messages = new ObservableCollection<Packet>();
            connection.Actions["Message"] = (Action<Packet>) DisplayMessage;
            connection.Actions["Image"] = (Action<Packet>) DisplayImage;
            connection.Actions["Buzz"] = (Action<Packet>) RecieveBuzz;
            connection.startReciving();
        }

        public override void ExitWindow()
        {
            List<Packet> lst = Messages.ToList();
            history.AppendToFile(lst, User.Username);
            Connection.Abort();
            MessengerInstance.Send<NotificationMessage>(new NotificationMessage("notification message"));
            base.ExitWindow();
        }

        public void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
        public void SendMessage()
        {
            Packet mess = new MessagePacket(ThisMsg, User.Username);
            Connection.Send(mess);
            Messages.Add(mess);
            ThisMsg = "";
        }
        public void RecieveBuzz(Packet packet)
        {
            BuzzPacket buzzpkt = (BuzzPacket) packet;
            System.Media.SoundPlayer player = new System.Media.SoundPlayer(AppDomain.CurrentDomain.BaseDirectory + @"buzz\buzz.wav");
            player.Play();
            Messages.Add(packet);
        }
        public void SendBuzz()
        {
            Packet packet = new BuzzPacket(User.Username);
            Connection.Send(packet);
            Messages.Add(packet);
        }
        public void SendImage()
        {
            string path = ImageHelper.SelectImage();
            if(path != null)
            {
                Image img = Image.FromFile(path);
                ImagePacket packet = new ImagePacket(img, User.Username);
                Connection.Send(packet);
                packet.StrImage = path;
                Messages.Add(packet);
            }
        }
        public void DisplayMessage(Packet packet)
        {
            MessagePacket pkt = (MessagePacket)packet;
            Messages.Add(pkt);
        }
        public void DisplayImage(Packet packet)
        {
            ImagePacket imgpkt = (ImagePacket)packet;
            string filePath = ImageHelper.SaveImage(imgpkt);
            imgpkt.StrImage = filePath;
            Messages.Add(packet);
        }
    }
}
