using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using BiblioGest.Models;
using BiblioGest.Data;
using LiveCharts;
using LiveCharts.Wpf;

namespace BiblioGest.ViewModels
{
    public class AdminDashboardViewModel : INotifyPropertyChanged
    {
        public int TotalLivres { get; set; }
        public int EmpruntsEnCours { get; set; }
        public int Retards { get; set; }

        public SeriesCollection LivresSeries { get; set; }
        public SeriesCollection EmpruntsSeries { get; set; }
        public SeriesCollection RetardsSeries { get; set; }
        public string[] Labels { get; set; }

        public ObservableCollection<Emprunt> DerniersEmprunts { get; set; }

        public SeriesCollection DashboardSeries { get; set; }

        public AdminDashboardViewModel()
        {
            using var db = new AppDbContext();
            var now = DateTime.UtcNow;

            TotalLivres = db.Livres.Count();
            EmpruntsEnCours = db.Emprunts.Count(e => e.DateRetourEffectif == null);
            Retards = db.Emprunts.Count(e => e.DateRetourEffectif == null && e.DateRetourPrevu < now);

            DashboardSeries = new SeriesCollection
    {
        new ColumnSeries
        {
            Title = "Livres",
            Values = new ChartValues<int> { TotalLivres },
            Fill = System.Windows.Media.Brushes.Green
        },
        new ColumnSeries
        {
            Title = "Emprunts",
            Values = new ChartValues<int> { EmpruntsEnCours },
            Fill = System.Windows.Media.Brushes.DodgerBlue
        },
        new ColumnSeries
        {
            Title = "Retards",
            Values = new ChartValues<int> { Retards },
            Fill = System.Windows.Media.Brushes.Red
        }
    };

            Labels = new[] { "Statistiques" };

            DerniersEmprunts = new ObservableCollection<Emprunt>(
                db.Emprunts.OrderByDescending(e => e.DateEmprunt).Take(5).ToList()
            );
        }


        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
