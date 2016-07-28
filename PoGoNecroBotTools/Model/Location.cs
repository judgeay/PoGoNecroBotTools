using System;
using System.ComponentModel;
using System.Windows;

namespace PoGoNecroBotTools.Model
{
    [Serializable]
    public sealed class Location : INotifyPropertyChanged
    {
        #region Fields

        private bool _isDefault;

        #endregion

        #region Constructors

        public Location(string title, double latitude, double longitude)
        {
            Title = title;
            Latitude = latitude;
            Longitude = longitude;
        }

        #endregion

        #region Properties

        public bool IsDefault
        {
            get { return _isDefault; }
            set
            {
                _isDefault = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsDefaultVisibility)));
            }
        }

        public Visibility IsDefaultVisibility => IsDefault ? Visibility.Visible : Visibility.Collapsed;

        public double Latitude { get; private set; }

        public double Longitude { get; private set; }

        public string Title { get; private set; }

        #endregion

        #region INotifyPropertyChanged Members

        [field: NonSerialized]
        public event PropertyChangedEventHandler PropertyChanged;

        #endregion
    }
}