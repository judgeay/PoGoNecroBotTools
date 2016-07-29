using System.Globalization;
using GalaSoft.MvvmLight.Command;
using PoGoNecroBotTools.View;

namespace PoGoNecroBotTools.ViewModel
{
    public class AddLocationDialogViewModel : DialogViewModelBase<AddLocationDialog>
    {
        #region Fields

        private string _locationLatitude;
        private string _locationLongitude;
        private string _locationTitle;

        private RelayCommand _okCommand;

        #endregion

        #region Constructors

        #endregion

        #region Properties

        public RelayCommand CancelCommand => new RelayCommand(CancelAction);

        public double DoubleLocationLatitude => LatitudeLongitudeParse(LocationLatitude);

        public double DoubleLocationLongitude => LatitudeLongitudeParse(LocationLongitude);

        public string LocationLatitude
        {
            get { return _locationLatitude; }
            set
            {
                _locationLatitude = value;
                OkCommand.RaiseCanExecuteChanged();
            }
        }

        public string LocationLongitude
        {
            get { return _locationLongitude; }
            set
            {
                _locationLongitude = value;
                OkCommand.RaiseCanExecuteChanged();
            }
        }

        public string LocationTitle
        {
            get { return _locationTitle; }
            set
            {
                _locationTitle = value;
                OkCommand.RaiseCanExecuteChanged();
            }
        }

        public RelayCommand OkCommand
        {
            // ReSharper disable once ConvertPropertyToExpressionBody
            get { return _okCommand ?? (_okCommand = new RelayCommand(OkAction, OkCanAction)); }
        }

        #endregion

        #region Methods

        private static double LatitudeLongitudeParse(string value)
        {
            double temp;
            if (double.TryParse(value, NumberStyles.Any, CultureInfo.CurrentCulture, out temp)) return temp;
            if (double.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out temp)) return temp;
            return double.NaN;
        }

        private void CancelAction()
        {
            View.DialogResult = false;
            Close();
        }

        private void OkAction()
        {
            View.DialogResult = true;
            Close();
        }

        private bool OkCanAction()
        {
            if (string.IsNullOrWhiteSpace(LocationTitle)) return false;
            if (string.IsNullOrWhiteSpace(LocationLatitude) || DoubleLocationLatitude.CompareTo(double.NaN) == 0) return false;
            if (string.IsNullOrWhiteSpace(LocationLongitude) || DoubleLocationLongitude.CompareTo(double.NaN) == 0) return false;

            return true;
        }

        #endregion
    }
}