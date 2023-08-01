using PeliculasApi.Entidades;
using System.ComponentModel.DataAnnotations;

namespace PeliculasApi.DTOS
{
    public class GeneroDto
    {
        public int Id { get; set; }
        [Required]
        [StringLength(40)]
        public string Nombre { get; set; }   
        public List<PeliculasGeneros> peliculasGeneros { get; set; }
    }
}
