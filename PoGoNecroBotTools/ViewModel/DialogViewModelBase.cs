using System.Windows;

namespace PoGoNecroBotTools.ViewModel
{
    public abstract class DialogViewModelBase<T>
        where T : Window, new()
    {
        #region Constructors

        protected DialogViewModelBase()
        {
            View = new T { DataContext = this };
        }

        #endregion

        #region Properties

        protected T View { get; }

        #endregion

        #region Methods

        public bool? ShowDialog()
        {
            return View.ShowDialog();
        }

        protected void Close()
        {
            View.Close();
        }

        #endregion
    }
}