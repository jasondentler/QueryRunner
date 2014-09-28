using System;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace QueryRunner
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        private readonly Controller _controller;

        public MainWindow()
        {
            _controller = new Controller(this);
            InitializeComponent();
            SqlText.Focus();
        }

        private void NewCommandBinding_OnExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            var cancel = false;
            _controller.New(ref cancel);
        }

        private void OpenCommandBinding_OnExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            var cancel = false;
            _controller.Open(ref cancel);
        }

        private void SaveCommandBinding_OnExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            var cancel = false;
            _controller.Save(ref cancel);
        }

        private void SaveAsCommandBinding_OnExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            var cancel = false;
            _controller.SaveAs(ref cancel);
        }

        private void PrintCommandBinding_OnExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            var cancel = false;
            _controller.Print(ref cancel);
        }

        private void CloseCommandBinding_OnCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            var dialogs = Application.Current.Windows.OfType<Window>().Where(w => w.IsModal());
            e.CanExecute = !dialogs.Any();
        }

        private void CloseCommandBinding_OnExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            _controller.Close();
        }

        private void UndoCommandBinding_OnCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = SqlText != null && SqlText.CanUndo;
        }

        private void UndoCommandBinding_OnExecuted(object sender, ExecutedRoutedEventArgs e)
        {
        }

        private void RedoCommandBinding_OnCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = SqlText != null && SqlText.CanRedo;
        }

        private void RedoCommandBinding_OnExecuted(object sender, ExecutedRoutedEventArgs e)
        {
        }

        private void CutCommandBinding_OnExecuted(object sender, ExecutedRoutedEventArgs e)
        {
        }

        private void CopyCommandBinding_OnExecuted(object sender, ExecutedRoutedEventArgs e)
        {
        }

        private void PasteCommandBinding_OnCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = Clipboard.ContainsText();
        }

        private void PasteCommandBinding_OnExecuted(object sender, ExecutedRoutedEventArgs e)
        {
        }

        private void SelectAllCommandBinding_OnExecuted(object sender, ExecutedRoutedEventArgs e)
        {
        }

        private void FindCommandBinding_OnExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void ReplaceCommandBinding_OnExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void DeleteCommandBinding_OnCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = SqlText != null && SqlText.SelectionLength > 0;
        }

        private void DeleteCommandBinding_OnExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            SqlText.SelectedText = string.Empty;
        }

        private void OpenConnectionCommandBinding_OnExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            var cancel = false;
            _controller.OpenConnection(ref cancel);
        }

        private void CloseConnectionCommandBinding_OnCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = _controller.ViewModel.IsConnected;
        }

        private void CloseConnectionCommandBinding_OnExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            _controller.CloseConnection();
        }

    }
}
