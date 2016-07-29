using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using PoGoNecroBotTools.Model;
using PoGoNecroBotTools.Properties;
using Application = System.Windows.Application;
using MessageBox = System.Windows.MessageBox;
using OpenFileDialog = Microsoft.Win32.OpenFileDialog;

namespace PoGoNecroBotTools.ViewModel
{
    public class MainWindowViewModel : ViewModelBase
    {
        #region Constants

        private const string LocationsLoc = "locations.loc";

        #endregion

        #region Fields

        private readonly List<Process> _necroBotProcesses = new List<Process>();

        private RelayCommand _changeDefaultDirectoryCommand;
        private DirectoryInfo _dirInfo;
        private DumpFileParser _dumpFileParser;
        private RelayCommand _killNecroBotCommand;
        private ObservableCollection<Location> _locations = new ObservableCollection<Location>();
        private ReadOnlyObservableCollection<Location> _readOnlyLocations;
        private RelayCommand _removeLocationCommand;
        private Location _selectedLocation;
        private RelayCommand _setAsDefaultCommand;
        private RelayCommand _startNecroBotCommand;
        private RelayCommand _updateNecroBotCommand;

        #endregion

        #region Constructors

        public MainWindowViewModel()
        {
            Locations = new ReadOnlyObservableCollection<Location>(_locations);
        }

        #endregion

        #region Properties

        public RelayCommand AddLocationCommand => new RelayCommand(AddLocationAction);

        public RelayCommand ChangeDefaultDirectoryCommand
        {
            // ReSharper disable once ConvertPropertyToExpressionBody
            get { return _changeDefaultDirectoryCommand ?? (_changeDefaultDirectoryCommand = new RelayCommand(ChangeDefaultDirectoryAction, ChangeDefaultDirectoryAndUpdateNecroBotCanAction)); }
        }

        public RelayCommand KillNecroBotCommand
        {
            // ReSharper disable once ConvertPropertyToExpressionBody
            get { return _killNecroBotCommand ?? (_killNecroBotCommand = new RelayCommand(KillNecroBotAction, KillNecroBotCanAction)); }
        }

        public ReadOnlyObservableCollection<Location> Locations
        {
            get { return _readOnlyLocations; }
            private set
            {
                _readOnlyLocations = value;
                RaisePropertyChanged(() => Locations);
            }
        }

        public ReadOnlyObservableCollection<PokemonGoAccount> PokemonGoAccounts => _dumpFileParser?.PokemonGoAccounts;

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
            get { return _startNecroBotCommand ?? (_startNecroBotCommand = new RelayCommand(StartNecroBotAction, StartNecroBotCanAction)); }
        }

        public RelayCommand UpdateNecroBotCommand
        {
            // ReSharper disable once ConvertPropertyToExpressionBody
            get { return _updateNecroBotCommand ?? (_updateNecroBotCommand = new RelayCommand(UpdateNecroBotAction, ChangeDefaultDirectoryAndUpdateNecroBotCanAction)); }
        }

        #endregion

        #region Methods

        private static bool ChangeDefaultDirectory()
        {
            var fbd = new FolderBrowserDialog { Description = Resources.MainWindow_Select_your_NecroBot_folder };
            var result = fbd.ShowDialog();

            if (result != DialogResult.OK || string.IsNullOrWhiteSpace(fbd.SelectedPath)) return false;

            Settings.Default.DefaultDirectory = fbd.SelectedPath;
            Settings.Default.Save();

            return true;
        }

        public void OnContentRendered()
        {
            if (!Directory.Exists(Settings.Default.DefaultDirectory))
            {
                if (ChangeDefaultDirectory()) LoadDefaultDirectory();
                else Application.Current.Shutdown();
            }
            else LoadDefaultDirectory();
        }

        private void AddLocationAction()
        {
            var addLocationDialog = new AddLocationDialogViewModel();
            var result = addLocationDialog.ShowDialog();

            if (!result.HasValue || !result.Value) return;

            _locations.Add(new Location(addLocationDialog.LocationTitle, addLocationDialog.DoubleLocationLatitude, addLocationDialog.DoubleLocationLongitude));
            Serialize();
        }

        private void ChangeDefaultDirectoryAction()
        {
            if (ChangeDefaultDirectory()) LoadDefaultDirectory();
        }

        private bool ChangeDefaultDirectoryAndUpdateNecroBotCanAction()
        {
            return _necroBotProcesses.Count == 0;
        }

        private void KillNecroBotAction()
        {
            foreach (var process in _necroBotProcesses.ToArray())
            {
                process.Kill();
            }

            _necroBotProcesses.Clear();

            RaiseCommandsCanExecuteChanged();
        }

        private bool KillNecroBotCanAction()
        {
            return _necroBotProcesses.Count > 0;
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

            _dumpFileParser?.Dispose();
            _dumpFileParser = new DumpFileParser(_dirInfo);

            RaisePropertyChanged(() => PokemonGoAccounts);
        }

        private void RaiseCommandsCanExecuteChanged()
        {
            ChangeDefaultDirectoryCommand.RaiseCanExecuteChanged();
            UpdateNecroBotCommand.RaiseCanExecuteChanged();
            KillNecroBotCommand.RaiseCanExecuteChanged();
            StartNecroBotCommand.RaiseCanExecuteChanged();
        }

        private void RemoveLocationAction()
        {
            if (SelectedLocation != null) _locations.Remove(SelectedLocation);
            Serialize();

            StartNecroBotCommand.RaiseCanExecuteChanged();
        }

        private bool RemoveLocationCanAction()
        {
            return SelectedLocation != null;
        }

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

        private bool SetAsDefaultCanAction()
        {
            return SelectedLocation != null;
        }

