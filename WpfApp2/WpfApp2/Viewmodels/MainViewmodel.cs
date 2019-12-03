using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using WpfApp2.Viewmodels.Commands;

namespace WpfApp2.Viewmodels
{
    class MainViewmodel : BaseViewmodel
    {
        public ICommand ExitWindowCommand { get; set; }
        public ICommand OpenWindowCommand { get; set; }

        public MainViewmodel(): base()
        {
            this.ExitWindowCommand = new ExitWindowCommand(this);
            this.OpenWindowCommand = new OpenWindowCommand(this);
        }
        public override void ExitWindow()
        {
            CloseWindow();
        }
    }
}
