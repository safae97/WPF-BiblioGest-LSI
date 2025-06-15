// Vue du tableau de bord administrateur
using System.Windows.Controls;
using BiblioGest.ViewModels;

namespace BiblioGest.Views
{
    /// <summary>
    /// Vue pour le tableau de bord administrateur
    /// </summary>
    public partial class AdminDashboardView : UserControl
    {
        public AdminDashboardView()
        {
            InitializeComponent();
            DataContext = new AdminDashboardViewModel(); // 🔥 très important pour la liaison de données
        }
    }
}
