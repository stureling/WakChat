using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using WpfApp2.Viewmodels.Commands;

namespace WpfApp2.Viewmodels
{
    public class LoginViewmodel
    {
        public ICommand ExitWindowCommand { get; set; } 

        public ICommand Test { get; set; }

        public LoginViewmodel()
        {
            this.ExitWindowCommand = new ExitWindowCommand(this);

            Test = new TestCommand(p => Debug.WriteLine("test"), p => true);
        }

        public void ExitClick()
        {
            Debug.WriteLine("hello");
        }
    }
}
