using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace WpfApp2.Viewmodels.Commands
{
    public class ExitWindowCommand : ICommand
    {
        public LoginViewmodel ViewModel { get; set; }

        public ExitWindowCommand(LoginViewmodel viewModel)
        {
            this.ViewModel = viewModel;
        }

        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            throw new NotImplementedException();
        }
        
        public void Execute(object parameter)
        {
            this.ViewModel.ExitClick();
        }
    }
}
