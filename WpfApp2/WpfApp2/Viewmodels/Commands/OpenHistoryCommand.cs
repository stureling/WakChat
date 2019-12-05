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
    class OpenHistoryCommand : ICommand
    {
        public event EventHandler CanExecuteChanged;

        public MainViewmodel ViewModel { get; set; }

        public OpenHistoryCommand(MainViewmodel viewModel)
        {
            ViewModel = viewModel;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            List<Packet> convo = (List<Packet>)parameter;
            ViewModel.OpenHistory(convo);
        }
    }
}
