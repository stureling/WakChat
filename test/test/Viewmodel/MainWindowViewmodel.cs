using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using test.Viewmodel.Commands;

namespace test.Viewmodel
{
    public class MainWindowViewmodel
    {
        public ICommand ExitCommand { get; set; }
        

        public MainWindowViewmodel()
        {
            this.ExitCommand = new ExitCommand(this);

        }

        public void ExitClick()
        {
            Debug.WriteLine("hello");
        }
    }
}
