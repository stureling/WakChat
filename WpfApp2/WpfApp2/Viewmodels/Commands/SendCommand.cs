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
            Debug.WriteLine("CONSTRUCTOR");
            this.ViewModel = viewModel;
        }

        public bool CanExecute(object parameter)

        {
            Debug.WriteLine("CANEXECUTE");
            return !String.IsNullOrWhiteSpace(ViewModel.ThisMsg);
        }

        public void Execute(object parameter)
        {
            Debug.WriteLine("EXECUTE");
            this.ViewModel.SendMessage();
        }
    }
}
