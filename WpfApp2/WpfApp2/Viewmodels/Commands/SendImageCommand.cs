using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace WpfApp2.Viewmodels.Commands
{
    public class SendImageCommand : ICommand
    {
        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public ChatViewmodel ViewModel { get; set; }

        public SendImageCommand(ChatViewmodel viewModel)
        {
            this.ViewModel = viewModel;
        }

        public bool CanExecute(object parameter)
        {
            return ViewModel.Connection.Client.Connected;
        }

        public void Execute(object parameter)
        {
            this.ViewModel.SendImage();
        }
    }
}
