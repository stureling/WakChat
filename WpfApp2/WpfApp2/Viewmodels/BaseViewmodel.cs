using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using WpfApp2.Views;

namespace WpfApp2.Viewmodels
{
    public class BaseViewmodel
    {
        public Window window;

        public BaseViewmodel()
        {
        }

        public void ExitClick(Window window)
        {
            Debug.WriteLine("Closing Window");
            window.Close();
        }
        public void EnterClick(Window window)
        {
            Debug.WriteLine("Enter Client");
            ChatWindow newWindow = new ChatWindow();
            newWindow.Show();
            window.Close();
        }
    }
}
