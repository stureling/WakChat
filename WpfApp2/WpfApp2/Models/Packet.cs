﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace WpfApp2.Models
{
    public abstract class Packet
    {
        public string Username { get; set; }
        public string ConnectionType { get; set; }
        public DateTime Time { get; set; }
        public Packet() { }
        public Packet(string username)
        {
            Username = username;
            Time = DateTime.Now;
        }
    }
    public class ConnectionPacket : Packet
    {
        public ConnectionPacket(string connectiontype, string username): base(username)
        {
            ConnectionType = connectiontype;
        }
    }
    public class ImagePacket: Packet
    {
        public ImagePacket() : base() { }
        public ImagePacket(Image image, string username): base(username)
        {
            ConnectionType = "Image";
            StrImage = ImageHelper.EncodeImage(image);
            format = image.RawFormat;
        }
        public string StrImage { get; set; }
        public ImageFormat format;
    }
    public class BuzzPacket : Packet
    {
        public BuzzPacket(string username) : base(username)
        {
            ConnectionType = "Buzz";
            Buzzer = "Sneeze";
        }
        public string Buzzer { get; set; }
    }
    public class MessagePacket : Packet
    {
        public MessagePacket(string _message, string username) : base(username)
        {
            ConnectionType = "Message";
            Message = _message;
        }
        public string Message { get; set; }
    }
    public class NullPacket : Packet
    {
        public NullPacket() : base("null")
        {
            ConnectionType = "Null";
        }
    }
}
