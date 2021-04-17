using System.Windows.Controls;
using NetworkToolbar.VM;

namespace NetworkToolbar.Views
{
    public partial class ToolbarView : UserControl
    {
        public ToolbarView()
        {
            DataContext = new NetworkStats();
            InitializeComponent();
        }
    }
}