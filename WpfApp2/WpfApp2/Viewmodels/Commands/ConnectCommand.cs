using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace WpfApp2.Viewmodels.Commands
{
    class ConnectCommand : ICommand
    {
        public event EventHandler CanExecuteChanged;
        public LoginViewmodel ViewModel { get; set; }
        public ConnectCommand(LoginViewmodel viewModel)
        {
            this.ViewModel = viewModel;
        }
        public bool CanExecute(object parameter)
        {
            if (String.IsNullOrWhiteSpace(ViewModel.user.Username) || String.IsNullOrWhiteSpace(ViewModel.user.Port.ToString()))
            {
                Debug.WriteLine("Null or whitespace");
                return false;
            }
            if (ViewModel.user.Port < 1024 || ViewModel.user.Port > 65535)
            {
                Debug.WriteLine("Invalid Port");
                return false;
            }
            else if (ViewModel.user.Username.Length == 0 || ViewModel.user.Username.Length > 24)
            {
                Debug.WriteLine("Invalid Username");
                return false;
            }
            /*else if (!Regex.Match(ViewModel.user.IP, @"^(?:(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.){3}(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)$").Success)

            {
                return false;
            }*/
            else
            {
                Debug.WriteLine("Button should be clickable");
                return true;
            }
        }

        public void Execute(object parameter)
        {
            this.ViewModel.Connect();
        }
    }
}