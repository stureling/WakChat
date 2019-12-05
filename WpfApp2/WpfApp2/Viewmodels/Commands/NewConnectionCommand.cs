using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using WpfApp2.Models;

namespace WpfApp2.Viewmodels.Commands
{
    public class NewConnectionCommand : ICommand
    {
        public BaseViewmodel ViewModel { get; set; }
        public NewConnectionCommand(BaseViewmodel viewModel)
        {
            this.ViewModel = viewModel;
        }

        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            this.ViewModel.EnterClick();
        }
    }
}
