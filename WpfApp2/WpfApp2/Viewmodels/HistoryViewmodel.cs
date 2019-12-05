using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfApp2.Models;

namespace WpfApp2.Viewmodels
{
    public class HistoryViewmodel: BaseViewmodel
    {
        public Conversation Convo { get; set; }

        public HistoryViewmodel(Conversation conversation)
        {
            Convo = conversation;
        }
    }
}
