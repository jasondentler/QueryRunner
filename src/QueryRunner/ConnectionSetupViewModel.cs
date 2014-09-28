using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using QueryRunner.Annotations;

namespace QueryRunner
{
    public class ConnectionSetupViewModel : INotifyPropertyChanged
    {
        private string _invariantName;
        private DbProviderFactory _factory;
        private DbConnectionStringBuilder _builder;
        private ICustomTypeDescriptor TypeDescriptor { get { return _builder; } }

        private void LoadChangedFactory()
        {
            if (Providers.Any(pi => pi.InvariantName == InvariantName))
            {
                _factory = DbProviderFactories.GetFactory(InvariantName);
                _builder = _factory.CreateConnectionStringBuilder();
                OnPropertyChanged("ConnectionString");
                OnPropertyChanged("ConnectionStringEnabled");
                OnPropertyChanged("ProviderProperties");
            }
            else
            {
                _factory = null;
                _builder = null;
                OnPropertyChanged("ConnectionString");
                OnPropertyChanged("ConnectionStringEnabled");
                OnPropertyChanged("ProviderProperties");
            }
        }

        public string InvariantName
        {
            get { return _invariantName; }
            set
            {
                if (value == _invariantName) return;
                _invariantName = value;
                OnPropertyChanged();
                LoadChangedFactory();
            }
        }

        public string ConnectionString
        {
            get
            {
                return _builder == null ? string.Empty : _builder.ConnectionString;
            }
            set
            {
                if (_builder == null) return;
                if (value == _builder.ConnectionString) return;
                _builder.ConnectionString = value;
                OnPropertyChanged();
                OnPropertyChanged("ProviderProperties");
            }
        }

        public bool ConnectionStringEnabled
        {
            get { return _builder != null; }
        }


        public IEnumerable<IGrouping<string, ProviderProperty>> ProviderProperties
        {
            get
            {
                if (TypeDescriptor == null)  return Enumerable.Empty<IGrouping<string, ProviderProperty>>();
                var descriptors = TypeDescriptor.GetProperties().Cast<PropertyDescriptor>();
                var properties = descriptors.Select(pd => new ProviderProperty(pd, this)).ToArray();
                var ignoredData = new[]
                {
                    new {Category = "Data", Name = "ConnectionString"},
                    new {Category = "Misc", Name = "Count"},
                    new {Category = "Misc", Name = "BrowsableConnectionString"},
                    new {Category = "Misc", Name = "Keys"},
                    new {Category = "Misc", Name = "IsFixedSize"},
                    new {Category = "Misc", Name = "IsReadOnly"},
                    new {Category = "Misc", Name = "Values"}
                };

                var ignored = properties.Where(p => ignoredData.Contains(new {Category = p.Category, Name = p.DisplayName}));
                properties = properties.Except(ignored)
                    .OrderBy(p => p.Category)
                    .ThenBy(p => p.DisplayName)
                    .ToArray();
                return properties.GroupBy(pd => pd.Category);
            }
        }

        public class ProviderProperty : INotifyPropertyChanged
        {
            private readonly PropertyDescriptor _pd;
            private readonly ConnectionSetupViewModel _viewModel;

            public string Category { get; set; }
            public string DisplayName { get; set; }
            public string Description { get; set; }
            public string Name { get; set; }
            public Type PropertyType { get; set; }

            public object Value
            {
                get { return _pd.GetValue(_viewModel._builder); }
                set
                {
                    if (value == Value) return;
                    _pd.SetValue(_viewModel._builder, value);
                    OnPropertyChanged();
                    _viewModel.OnPropertyChanged("ConnectionString");
                }
            }

            public ProviderProperty(PropertyDescriptor pd, ConnectionSetupViewModel viewModel)
            {
                _pd = pd;
                _viewModel = viewModel;
                Category = pd.Category;
                DisplayName = pd.DisplayName;
                Description = pd.Description;
                Name = pd.Name;
                PropertyType = pd.PropertyType;
            }

            public event PropertyChangedEventHandler PropertyChanged;

            [NotifyPropertyChangedInvocator]
            protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
            {
                PropertyChangedEventHandler handler = PropertyChanged;
                if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public IEnumerable<ProviderInfo> Providers
        {
            get
            {
                return DbProviderFactories.GetFactoryClasses()
                    .Rows.Cast<DataRow>()
                    .Select(r => (ProviderInfo)r);
            }
        }

        public class ProviderInfo
        {
            public string Name { get; set; }
            public string Description { get; set; }
            public string InvariantName { get; set; }

            public static implicit operator ProviderInfo(DataRow row)
            {
                return new ProviderInfo()
                {
                    Name = row.Field<string>("Name"),
                    Description = row.Field<string>("Description"),
                    InvariantName = row.Field<string>("InvariantName")
                };
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            Debug.WriteLine("{0}.{1} changed.", new object[]{this.GetType().Name, propertyName});
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
