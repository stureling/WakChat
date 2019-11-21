using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Input;

namespace WpfApp2.Viewmodels.Commands
{
    class ConnectCommand : ICommand
    {
        public LoginViewmodel ViewModel { get; set; }
        public ConnectCommand(LoginViewmodel viewModel)
        {
            this.ViewModel = viewModel;
        }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public bool CanExecute(object parameter)
        {
            if (String.IsNullOrWhiteSpace(ViewModel.User.Username) || String.IsNullOrWhiteSpace(ViewModel.User.Port.ToString()))
            {
                Debug.WriteLine("Null or whitespace");
                return false;
            }
            if (ViewModel.User.Port < 1024 || ViewModel.User.Port > 65535)
            {
                Debug.WriteLine("Invalid Port");
                return false;
            }
            else if (ViewModel.User.Username.Length == 0 || ViewModel.User.Username.Length > 24)
            {
                Debug.WriteLine("Invalid Username");
                return false;
            }
            else if (!Regex.Match(ViewModel.User.IP, @"^(?:(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.){3}(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)$").Success)

            {
                return false;
            }
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