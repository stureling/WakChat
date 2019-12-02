using System.Windows;
using WpfApp2.Models;
using WpfApp2.Viewmodels;

namespace WpfApp2.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class ChatView : Window
    {
        public ChatView(Connection connection, User user)
        {
            InitializeComponent();
            DataContext = new ChatViewmodel(connection, user, this);
        }
    }
}
