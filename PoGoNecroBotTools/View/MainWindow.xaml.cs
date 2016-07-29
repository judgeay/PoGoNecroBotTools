using System;
using PoGoNecroBotTools.ViewModel;

namespace PoGoNecroBotTools.View
{
    /// <summary>
    ///     Logique d'interaction pour MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        #region Constructors

        public MainWindow()
        {
            InitializeComponent();

            DataContext = new MainWindowViewModel();

            ContentRendered += OnContentRendered;
        }

        #endregion

        #region Methods

        private void OnContentRendered(object sender, EventArgs eventArgs)
        {
            var viewModel = DataContext as MainWindowViewModel;
            viewModel?.OnContentRendered();
        }

        #endregion
    }
}