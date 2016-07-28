using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Windows.Forms;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using PoGoNecroBotTools.Model;
using PoGoNecroBotTools.Properties;
using Application = System.Windows.Application;

namespace PoGoNecroBotTools.ViewModel
{
    public class MainWindowViewModel : ViewModelBase
    {
        #region Constants

        private const string LocationsLoc = "locations.loc";

        #endregion

        #region Fields

        private DirectoryInfo _dirInfo;
        private ObservableCollection<Location> _locations = new ObservableCollection<Location>();
        private ReadOnlyObservableCollection<Location> _readOnlyLocations;
        private RelayCommand _removeLocationCommand;
        private Location _selectedLocation;
        private RelayCommand _setAsDefaultCommand;
        private RelayCommand _startNecroBotAction;

        #endregion

        #region Constructors

        public MainWindowViewModel()
        {
            if (!Directory.Exists(Settings.Default.DefaultDirectory))
            {
                var fbd = new FolderBrowserDialog { Description = Resources.MainWindow_Select_your_NecroBot_folder };
                var result = fbd.ShowDialog();

                if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(fbd.SelectedPath))
                {
                    Settings.Default.DefaultDirectory = fbd.SelectedPath;
                    Settings.Default.Save();

                    LoadDefaultDirectory();
                }
                else Application.Current.Shutdown();
            }
            else LoadDefaultDirectory();
        }

        #endregion

        #region Properties

        public RelayCommand AddLocationCommand => new RelayCommand(AddLocationAction);

        public ReadOnlyObservableCollection<Location> Locations
        {
            get { return _readOnlyLocations; }
            private set
            {
                _readOnlyLocations = value;
                RaisePropertyChanged(() => Locations);
            }
        }

        public RelayCommand RemoveLocationCommand
        {
            // ReSharper disable once ConvertPropertyToExpressionBody
            get { return _removeLocationCommand ?? (_removeLocationCommand = new RelayCommand(RemoveLocationAction, RemoveLocationCanAction)); }
        }

        public Location SelectedLocation
        {
            get { return _selectedLocation; }
            set
            {
                _selectedLocation = value;
                SetAsDefaultCommand.RaiseCanExecuteChanged();
                RemoveLocationCommand.RaiseCanExecuteChanged();
            }
        }

        public RelayCommand SetAsDefaultCommand
        {
            // ReSharper disable once ConvertPropertyToExpressionBody
            get { return _setAsDefaultCommand ?? (_setAsDefaultCommand = new RelayCommand(SetAsDefaultAction, SetAsDefaultCanAction)); }
        }

        public RelayCommand StartNecroBotCommand
        {
            // ReSharper disable once ConvertPropertyToExpressionBody
            get { return _startNecroBotAction ?? (_startNecroBotAction = new RelayCommand(StartNecroBotAction, StartNecroBotCanAction)); }
        }

        #endregion

        #region Methods

        private void AddLocationAction()
        {
            var addLocationDialog = new AddLocationDialogViewModel();
            var result = addLocationDialog.ShowDialog();

            if (!result.HasValue || !result.Value) return;

            _locations.Add(new Location(addLocationDialog.LocationTitle, addLocationDialog.DoubleLocationLatitude, addLocationDialog.DoubleLocationLongitude));
            Serialize();
        }

        private void LoadDefaultDirectory()
        {
            _dirInfo = new DirectoryInfo(Settings.Default.DefaultDirectory);
            var locationFile = _dirInfo.EnumerateFiles().FirstOrDefault(x => x.Name == LocationsLoc);
            if (locationFile != null)
            {
                var formatter = new BinaryFormatter();
                try
                {
                    using (var stream = locationFile.OpenRead())
                    {
                        _locations = (ObservableCollection<Location>)formatter.Deserialize(stream);
                    }
                }
                catch (Exception)
                {
                    locationFile.Delete();
                }
            }
            else _locations = new ObservableCollection<Location>();

            Locations = new ReadOnlyObservableCollection<Location>(_locations);

            StartNecroBotCommand.RaiseCanExecuteChanged();
        }

        private void RemoveLocationAction()
        {
            if (SelectedLocation != null) _locations.Remove(SelectedLocation);
            Serialize();

            StartNecroBotCommand.RaiseCanExecuteChanged();
        }

        private bool RemoveLocationCanAction() { return SelectedLocation != null; }

        private void Serialize()
        {
            using (var stream = File.Create(Path.Combine(_dirInfo.FullName, LocationsLoc)))
            {
                var formatter = new BinaryFormatter();
                formatter.Serialize(stream, _locations);
            }
        }

        private void SetAsDefaultAction()
        {
            if (SelectedLocation != null)
            {
                foreach (var location in Locations.Where(x => x.IsDefault))
                {
                    location.IsDefault = false;
                }
                SelectedLocation.IsDefault = true;
            }
            Serialize();

            StartNecroBotCommand.RaiseCanExecuteChanged();
        }

        private bool SetAsDefaultCanAction() { return SelectedLocation != null; }

        private void StartNecroBotAction()
        {
            Serialize();

            var necroBotDirectories = _dirInfo.EnumerateDirectories().Where(x => x.EnumerateFiles().Any(y => y.Name == Settings.Default.NecroBotExeName)).ToArray();

            foreach (var necroBotDirectory in necroBotDirectories)
            {
                var configDir = necroBotDirectory.EnumerateDirectories().FirstOrDefault(x => x.Name == Settings.Default.NecroBotConfigDirectoryName);
                var configFileInfo = configDir?.EnumerateFiles().FirstOrDefault(x => x.Name == Settings.Default.NecroBotConfigFileName);
                if (configFileInfo != null)
                {
                    var configFile = new ConfigFile(configFileInfo.FullName);

                    var defaultLocation = Locations.Single(x => x.IsDefault);
                    configFile.UpdateDefaultLatitudeLongitude(defaultLocation.Latitude, defaultLocation.Longitude);
                }

                var necroBotExe = necroBotDirectory.EnumerateFiles().Single(x => x.Name == Settings.Default.NecroBotExeName);

                var processStartInfo = new ProcessStartInfo(necroBotExe.FullName) { WorkingDirectory = necroBotDirectory.FullName };
                Process.Start(processStartInfo);
            }
        }

        private bool StartNecroBotCanAction() { return Locations.Any(x => x.IsDefault); }

        #endregion
    }
}