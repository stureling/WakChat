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
            LoginView newWindow = new LoginView();
            newWindow.Show();
        }
    }
}
