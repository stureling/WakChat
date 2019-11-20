using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace WpfApp2.Viewmodels.Commands
{
    class ListenCommand : ICommand
    {
        public event EventHandler CanExecuteChanged;

        public LoginViewmodel ViewModel { get; set; }

        public ListenCommand(LoginViewmodel viewModel)
        {
            this.ViewModel = viewModel;
        }
        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            this.ViewModel.Listen();
        }
    }
}
