using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfApp2.Viewmodels.Commands;

namespace WpfApp2.Viewmodels
{
    public class LoginViewmodel
    {
        public ExitWindowCommand GetExitWindow { get; set; } 
        public LoginViewmodel()
        {
            this.GetExitWindow = new ExitWindowCommand(this);
        }

        public void ExitClick()
        {
            Debug.WriteLine("hello");
        }
    }
}
