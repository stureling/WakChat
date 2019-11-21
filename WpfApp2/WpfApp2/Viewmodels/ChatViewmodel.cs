using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using WpfApp2.Viewmodels.Commands;
using WpfApp2.Views;

namespace WpfApp2.Viewmodels
{
    public class ChatViewmodel : BaseViewmodel
    {
        public ICommand ExitWindowCommand { get; set; }
        public ICommand OpenWindowCommand { get; set; }

        public ChatViewmodel()
        {
            this.ExitWindowCommand = new ExitWindowCommand(this);
            this.OpenWindowCommand = new OpenWindowCommand(this);
        }
    }
}
