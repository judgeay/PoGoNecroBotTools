using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Timers;
using System.Windows;
using PoGoNecroBotTools.Model;
using PoGoNecroBotTools.Properties;

namespace PoGoNecroBotTools
{
    internal class DumpFileParser : IDisposable
    {
        #region Fields

        private readonly DirectoryInfo _directoryInfo;
        private readonly Dictionary<string, DateTime> _lastModifiedDictionary = new Dictionary<string, DateTime>();
        private readonly ObservableCollection<PokemonGoAccount> _pokemonGoAccounts = new ObservableCollection<PokemonGoAccount>();
        private readonly Timer _timer;

        #endregion

        #region Constructors

        public DumpFileParser(DirectoryInfo directoryInfo)
        {
            _directoryInfo = directoryInfo;

            _timer = new Timer(1000);
            _timer.Elapsed += HandleTimer;
            _timer.Start();

            PokemonGoAccounts = new ReadOnlyObservableCollection<PokemonGoAccount>(_pokemonGoAccounts);
        }

        #endregion

        #region Properties

        public ReadOnlyObservableCollection<PokemonGoAccount> PokemonGoAccounts { get; }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            _timer.Stop();
            _timer.Close();
            _timer.Dispose();
        }

        #endregion

        #region Methods

        private void HandleTimer(object sender, ElapsedEventArgs e)
        {
            var necroBotDirectories = _directoryInfo.EnumerateDirectories().Where(x => x.EnumerateFiles().Any(y => y.Name == Settings.Default.NecroBotExeName)).ToArray();
            if (necroBotDirectories.Length == 0) return;

            foreach (var necroBotDirectory in necroBotDirectories)
            {
                var dumpsDir =
                    necroBotDirectory.EnumerateDirectories().FirstOrDefault(x => string.Equals(x.Name, Settings.Default.NecroBotDumpsDirectoryName, StringComparison.CurrentCultureIgnoreCase));

                // ReSharper disable once UseNullPropagation
                if (dumpsDir == null) continue;

                var files = dumpsDir.EnumerateFiles().Where(x => x.Name.Contains(Settings.Default.NecroBotPokeBagStatsFileName)).OrderByDescending(x => x.LastWriteTimeUtc).ToArray();
                if (files.Length == 0) continue;

                var lastFile = files.First();
                if (_lastModifiedDictionary.ContainsKey(lastFile.FullName) && _lastModifiedDictionary[lastFile.FullName] < lastFile.LastWriteTimeUtc) UpdateDumpFileContent(lastFile, necroBotDirectory.Name);
                else if (_lastModifiedDictionary.ContainsKey(lastFile.FullName) == false)
                {
                    _lastModifiedDictionary.Add(lastFile.FullName, lastFile.LastWriteTimeUtc);
                    UpdateDumpFileContent(lastFile, necroBotDirectory.Name);
                }
            }
        }

        private void UpdateDumpFileContent(FileInfo lastFile, string accountName)
        {
            var poGoAccount = PokemonGoAccounts.FirstOrDefault(x => x.Name == accountName);
            if (poGoAccount == null)
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    poGoAccount = new PokemonGoAccount(accountName);
                    _pokemonGoAccounts.Add(poGoAccount);
                });
            }

            Application.Current.Dispatcher.Invoke(() => poGoAccount.ClearPokemons());

            var lines = File.ReadAllLines(lastFile.FullName);

            foreach (var line in lines)
            {
                var split = line.Split(new[] { Settings.Default.DumpName, Settings.Default.DumpLevel, Settings.Default.DumpCp, Settings.Default.DumpIv }, StringSplitOptions.RemoveEmptyEntries);

                var name = split[Settings.Default.DumpNameOrder].Trim();

                byte level;
                if (byte.TryParse(split[Settings.Default.DumpLevelOrder].Trim(), out level) == false) continue;

                ushort cp;
                if (ushort.TryParse(split[Settings.Default.DumpCpOrder].Trim(), out cp) == false) continue;

                double iv;
                if (double.TryParse(split[Settings.Default.DumpIvOrder].Trim('%'), out iv) == false) continue;

                Application.Current.Dispatcher.Invoke(() => poGoAccount.AddPokemon(new Pokemon(name, level, cp, iv)));
            }
        }

        #endregion
    }
}