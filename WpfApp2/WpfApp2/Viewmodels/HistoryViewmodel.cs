using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using WpfApp2.Models;
using WpfApp2.Viewmodels.Commands;

namespace WpfApp2.Viewmodels
{
    public class HistoryViewmodel: BaseViewmodel
    {
        public List<Packet> Convo { get; set; }
        public ICommand ExitWindowCommand { get; set; }

        public HistoryViewmodel(List<Packet> conversation)
        {
            Convo = conversation;
            ExitWindowCommand = new ExitWindowCommand(this);
        }
    }
}
