using System;
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
    public class EmpruntViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<Emprunt> Emprunts { get; set; }
        public ObservableCollection<Livre> Livres { get; set; }
        public ObservableCollection<Adherent> Adherents { get; set; }

        public Emprunt NewEmprunt { get; set; } = new Emprunt
        {
            DateEmprunt = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc),
            DateRetourPrevu = DateTime.SpecifyKind(DateTime.Now.AddDays(15), DateTimeKind.Utc)
        };
        public Emprunt SelectedEmprunt { get; set; }

        public ICommand AddEmpruntCommand { get; }
        public ICommand ReturnEmpruntCommand { get; }

        public EmpruntViewModel()
        {
            Emprunts = new ObservableCollection<Emprunt>();
            Livres = new ObservableCollection<Livre>();
            Adherents = new ObservableCollection<Adherent>();

            LoadData();

            AddEmpruntCommand = new RelayCommand(_ => AjouterEmprunt());
            ReturnEmpruntCommand = new RelayCommand(_ => RetournerEmprunt());
        }

        private void LoadData()
        {
            using var db = new AppDbContext();
            Emprunts.Clear();
            Livres.Clear();
            Adherents.Clear();

            foreach (var emp in db.Emprunts.ToList()) Emprunts.Add(emp);
            foreach (var l in db.Livres.ToList()) Livres.Add(l);
            foreach (var a in db.Adherents.ToList()) Adherents.Add(a);
        }

        private void AjouterEmprunt()
        {
            try
            {
                // 🛑 Conversion obligatoire pour PostgreSQL
                NewEmprunt.DateEmprunt = DateTime.SpecifyKind(NewEmprunt.DateEmprunt, DateTimeKind.Utc);
                NewEmprunt.DateRetourPrevu = DateTime.SpecifyKind(NewEmprunt.DateRetourPrevu, DateTimeKind.Utc);

                using var db = new AppDbContext();
                db.Emprunts.Add(NewEmprunt);
                db.SaveChanges();

                Emprunts.Add(new Emprunt
                {
                    ISBN = NewEmprunt.ISBN,
                    AdherentId = NewEmprunt.AdherentId,
                    DateEmprunt = NewEmprunt.DateEmprunt,
                    DateRetourPrevu = NewEmprunt.DateRetourPrevu
                });

                NewEmprunt = new Emprunt
                {
                    DateEmprunt = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc),
                    DateRetourPrevu = DateTime.SpecifyKind(DateTime.Now.AddDays(15), DateTimeKind.Utc)
                };
                OnPropertyChanged(nameof(NewEmprunt));
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show("💥 Erreur ajout : " + (ex.InnerException?.Message ?? ex.Message));
            }
        }


        private void RetournerEmprunt()
        {
            if (SelectedEmprunt == null) return;

            using var db = new AppDbContext();
            var emp = db.Emprunts.FirstOrDefault(e => e.Id == SelectedEmprunt.Id);
            if (emp != null && emp.DateRetourEffectif == null)
            {
                emp.DateRetourEffectif = DateTime.UtcNow;
                db.SaveChanges();

                SelectedEmprunt.DateRetourEffectif = emp.DateRetourEffectif;
                OnPropertyChanged(nameof(SelectedEmprunt));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
