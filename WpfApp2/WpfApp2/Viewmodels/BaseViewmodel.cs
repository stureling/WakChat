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
    public abstract class BaseViewmodel
    {
        private Window Window;
        public BaseViewmodel() { }
        public BaseViewmodel(Window window)
        {
            Window = window;
        }
        public abstract void ExitWindow();
        public void CloseWindow()
        {
            Window.Close();
        }
        public void EnterClick()
        {
            LoginView newWindow = new LoginView();
            newWindow.Show();
        }
    }
}
