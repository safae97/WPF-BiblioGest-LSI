// Vue du tableau de bord administrateur
using System.Windows.Controls;
using BiblioGest.ViewModels;

namespace BiblioGest.Views
{
    public partial class AdminDashboardView : UserControl
    {
        public AdminDashboardView()
        {
            InitializeComponent();
            DataContext = new AdminDashboardViewModel(); // 🔥 très important pour la liaison de données
        }
    }
}
