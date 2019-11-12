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

        public BaseViewmodel(Window window)
        {
            this.window = window;
        }

        public void ExitClick()
        {
            Debug.WriteLine("Closing Window");
            this.window.Close();
        }
        public void EnterClick()
        {
            Debug.WriteLine("Enter Client");
            ChatWindow newWindow = new ChatWindow();
            newWindow.Show();
            this.window.Close();
        }
    }
}
