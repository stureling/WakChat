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
        public event PropertyChangedEventHandler PropertyChanged;

        public ChatViewmodel(Connection connection, User user, Window window): base(window)
        {
            ExitWindowCommand = new ExitWindowCommand(this);
            OpenWindowCommand = new OpenWindowCommand(this);
            SendCommand = new SendCommand(this);
            SendImageCommand = new SendImageCommand(this);
            Connection = connection;
            User = user;
            ThisMsg = "";
            Messages = new ObservableCollection<Packet>();
            connection.Actions["Message"] = (Action<Packet>) DisplayMessage;
            connection.Actions["Image"] = (Action<Packet>) DisplayPicture;
            connection.Actions["Buzz"] = (Action<Packet>) Buzz;
            connection.startReciving();
        }

        public override void ExitWindow()
        {
            List<Packet> lst = Messages.ToList();
            history.AppendToFile(lst, User.Username);
            Connection.Abort();
            base.ExitWindow();
        }

        public void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
        public void SendMessage()
        {
            Packet mess;
            if(ThisMsg.ToLower() == "buzz")
            {
                mess = new Packet() { ConnectionType = "Buzz", ConnectionTypeValue = ThisMsg, Username = User.Username, Time = DateTime.Now };
            }
            else
            {
                mess = new Packet() { ConnectionType = "Message", ConnectionTypeValue = ThisMsg, Username = User.Username, Time = DateTime.Now };
            }
            Connection.Send(mess);
            Messages.Add(mess);
            ThisMsg = "";
        }
        public void Buzz(Packet packet = null)
        {
            System.Media.SoundPlayer player = new System.Media.SoundPlayer(AppDomain.CurrentDomain.BaseDirectory + @"buzz\buzz.wav");
            player.Play();
        }
        public void SendImage()
        {

            string imagestring = OpenImage();
            Packet packet = new Packet() {Username = User.Username, Time = DateTime.Now, ConnectionType = "Image", ConnectionTypeValue = imagestring};
            Connection.Send(packet);
        }
        public string OpenImage()
        { 
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
            ofd.Filter = "Image files (*.png;*.jpeg;*.jpg)|*.png;*.jpeg;*.jpg|All files (*.*)|*.*";
            Image img = null;
            if (ofd.ShowDialog() == true)
            {
                img = Image.FromFile(ofd.FileName);
            }
            Messages.Add(new Packet() { Username = User.Username, Time = DateTime.Now, ConnectionType = "Image", ConnectionTypeValue = ofd.FileName});

            Byte[] imageByte = null;
            using (var ms = new MemoryStream())
            {
                img.Save(ms, img.RawFormat);
                imageByte = ms.ToArray();
            }
            string imagestring = Convert.ToBase64String(imageByte);
            return imagestring;
        }
        public void DisplayMessage(Packet packet)
        {
            Messages.Add(packet);
        }
        public void DisplayPicture(Packet packet)
        {
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
            string path = AppDomain.CurrentDomain.BaseDirectory + @"image\"+ "test" + packet.Username + packet.Time.Millisecond.ToString() + ".png";
            Debug.WriteLine(path);
            image.Save(path, ImageFormat.Png);
            return path;
        }
    }
}
