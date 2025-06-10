using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using BiblioGest.Models;
using BiblioGest.Data;
using BiblioGest.Commands;

namespace BiblioGest.ViewModels
{
    public class LivreViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<Livre> Livres { get; set; }
        public ICommand UpdateLivreCommand { get; }


        private Livre _newLivre = new Livre();
        public Livre NewLivre
        {
            get => _newLivre;
            set
            {
                _newLivre = value;
                OnPropertyChanged();
            }
        }

        private Livre _selectedLivre;
        public Livre SelectedLivre
        {
            get => _selectedLivre;
            set
            {
                _selectedLivre = value;
                OnPropertyChanged();
            }
        }

        public ICommand AddLivreCommand { get; }
        public ICommand DeleteLivreCommand { get; }

        public LivreViewModel()
        {
            Livres = new ObservableCollection<Livre>();
            LoadLivres();

            AddLivreCommand = new RelayCommand(_ => AjouterLivre());
            UpdateLivreCommand = new RelayCommand(_ => ModifierLivre());

            DeleteLivreCommand = new RelayCommand(_ => SupprimerLivre());
        }

        private void LoadLivres()
        {
            using var db = new AppDbContext();
            Livres.Clear();
            foreach (var livre in db.Livres.ToList())
            {
                Livres.Add(livre);
            }
        }

        private void ModifierLivre()
        {
            if (SelectedLivre == null) return;

            using var db = new AppDbContext();
            var livre = db.Livres.FirstOrDefault(l => l.ISBN == SelectedLivre.ISBN);
            if (livre != null)
            {
                livre.Titre = SelectedLivre.Titre;
                livre.Auteur = SelectedLivre.Auteur;
                livre.Editeur = SelectedLivre.Editeur;
                livre.AnneePublication = SelectedLivre.AnneePublication;
                livre.Categorie = SelectedLivre.Categorie;
                livre.NombreExemplaires = SelectedLivre.NombreExemplaires;

                db.SaveChanges();
                LoadLivres(); // Recharge la liste pour que les modifs apparaissent
            }
        }


        private void AjouterLivre()
        {
            if (string.IsNullOrWhiteSpace(NewLivre.ISBN)) return;

            using var db = new AppDbContext();
            db.Livres.Add(NewLivre);
            db.SaveChanges();

            Livres.Add(new Livre
            {
                ISBN = NewLivre.ISBN,
                Titre = NewLivre.Titre,
                Auteur = NewLivre.Auteur,
                Editeur = NewLivre.Editeur,
                AnneePublication = NewLivre.AnneePublication,
                Categorie = NewLivre.Categorie,
                NombreExemplaires = NewLivre.NombreExemplaires
            });

            NewLivre = new Livre(); // reset le formulaire
        }

        private void SupprimerLivre()
        {
            if (SelectedLivre == null) return;

            using var db = new AppDbContext();
            var livre = db.Livres.FirstOrDefault(l => l.ISBN == SelectedLivre.ISBN);
            if (livre != null)
            {
                db.Livres.Remove(livre);
                db.SaveChanges();
                Livres.Remove(SelectedLivre);
                SelectedLivre = null;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
