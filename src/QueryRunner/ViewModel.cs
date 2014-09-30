using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;
using QueryRunner.Annotations;

namespace QueryRunner
{
    public class ViewModel : INotifyPropertyChanged
    {
        private readonly MainWindow _mainWindow;

        public ViewModel(MainWindow mainWindow)
        {
            _mainWindow = mainWindow;
            _messages = new List<string>();
            RecalculateSqlTextHash(true);
        }

        private string _sqlText;
        private string _currentHash;
        private string _loadedHash;
        private DispatcherOperation _recalculateOperation;
        private ConnectionState _connectionState;
        private List<string> _messages;
        private string _currentFileName;
        private DataTable _results;
        public event PropertyChangedEventHandler PropertyChanged;

        public Visibility HasChangesVisibility
        {
            get { return HasChanges ? Visibility.Visible : Visibility.Hidden; }
        }

        public bool HasChanges
        {
            get { return _currentHash != _loadedHash; }
            set
            {
                if (!value)
                {
                    RecalculateSqlTextHash(true);
                }
                else
                {
                    _loadedHash = (_currentHash ?? "") + ".";
                }
                OnPropertyChanged();
                OnPropertyChanged("HasChangesVisibility");
            }
        }

        public string SqlText
        {
            get { return _sqlText; }
            set
            {
                if (value == _sqlText) return;
                _sqlText = value;
                OnPropertyChanged();
                OnPropertyChanged("CanExecuteQuery");
                if (_recalculateOperation != null) _recalculateOperation.Abort();
                _recalculateOperation = Dispatcher.CurrentDispatcher.InvokeAsync(() => RecalculateSqlTextHash(), DispatcherPriority.Background);
            }
        }

        public bool CanExecuteQuery
        {
            get
            {
                return !string.IsNullOrWhiteSpace(SqlText)
                       && (ConnectionState & ConnectionState.Open) != 0;
            }
        }

        public string WindowTitle
        {
            get
            {
                const string title = "ADO.NET Query Runner";
                return HasFileName ? Path.GetFileName(CurrentFileName) + " - " + title : title;
            }
        }

        public string CurrentFileName
        {
            get { return _currentFileName; }
            set
            {
                if (value == _currentFileName) return;
                _currentFileName = value;
                OnPropertyChanged();
                OnPropertyChanged("HasFileName");
                OnPropertyChanged("WindowTitle");
            }
        }

        public bool HasFileName { get { return !string.IsNullOrWhiteSpace(CurrentFileName); } }

        public ConnectionState ConnectionState
        {
            get { return _connectionState; }
            set
            {
                if (value == _connectionState) return;
                _connectionState = value;
                OnPropertyChanged();
                OnPropertyChanged("CanExecuteQuery");
                OnPropertyChanged("ConnectionStateOverlayImage");
                OnPropertyChanged("IsConnected");
                OnPropertyChanged("ConnectionStateString");
            }
        }

        public string ConnectionStateString
        {
            get { return ConnectionState.ToString(); }
        }

        public bool IsConnected
        {
            get
            {
                switch (ConnectionState)
                {
                    case ConnectionState.Open:
                    case ConnectionState.Fetching:
                    case ConnectionState.Executing:
                        return true;
                    default:
                        return false;
                }
            }
        }

        public ImageSource ConnectionStateOverlayImage
        {
            get
            {
                switch (ConnectionState)
                {
                    case ConnectionState.Open:
                    case ConnectionState.Fetching:
                    case ConnectionState.Executing:
                        return (ImageSource) _mainWindow.Resources["Connected"];
                    default:
                        return (ImageSource)_mainWindow.Resources["NotConnected"];
                }
            }
        }

        public IEnumerable<string> Messages
        {
            get { return _messages.AsReadOnly(); }
        }

        public void AddMessage(string message)
        {
            Debug.WriteLine(message);
            _messages.Add(message);
            OnPropertyChanged("Messages");
        }

        public void ClearMessages()
        {
            _messages = new List<string>();
            OnPropertyChanged("Messages");
        }

        private void RecalculateSqlTextHash(bool setLoadedHash = false)
        {
            var previousHasChanges = HasChanges;

            _currentHash = SqlText.GetMD5Hash();

            if (setLoadedHash)
                _loadedHash = _currentHash;

            if (HasChanges != previousHasChanges)
            {
                OnPropertyChanged("HasChanges");
                OnPropertyChanged("HasChangesVisibility");
            }
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            Debug.WriteLine("{0}.{1} changed.", new object[] { this.GetType().Name, propertyName });
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
