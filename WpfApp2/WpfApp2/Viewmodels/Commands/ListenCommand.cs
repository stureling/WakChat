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
    class ListenCommand : ICommand
    {
        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public LoginViewmodel ViewModel { get; set; }

        public ListenCommand(LoginViewmodel viewModel)
        {
            this.ViewModel = viewModel;
        }

        public bool CanExecute(object parameter)
         /*
         A lot of if-statements checking user input, split up to ease readability.
         Is bound to text boxes on login page.
         Username needs to be between 1 and 24 characters long.
         Port needs to be an integer with a value between 1024 ad 65535.
         */
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
            else if (ViewModel.User.Username.Length == 0 || ViewModel.User.Username.Length > 24 || !Regex.Match(ViewModel.User.Username, @"^[\p{L}\p{N}]+$").Success)
            {
                Debug.WriteLine("Invalid Username");
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
            this.ViewModel.Listen();
        }
    }
}
