using System.Windows;

namespace QueryRunner
{
    /// <summary>
    /// Interaction logic for ConnectionSetup.xaml
    /// </summary>
    public partial class ConnectionSetup : Window
    {

        private readonly ConnectionSetupViewModel _viewModel;

        public ConnectionSetup()
        {
            _viewModel = new ConnectionSetupViewModel();
            DataContext = _viewModel;
            InitializeComponent();
        }

        private void OpenConnection_OnClick(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
            this.Close();
        }
    }
}
