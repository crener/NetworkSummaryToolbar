using NetworkToolbar.VM;

namespace NetworkToolbar
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        public MainWindow()
        {
            DataContext = new NetworkStats();
            InitializeComponent();
        }
    }
}