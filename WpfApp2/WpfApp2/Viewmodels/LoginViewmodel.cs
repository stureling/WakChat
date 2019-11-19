using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using WpfApp2.Viewmodels.Commands;
using WpfApp2.Views;

namespace WpfApp2.Viewmodels
{
    public class LoginViewmodel : BaseViewmodel
    {
        int port;
        public ICommand ExitWindowCommand { get; set; }
        public ICommand EnterWindowCommand { get; set; }

        public LoginViewmodel(Window window) : base(window)
        {
            this.ExitWindowCommand = new ExitWindowCommand(this);
            this.EnterWindowCommand = new EnterWindowCommand(this);
        }

        public int PortNR
        {
            get
            {
                return port;
            }

            set
            {
                port = value;
            }
        }
    }
}
