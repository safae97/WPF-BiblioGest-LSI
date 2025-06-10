using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using BiblioGest.Commands;
using BiblioGest.Views;

namespace BiblioGest.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        // Commandes de navigation
        public ICommand ShowLivreViewCommand { get; }
        public ICommand ShowAdherentViewCommand { get; }
        public ICommand ShowEmpruntViewCommand { get; }
        public ICommand ShowDashboardViewCommand { get; }
        public ICommand ShowAdminDashboardCommand { get; }

        // Vue courante affichée
        private object _currentView;
        public object CurrentView
        {
            get => _currentView;
            set
            {
                _currentView = value;
                OnPropertyChanged();
            }
        }

        public MainViewModel()
        {
            // Create custom welcome screen instead of simple text
            CurrentView = CreateWelcomeView();

            ShowLivreViewCommand = new RelayCommand(_ => ShowViewSafe(() => new LivreView()));
            ShowAdherentViewCommand = new RelayCommand(_ => ShowViewSafe(() => new AdherentView()));
            ShowEmpruntViewCommand = new RelayCommand(_ => ShowViewSafe(() => new EmpruntView()));
            ShowAdminDashboardCommand = new RelayCommand(_ => ShowViewSafe(() => new AdminDashboardView()));
        }

        private UserControl CreateWelcomeView()
        {
            var welcomeView = new UserControl();

            // Create main grid
            var mainGrid = new Grid();
            mainGrid.Background = new SolidColorBrush(Colors.White);

            // Create main border with styling
            var mainBorder = new Border
            {
                Background = new SolidColorBrush(Color.FromRgb(245, 245, 245)),
                CornerRadius = new CornerRadius(20),
                Padding = new Thickness(40),
                Margin = new Thickness(20),
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Center,
                Width = 500,
                Height = 400 // Reduced height since less content
            };

            // Create content stack panel
            var contentStack = new StackPanel
            {
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            };

            // Library logo/icon
            var logoContainer = new Border
            {
                Background = new SolidColorBrush(Color.FromRgb(139, 69, 19)), // #8B4513
                CornerRadius = new CornerRadius(75),
                Width = 150,
                Height = 150,
                HorizontalAlignment = HorizontalAlignment.Center,
                Margin = new Thickness(0, 0, 0, 30)
            };

            

            // Library name
            var libraryName = new TextBlock
            {
                Text = "Bibliothèque LSI",
                FontSize = 25,
                FontWeight = FontWeights.Bold,
                Foreground = new SolidColorBrush(Color.FromRgb(139, 69, 19)),
                HorizontalAlignment = HorizontalAlignment.Center,
                Margin = new Thickness(0, 0, 0, 15)
            };
            contentStack.Children.Add(libraryName);

            // Subtitle
            var subtitle = new TextBlock
            {
                Text = "Système de Gestion Bibliothécaire",
                FontSize = 18,
                FontWeight = FontWeights.SemiBold,
                Foreground = new SolidColorBrush(Color.FromRgb(100, 100, 100)),
                HorizontalAlignment = HorizontalAlignment.Center,
                Margin = new Thickness(0, 0, 0, 30)
            };
            contentStack.Children.Add(subtitle);

            // Image (logo)
            var logoImage = new Image
            {
                Source = new BitmapImage(new Uri("pack://application:,,,/Images/Library.png")),
                Width = 80,
                Height = 80
            };
            contentStack.Children.Add(logoImage);

            // Add content to border and grid
            mainBorder.Child = contentStack;
            mainGrid.Children.Add(mainBorder);
            welcomeView.Content = mainGrid;

            return welcomeView;
        }


        // Méthode utilitaire pour éviter les crash si une vue plante
        private void ShowViewSafe(Func<object> createView)
        {
            try
            {
                CurrentView = createView();
            }
            catch (Exception ex)
            {
                CurrentView = new TextBlock
                {
                    Text = $"❌ Erreur de chargement : {ex.Message}",
                    Foreground = Brushes.Red,
                    Margin = new Thickness(20)
                };
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}