        private void StartNecroBotAction()
        {
            Serialize();

            var necroBotDirectories = _dirInfo.EnumerateDirectories().Where(x => x.EnumerateFiles().Any(y => y.Name == Settings.Default.NecroBotExeName)).ToArray();
            if (necroBotDirectories.Length == 0) MessageBox.Show(Resources.MainWindowViewModel_Any_NecroBot_directory_was_detected, Resources.MainWindowViewModel_UpdateNecroBotAction_Error, MessageBoxButton.OK, MessageBoxImage.Error);

            foreach (var necroBotDirectory in necroBotDirectories)
            {
                var configDir =
                    necroBotDirectory.EnumerateDirectories().FirstOrDefault(x => string.Equals(x.Name, Settings.Default.NecroBotConfigDirectoryName, StringComparison.CurrentCultureIgnoreCase));
                var configFileInfo = configDir?.EnumerateFiles().FirstOrDefault(x => x.Name == Settings.Default.NecroBotConfigFileName);
                if (configFileInfo != null)
                {
                    var configFile = new ConfigFile(configFileInfo.FullName);

                    var defaultLocation = Locations.Single(x => x.IsDefault);
                    configFile.UpdateDefaultLatitudeLongitude(defaultLocation.Latitude, defaultLocation.Longitude);
                }

                var necroBotExe = necroBotDirectory.EnumerateFiles().Single(x => x.Name == Settings.Default.NecroBotExeName);

                var processStartInfo = new ProcessStartInfo(necroBotExe.FullName) { WorkingDirectory = necroBotDirectory.FullName };
                var processStart = Process.Start(processStartInfo);

                Task.Factory.StartNew(() =>
                {
                    processStart?.WaitForExit();
                    if (processStart == null || !_necroBotProcesses.Contains(processStart)) return;

                    _necroBotProcesses.Remove(processStart);

                    Application.Current.Dispatcher.BeginInvoke(new Action(RaiseCommandsCanExecuteChanged));
                });

                _necroBotProcesses.Add(processStart);
            }

            RaiseCommandsCanExecuteChanged();
        }

        private bool StartNecroBotCanAction()
        {
            return _necroBotProcesses.Count == 0 && Locations.Any(x => x.IsDefault);
        }

        private void UpdateNecroBotAction()
        {
            var dlg = new OpenFileDialog { InitialDirectory = _dirInfo.FullName, Filter = Settings.Default.NecroBotReleaseFileExtension, Multiselect = false };
            var result = dlg.ShowDialog();

            if (result != true || !File.Exists(dlg.FileName)) return;

            var releaseFileInfo = new FileInfo(dlg.FileName);

            #region Extracting Zip file

            var releaseZip = ZipFile.OpenRead(releaseFileInfo.FullName);
            if (releaseZip.Entries.Count == 0) return;

            var updateDirectoryInfo = new DirectoryInfo(Path.Combine(_dirInfo.FullName, "Update"));
            if (updateDirectoryInfo.Exists) updateDirectoryInfo.Delete(true);
            updateDirectoryInfo.Create();
            releaseZip.ExtractToDirectory(updateDirectoryInfo.FullName);

            var realUpdate = updateDirectoryInfo;
            while (!realUpdate.EnumerateFiles().Any())
            {
                realUpdate = realUpdate.EnumerateDirectories().First();
            }

            #endregion Extracting Zip file

            var necroBotDirectories = _dirInfo.EnumerateDirectories().Where(x => x.EnumerateFiles().Any(y => y.Name == Settings.Default.NecroBotExeName)).ToArray();
            if (necroBotDirectories.Length == 0)
            {
                MessageBox.Show(Resources.MainWindowViewModel_Any_NecroBot_directory_was_detected, Resources.MainWindowViewModel_UpdateNecroBotAction_Error, MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            #region Deleting old files

            foreach (var necroBotDirectory in necroBotDirectories)
            {
                var configDir =
                    necroBotDirectory.EnumerateDirectories().FirstOrDefault(x => string.Equals(x.Name, Settings.Default.NecroBotConfigDirectoryName, StringComparison.CurrentCultureIgnoreCase));
                var logsDir = necroBotDirectory.EnumerateDirectories().FirstOrDefault(x => string.Equals(x.Name, Settings.Default.NecroBotLogsDirectoryName, StringComparison.CurrentCultureIgnoreCase));

                if (configDir != null) // Saving auth.json and config.json
                {
                    var authFileInfo = configDir.EnumerateFiles().FirstOrDefault(x => x.Name == Settings.Default.NecroBotAuthFileName);
                    var configFileInfo = configDir.EnumerateFiles().FirstOrDefault(x => x.Name == Settings.Default.NecroBotConfigFileName);

                    foreach (var directoryInfo in configDir.EnumerateDirectories())
                    {
                        directoryInfo.Delete(true);
                    }
                    foreach (var fileInfo in configDir.EnumerateFiles())
                    {
                        if (fileInfo.FullName != authFileInfo?.FullName && fileInfo.FullName != configFileInfo?.FullName) fileInfo.Delete();
                    }
                }

                foreach (var directoryInfo in necroBotDirectory.EnumerateDirectories())
                {
                    if (directoryInfo.FullName != configDir?.FullName && directoryInfo.FullName != logsDir?.FullName) directoryInfo.Delete(true);
                }
                foreach (var fileInfo in necroBotDirectory.EnumerateFiles())
                {
                    fileInfo.Delete();
                }
            }

            #endregion Deleting old files

            foreach (var necroBotDirectory in necroBotDirectories)
            {
                realUpdate.CopyTo(necroBotDirectory);
            }

            if (Directory.Exists(updateDirectoryInfo.FullName)) updateDirectoryInfo.Delete(true);
        }

        #endregion
    }
}