﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace WpfApp2.Viewmodels.Commands
{
    public class EnterWindowCommand : ICommand
    {
        public BaseViewmodel ViewModel { get; set; }

        public EnterWindowCommand(BaseViewmodel viewModel)
        {
            this.ViewModel = viewModel;
        }

        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            return false;
        }

        public void Execute(object parameter)
        {
            this.ViewModel.EnterClick();
        }
    }
}