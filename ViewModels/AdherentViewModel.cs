using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using BiblioGest.Models;
using BiblioGest.Data;
using BiblioGest.Commands;
using System.Windows;

namespace BiblioGest.ViewModels
{
    public class AdherentViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<Adherent> Adherents { get; set; } = new();

        private Adherent _newAdherent = new Adherent { DateInscription = DateTime.Now };
        public Adherent NewAdherent
        {
            get => _newAdherent;
            set { _newAdherent = value; OnPropertyChanged(); }
        }

        private Adherent _selectedAdherent;
        public Adherent SelectedAdherent
        {
            get => _selectedAdherent;
            set { _selectedAdherent = value; OnPropertyChanged(); }
        }

        public ICommand AddAdherentCommand { get; }
        public ICommand UpdateAdherentCommand { get; }
        public ICommand DeleteAdherentCommand { get; }

        public AdherentViewModel()
        {
            LoadAdherents();
            AddAdherentCommand = new RelayCommand(_ => AjouterAdherent());
            UpdateAdherentCommand = new RelayCommand(_ => ModifierAdherent());
            DeleteAdherentCommand = new RelayCommand(_ => SupprimerAdherent());
        }

        private void LoadAdherents()
        {
            using var db = new AppDbContext();
            Adherents.Clear();
            foreach (var adh in db.Adherents.ToList())
                Adherents.Add(adh);
        }

        private void AjouterAdherent()
        {
            if (string.IsNullOrWhiteSpace(NewAdherent.Nom) ||
                string.IsNullOrWhiteSpace(NewAdherent.Prenom) ||
                string.IsNullOrWhiteSpace(NewAdherent.Adresse) ||
                string.IsNullOrWhiteSpace(NewAdherent.Email) ||
                string.IsNullOrWhiteSpace(NewAdherent.Telephone))
            {
                MessageBox.Show("Veuillez remplir tous les champs obligatoires !");
                return;
            }

            try
            {
                using var db = new AppDbContext();

                // On crée un nouveau Adherent sans insérer NewAdherent directement
                var nouvelAdherent = new Adherent
                {
                    Nom = NewAdherent.Nom,
                    Prenom = NewAdherent.Prenom,
                    Adresse = NewAdherent.Adresse,
                    Email = NewAdherent.Email,
                    Telephone = NewAdherent.Telephone,
                    DateInscription = DateTime.UtcNow
                };

                db.Adherents.Add(nouvelAdherent);
                db.SaveChanges();

                // Ajoute à la liste en local
                Adherents.Add(nouvelAdherent);

                // Réinitialise le formulaire
                NewAdherent = new Adherent { DateInscription = DateTime.Now };
                OnPropertyChanged(nameof(NewAdherent));
            }
            catch (Exception ex)
            {
                var fullMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    fullMessage += "\n\n➡️ INNER EXCEPTION:\n" + ex.InnerException.Message;

                    if (ex.InnerException.InnerException != null)
                    {
                        fullMessage += "\n\n➡️ DEEPER EXCEPTION:\n" + ex.InnerException.InnerException.Message;
                    }
                }

                System.Windows.MessageBox.Show("Erreur ajout : \n" + fullMessage);
            }

        }


        private void ModifierAdherent()
        {
            if (SelectedAdherent == null) return;
            using var db = new AppDbContext();
            var adh = db.Adherents.FirstOrDefault(a => a.Id == SelectedAdherent.Id);
            if (adh != null)
            {
                adh.Nom = SelectedAdherent.Nom;
                adh.Prenom = SelectedAdherent.Prenom;
                adh.Adresse = SelectedAdherent.Adresse;
                adh.Email = SelectedAdherent.Email;
                adh.Telephone = SelectedAdherent.Telephone;
                db.SaveChanges();
                LoadAdherents();
            }
        }

        private void SupprimerAdherent()
        {
            if (SelectedAdherent == null) return;
            using var db = new AppDbContext();
            var adh = db.Adherents.FirstOrDefault(a => a.Id == SelectedAdherent.Id);
            if (adh != null)
            {
                db.Adherents.Remove(adh);
                db.SaveChanges();
                Adherents.Remove(SelectedAdherent);
                SelectedAdherent = null;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
