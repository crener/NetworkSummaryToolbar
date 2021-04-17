using System.Windows.Controls;
using NetworkToolbar.VM;

namespace NetworkToolbar.Views
{
    public partial class DeskBand : UserControl
    {
        public DeskBand()
        {
            DataContext = new NetworkStats();
            InitializeComponent();
        }
    }
}