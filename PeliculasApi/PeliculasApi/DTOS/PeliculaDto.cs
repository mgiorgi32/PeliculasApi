using PeliculasApi.Entidades;
using System.ComponentModel.DataAnnotations;

namespace PeliculasApi.DTOS
{
    public class PeliculaDto
    {
        public int Id { get; set; }

        public string Titulo { get; set; }
        public bool EnCines { get; set; }
        public DateTime FechaEstreno { get; set; }
        public string Poster { get; set; }
        public List<PeliculaActor> peliculasActores { get; set; }   
        public List<PeliculasGeneros> peliculasGeneros { get; set; }
    }
}
