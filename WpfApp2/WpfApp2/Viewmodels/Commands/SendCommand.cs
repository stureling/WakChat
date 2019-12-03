using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace WpfApp2.Viewmodels.Commands
{
    public class SendCommand : ICommand
    {
        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public ChatViewmodel ViewModel { get; set; }

        public SendCommand(ChatViewmodel viewModel)
        {
            this.ViewModel = viewModel;
        }

        public bool CanExecute(object parameter)

        {
            return !String.IsNullOrWhiteSpace(ViewModel.ThisMsg);
        }

        public void Execute(object parameter)
        {
            this.ViewModel.SendMessage();
        }
    }
}
