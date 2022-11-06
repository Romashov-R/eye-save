using EyeSave.ViewModels;
using System.Windows;

namespace EyeSave.Views
{
    /// <summary>
    /// Interaction logic for AgentWindow.xaml
    /// </summary>
    public partial class AgentWindow : Window
    {
        private AgentViewModel _viewModel;

        public AgentWindow(int? agentId)
        {
            InitializeComponent();
            
            _viewModel = new AgentViewModel(agentId);
            DataContext = _viewModel;
        }

        private void btnRemove_Click(object sender, RoutedEventArgs e)
        {
            _viewModel.DeleteSelectedProductSale();
        }
    }
}
