using System.Windows.Input;
using PoGoNecroBotTools.ViewModel;

namespace PoGoNecroBotTools.View
{
    /// <summary>
    ///     Logique d'interaction pour AddLocationDialog.xaml
    /// </summary>
    public partial class AddLocationDialog
    {
        #region Constructors

        public AddLocationDialog()
        {
            InitializeComponent();

            KeyUp += OnKeyUp;
        }

        #endregion

        #region Methods

        private void OnKeyUp(object sender, KeyEventArgs keyEventArgs)
        {
            if (keyEventArgs.Key != Key.Enter) return;

            var viewModel = DataContext as AddLocationDialogViewModel;
            if (viewModel?.OkCommand.CanExecute(null) == true) viewModel?.OkCommand.Execute(null);
        }

        #endregion
    }
}