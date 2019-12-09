using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp2.Models
{
    static class ImageHelper
    {
        static public string SelectImage()
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
            ofd.Filter = "Image files (*.png;*.jpeg;*.jpg)|*.png;*.jpeg;*.jpg|All files (*.*)|*.*";
            if (ofd.ShowDialog() == true)
            {
                return ofd.FileName;
            }
            return null;
        }
        static public string EncodeImage(Image image)
        {

            Byte[] imageByte = null;
            using (var ms = new MemoryStream())
            {
                image.Save(ms, image.RawFormat);
                imageByte = ms.ToArray();
            }
            return Convert.ToBase64String(imageByte);
        }
        static public string SaveImage(ImagePacket packet)
        {
            Byte[] rawImage = new Byte[133769420];
            rawImage = Convert.FromBase64String(packet.StrImage);
            Image image = null;
            using (var ms = new MemoryStream(rawImage))
            {
                image = Image.FromStream(ms);
            }
            string path = AppDomain.CurrentDomain.BaseDirectory + @"image\"+ packet.Username + packet.Time.Millisecond.ToString() + "." + packet.format.ToString();
            image.Save(path, packet.format);
            return path;
        }
    }
}
