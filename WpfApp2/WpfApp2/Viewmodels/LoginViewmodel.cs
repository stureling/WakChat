using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using WpfApp2.Viewmodels.Commands;

namespace WpfApp2.Viewmodels
{
    public class LoginViewmodel
    {
        public ICommand ExitWindowCommand { get; set; }
        public ICommand EnterWindowCommand { get; set; }

        public Window window;

        public LoginViewmodel(Window window)
        {
            this.ExitWindowCommand = new ExitWindowCommand(this);
            this.EnterWindowCommand = new EnterWindowCommand(this);
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
            MainWindow newWindow = new MainWindow();
            newWindow.Show();
            this.window.Close();
        }
    }
}
