using System.Windows.Controls;
using BiblioGest.ViewModels;

namespace BiblioGest.Views
{
    public partial class MainAppView : UserControl
    {
        public MainAppView()
        {
            InitializeComponent();
            DataContext = new MainViewModel();
        }
    }
}
