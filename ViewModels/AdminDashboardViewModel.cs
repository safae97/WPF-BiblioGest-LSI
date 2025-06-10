using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using BiblioGest.Models;
using BiblioGest.Data;
using LiveCharts;
using LiveCharts.Wpf;
using System.Windows.Media;

namespace BiblioGest.ViewModels
{
    public class CategoryStat
    {
        public string Category { get; set; }
        public double Percentage { get; set; }
        public double BarWidth { get; set; }
        public Brush BarColor { get; set; }
    }

    public class AdminDashboardViewModel : INotifyPropertyChanged
    {
        private int _totalLivres;
        public int TotalLivres
        {
            get => _totalLivres;
            set { _totalLivres = value; OnPropertyChanged(); }
        }

        private int _empruntsEnCours;
        public int EmpruntsEnCours
        {
            get => _empruntsEnCours;
            set { _empruntsEnCours = value; OnPropertyChanged(); }
        }

        private int _totalAdherents;
        public int TotalAdherents
        {
            get => _totalAdherents;
            set { _totalAdherents = value; OnPropertyChanged(); }
        }

        private int _retards;
        public int Retards
        {
            get => _retards;
            set { _retards = value; OnPropertyChanged(); }
        }

        private double _pourcentageUtilisation;
        public double PourcentageUtilisation
        {
            get => _pourcentageUtilisation;
            set { _pourcentageUtilisation = value; OnPropertyChanged(); }
        }

        private double _progressWidth;
        public double ProgressWidth
        {
            get => _progressWidth;
            set { _progressWidth = value; OnPropertyChanged(); }
        }

        public SeriesCollection LivresSeries { get; set; }
        public SeriesCollection EmpruntsSeries { get; set; }
        public SeriesCollection RetardsSeries { get; set; }
        public string[] Labels { get; set; }

        public ObservableCollection<Emprunt> DerniersEmprunts { get; set; }
        public ObservableCollection<CategoryStat> CategoryStats { get; set; }

        public SeriesCollection DashboardSeries { get; set; }

        public AdminDashboardViewModel()
        {
            using var db = new AppDbContext();
            var now = DateTime.UtcNow;

            TotalLivres = db.Livres.Count();
            EmpruntsEnCours = db.Emprunts.Count(e => e.DateRetourEffectif == null);
            TotalAdherents = db.Adherents.Count();
            Retards = db.Emprunts.Count(e => e.DateRetourEffectif == null && e.DateRetourPrevu < now);

            // Calculate library usage percentage
            PourcentageUtilisation = TotalLivres > 0 ? Math.Round((double)EmpruntsEnCours / TotalLivres * 100, 2) : 0;
            ProgressWidth = PourcentageUtilisation * 2; // Scale for visual bar (max width ~200px)

            // Fetch top 3 categories by book count
            var categoryCounts = db.Livres
                .GroupBy(l => l.Categorie)
                .Select(g => new { Category = g.Key, Count = g.Count() })
                .OrderByDescending(g => g.Count)
                .Take(3)
                .ToList();

            var totalBooks = categoryCounts.Sum(c => c.Count);
            var colors = new[] { Brushes.SaddleBrown, Brushes.Goldenrod, Brushes.Peru };

            CategoryStats = new ObservableCollection<CategoryStat>();
            for (int i = 0; i < categoryCounts.Count; i++)
            {
                var percentage = totalBooks > 0 ? Math.Round((double)categoryCounts[i].Count / totalBooks * 100, 2) : 0;
                CategoryStats.Add(new CategoryStat
                {
                    Category = categoryCounts[i].Category ?? "Unknown",
                    Percentage = percentage,
                    BarWidth = percentage * 1.5, // Scale for visual bar (max width ~150px)
                    BarColor = colors[i]
                });
            }

            DashboardSeries = new SeriesCollection
            {
                new ColumnSeries
                {
                    Title = "Livres",
                    Values = new ChartValues<int> { TotalLivres },
                    Fill = Brushes.Green
                },
                new ColumnSeries
                {
                    Title = "Emprunts",
                    Values = new ChartValues<int> { EmpruntsEnCours },
                    Fill = Brushes.DodgerBlue
                },
                new ColumnSeries
                {
                    Title = "Retards",
                    Values = new ChartValues<int> { Retards },
                    Fill = Brushes.Red
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