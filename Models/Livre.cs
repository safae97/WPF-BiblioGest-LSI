using System.ComponentModel.DataAnnotations;

namespace BiblioGest.Models
{
    public class Livre
    {
        [Key] 
        public string ISBN { get; set; }

        public string Titre { get; set; }
        public string Auteur { get; set; }
        public string Editeur { get; set; }
        public int AnneePublication { get; set; }
        public string Categorie { get; set; }
        public int NombreExemplaires { get; set; }
    }
}
