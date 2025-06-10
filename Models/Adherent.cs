using System;
using System.ComponentModel.DataAnnotations;

namespace BiblioGest.Models
{
    public class Adherent
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Nom { get; set; }

        [Required]
        public string Prenom { get; set; }

        [Required]
        public string Adresse { get; set; }

        [Required]
        public string Email { get; set; }

        [Required]
        public string Telephone { get; set; }

        public DateTime DateInscription { get; set; } = DateTime.UtcNow;
    }
}
