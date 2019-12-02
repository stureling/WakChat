using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using WpfApp2.Models;
using WpfApp2.Views;

namespace WpfApp2.Viewmodels
{
    public class BaseViewmodel
    {
        public BaseViewmodel()
        {
        }

        public void ExitClick(Window window)
        {
            Debug.WriteLine("Closing Window");
            window.Close();
        }
        public void EnterClick()
        {
            User user = new User();
            user.IP = "127.0.0.1";
            user.Username = "EEEEEEEEEEEEEEEEEEEEEEEE";
            user.Port = 4444;
            ChatView newWindow = new ChatView(new Connection(), user);
            newWindow.Show();
        }
    }
}
