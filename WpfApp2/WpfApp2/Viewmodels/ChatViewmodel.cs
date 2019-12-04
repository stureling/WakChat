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
   
        public string ThisMsg
        {
            get
            {
                return _user;
            }
            set
            {
                _user = value;
                OnPropertyChanged(ThisMsg);
            }
        }
        public ObservableCollection<Packet> Messages { get; set; }
        public ICommand ExitWindowCommand { get; set; }
        public ICommand OpenWindowCommand { get; set; }
        public ICommand SendCommand { get; set; }
        public ICommand SendImageCommand { get; set; }
        public event PropertyChangedEventHandler PropertyChanged;

        public ChatViewmodel(Connection connection, User user, Window window): base(window)
        {
            this.ExitWindowCommand = new ExitWindowCommand(this);
            this.OpenWindowCommand = new OpenWindowCommand(this);
            this.SendCommand = new SendCommand(this);
            this.SendImageCommand = new SendImageCommand(this);
            this.Connection = connection;
            this.User = user;
            this.ThisMsg = "";
            this.Messages = new ObservableCollection<Packet>();
            connection.Actions["Message"] = (Action<Packet>) DisplayMessage;
            connection.Actions["Image"] = (Action<Packet>) DisplayPicture;
            connection.startReciving();
        }

        public void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
        public void SendMessage()
        {
            Packet mess = new Packet() { ConnectionType = "Message", ConnectionTypeValue = ThisMsg, Username = User.Username, Time = DateTime.Now };
            Connection.Send(mess);
            Messages.Add(mess);
            ThisMsg = "";
        }
        public void SendPicture()
        {

            string imagestring = OpenPicture();
            Packet packet = new Packet() { ConnectionType = "Image", ConnectionTypeValue = imagestring};
            Connection.Send(packet);
           
        }
        public string OpenPicture()
        { 
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
            ofd.Filter = "Image files (*.png;*.jpeg)|*.png;*.jpeg|All files (*.*)|*.*";
            Image img = null;
            if (ofd.ShowDialog() == true)
            {
                img = Image.FromFile(ofd.FileName);
            }

            Byte[] imageByte = null;
            using (var ms = new MemoryStream())
            {
                img.Save(ms, img.RawFormat);
                imageByte = ms.ToArray();
            }
            string imagestring = Convert.ToBase64String(imageByte);
            return imagestring;
        }
        public void DisplayMessage(Packet messagee)
        {
            Messages.Add(messagee);
        }
        public void DisplayPicture(Packet packet)
        {
            MessageBox.Show("Got a picture for ya boss!", "Alert", MessageBoxButton.OK);

            string filePath = SaveImage(packet);
            packet.ConnectionTypeValue = filePath;
            Messages.Add(packet);
        }
        public string SaveImage(Packet packet)
        {
            Byte[] rawImage = new Byte[133769420];
            rawImage = Convert.FromBase64String(packet.ConnectionTypeValue);
            Image image = null;
            using (var ms = new MemoryStream(rawImage))
            {
                image = Image.FromStream(ms);
            }
            string path = AppDomain.CurrentDomain.BaseDirectory + @"\imagê\" + packet.ConnectionType + packet.Username + packet.Time.Date + ".png";
            image.Save(path, ImageFormat.Png);
            return path;
        }
        public override void ExitWindow()
        {
            Connection.Abort();
            CloseWindow();
        }
    }
}
