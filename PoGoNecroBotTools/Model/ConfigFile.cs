using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using PoGoNecroBotTools.Properties;

namespace PoGoNecroBotTools.Model
{
    public class ConfigFile
    {
        #region Fields

        private readonly string _path;

        #endregion

        #region Constructors

        public ConfigFile(string path)
        {
            _path = path;

            Configuration = ParseJsonFile(path);
        }

        #endregion

        #region Properties

        private Dictionary<string, string> Configuration { get; }

        #endregion

        #region Methods

        private static string ConfigWriteLine(string configLine, KeyValuePair<string, string> line)
        {
            const string splitter = "\":";
            var splitterIndex = configLine.IndexOf(splitter, StringComparison.Ordinal) + splitter.Length;
            var endOfLineIndex = configLine.IndexOf(",", StringComparison.Ordinal);

            configLine = configLine.Replace(configLine.Substring(splitterIndex, endOfLineIndex - splitterIndex), line.Value);

            return configLine;
        }

        private static Dictionary<string, string> ParseJsonFile(string path)
        {
            var config = File.ReadAllLines(path).Select(line => line.Trim().Trim(',')).Where(x => x.Length > 1).ToList();

            var removeMode = false;
            foreach (var line in config.ToArray())
            {
                if (line.Contains("["))
                {
                    removeMode = true;
                    config.Remove(line);
                }
                else if (line.Contains("]"))
                {
                    removeMode = false;
                    config.Remove(line);
                }
                else if (removeMode) config.Remove(line);
            }

            var configDictionary = new Dictionary<string, string>();

            foreach (var line in config)
            {
                var splittedLine = line.Split(new[] { ':' }, StringSplitOptions.RemoveEmptyEntries).Select(x => x.Trim()).ToArray();
                configDictionary.Add(splittedLine[0].Trim('\"'), splittedLine[1]);
            }

            return configDictionary;
        }

        private static void UpdateConfiguration(string path, Dictionary<string, string> configuration)
        {
            var configLines = File.ReadAllLines(path);
            var oldConfiguration = ParseJsonFile(path);

            foreach (var oldLine in oldConfiguration)
            {
                var line = configuration.First(x => x.Key == oldLine.Key);
                if (line.Value != oldLine.Value)
                {
                    var fileLine = configLines.First(x => x.Contains(oldLine.Key));
                    var fileLineIndex = Array.IndexOf(configLines, fileLine);

                    configLines[fileLineIndex] = ConfigWriteLine(configLines[fileLineIndex], line);
                }
            }

            File.WriteAllLines(path, configLines);
        }

        public void UpdateDefaultLatitudeLongitude(double latitude, double longitude)
        {
            Configuration[Settings.Default.DefaultLatitudeName] = latitude.ToString(CultureInfo.InvariantCulture);
            Configuration[Settings.Default.DefaultLongitudeName] = longitude.ToString(CultureInfo.InvariantCulture);

            UpdateConfiguration(_path, Configuration);
        }

        #endregion
    }
}