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
    public class ChatWindowViewmodel : BaseViewmodel
    {
        public ICommand ExitWindowCommand { get; set; }
        public ICommand EnterWindowCommand { get; set; }

        public ChatWindowViewmodel()
        {
            this.ExitWindowCommand = new ExitWindowCommand(this);
            this.EnterWindowCommand = new EnterWindowCommand(this);
        }
    }
}
