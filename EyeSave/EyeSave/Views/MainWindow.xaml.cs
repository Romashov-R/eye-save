using EyeSave.ViewModels;
using System.Windows;

namespace EyeSave.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private MainWindowViewModel _viewModel;

        public MainWindow()
        {
            InitializeComponent();
            _viewModel = (MainWindowViewModel)DataContext;
        }

        private void ListView_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if(_viewModel.SelectedAgent != null)
                new AgentWindow(_viewModel.SelectedAgent.Id).ShowDialog();            
        }

        private void btnPreviousPage_Click(object sender, RoutedEventArgs e)
        {
            if (_viewModel.SelectedPage.pageNum == 1)
                return;

            var currentPageNum = _viewModel.SelectedPage.pageNum;

            _viewModel.SelectedPage = _viewModel.Pages[currentPageNum - 2];
        }

        private void btnNextPage_Click(object sender, RoutedEventArgs e)
        {
            if (_viewModel.SelectedPage.pageNum == _viewModel.Pages.Count)
                return;

            var currentPageNum = _viewModel.SelectedPage.pageNum;

            _viewModel.SelectedPage = _viewModel.Pages[currentPageNum];
        }
    }
}
