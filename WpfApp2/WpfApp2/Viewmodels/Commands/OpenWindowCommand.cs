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
    public class OpenWindowCommand : ICommand
    {
        public BaseViewmodel ViewModel { get; set; }
        public LoginViewmodel LoginModel { get; set; }

        public OpenWindowCommand(BaseViewmodel viewModel)
        {
            this.ViewModel = viewModel;
            this.LoginModel = LoginModel;
        }

        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            //NO WORKERINO
            //return String.IsNullOrWhiteSpace(LoginModel.user.Username);
            return true;
        }

        public void Execute(object parameter)
        {
            this.ViewModel.EnterClick();
        }
    }
}
