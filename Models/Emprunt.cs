using System;
using System.ComponentModel.DataAnnotations;

namespace BiblioGest.Models
{
    public class Emprunt
    {
        [Key]
        public int Id { get; set; }
        public string ISBN { get; set; }
        public int AdherentId { get; set; }
        public DateTime DateEmprunt { get; set; }
        public DateTime DateRetourPrevu { get; set; }
        public DateTime? DateRetourEffectif { get; set; }
    }
}