using System.ComponentModel;
using System.Data;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Threading;
using DB2Query.Annotations;

namespace DB2Query
{
    public class ViewModel : INotifyPropertyChanged
    {

        public ViewModel()
        {
            RecalculateSqlTextHash(true);
        }

        private string _sqlText;
        private string _currentHash;
        private string _loadedHash;
        private DispatcherOperation _recalculateOperation;
        private ConnectionState _connectionState;
        private string _messages;
        private string _currentFileName;
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
                if (_recalculateOperation != null) _recalculateOperation.Abort();
                _recalculateOperation = Dispatcher.CurrentDispatcher.InvokeAsync(() => RecalculateSqlTextHash(), DispatcherPriority.Background);
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
            }
        }

        public string Messages
        {
            get { return _messages; }
            set
            {
                if (value == _messages) return;
                _messages = value;
                OnPropertyChanged();
            }
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
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
