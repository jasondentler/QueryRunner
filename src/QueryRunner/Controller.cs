using System;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using QueryRunner.Annotations;

namespace QueryRunner
{
    public class Controller
    {
        private readonly ViewModel _viewModel;
        private readonly MainWindow _mainWindow;

        public Controller([NotNull] MainWindow mainWindow)
        {
            if (mainWindow == null) throw new ArgumentNullException("mainWindow");
            _mainWindow = mainWindow;
            _viewModel = new ViewModel(mainWindow);
            _mainWindow.DataContext = _viewModel;
        }

        public void Close()
        {
            var cancel = false;
            if (_viewModel.HasChanges)
                PromptToHandleUnsavedChanges(ref cancel);
            if (!cancel)
                Application.Current.Shutdown();
        }

        public void Save(ref bool cancel)
        {
            cancel = false;
            if (string.IsNullOrWhiteSpace(_viewModel.CurrentFileName))
            {
                SaveAs(ref cancel);
                return;
            }
            try
            {
                File.WriteAllText(_viewModel.CurrentFileName, _viewModel.SqlText);
                _viewModel.HasChanges = false;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
                MessageBox.Show(_mainWindow, ex.Message, "Error saving file", MessageBoxButton.OK, MessageBoxImage.Error);
                cancel = true;
            }
        }

        public void SaveAs(ref bool cancel)
        {
            var dlg = new Microsoft.Win32.SaveFileDialog();
            if (!string.IsNullOrWhiteSpace(_viewModel.CurrentFileName))
            {
                dlg.InitialDirectory = Path.GetDirectoryName(_viewModel.CurrentFileName);
                dlg.FileName = Path.GetFileName(_viewModel.CurrentFileName);
            }
            dlg.DefaultExt = ".sql"; // Default file extension
            dlg.Filter = "SQL scripts (.sql)|*.sql"; // Filter files by extension

            // Show save file dialog box
            cancel = !(dlg.ShowDialog(_mainWindow) ?? false);

            // Process save file dialog box results
            if (!cancel)
            {
                // Save document
                _viewModel.CurrentFileName = dlg.FileName;
                Save(ref cancel);
            }
        }

        public void Print(ref bool cancel)
        {
            var doc = new FlowDocument(new Paragraph(new Run(_viewModel.SqlText)));
            
            var paginator = ((IDocumentPaginatorSource) doc).DocumentPaginator;

            var dlg = new PrintDialog();
            dlg.MinPage = 1;
            dlg.MaxPage = Convert.ToUInt32(paginator.PageCount);
            dlg.PageRangeSelection = PageRangeSelection.AllPages;
            dlg.CurrentPageEnabled = false;
            dlg.SelectedPagesEnabled = false;
            dlg.UserPageRangeEnabled = true;
            
            cancel = !(dlg.ShowDialog() ?? false);

            if (!cancel)
                dlg.PrintDocument(paginator, _viewModel.HasFileName ? Path.GetFileName(_viewModel.CurrentFileName) : "SQL Query");
        }

        private void PromptToHandleUnsavedChanges(ref bool cancel)
        {
            var result = MessageBox.Show(_mainWindow, "Do you want to save your SQL statements?", "Unsaved changes",
                MessageBoxButton.YesNoCancel, MessageBoxImage.Warning);
            cancel = result == MessageBoxResult.Cancel;
            if (result == MessageBoxResult.Yes)
                Save(ref cancel);
        }

        public void New(ref bool cancel)
        {
            cancel = false;
            if (_viewModel.HasChanges)
                PromptToHandleUnsavedChanges(ref cancel);
            if (cancel) return;
            _viewModel.SqlText = string.Empty;
            _viewModel.CurrentFileName = null;
            _viewModel.HasChanges = false;
        }

        public void Open(ref bool cancel)
        {
            cancel = false;
            if (_viewModel.HasChanges)
                PromptToHandleUnsavedChanges(ref cancel);
            if (cancel) return;

            var dlg = new Microsoft.Win32.OpenFileDialog();
            if (!string.IsNullOrWhiteSpace(_viewModel.CurrentFileName))
            {
                dlg.InitialDirectory = Path.GetDirectoryName(_viewModel.CurrentFileName);
                dlg.FileName = Path.GetFileName(_viewModel.CurrentFileName);
            }
            dlg.DefaultExt = ".sql"; // Default file extension
            dlg.Filter = "SQL scripts (.sql)|*.sql"; // Filter files by extension

            cancel = !(dlg.ShowDialog(_mainWindow) ?? false);
            if (cancel) return;

            _viewModel.SqlText = File.ReadAllText(dlg.FileName);
            _viewModel.CurrentFileName = dlg.FileName;
            _viewModel.HasChanges = false;
        }

        public void OpenConnection()
        {
            _viewModel.ConnectionState = _viewModel.ConnectionState == ConnectionState.Open
                ? ConnectionState.Closed
                : ConnectionState.Open;
        }
    }
}
