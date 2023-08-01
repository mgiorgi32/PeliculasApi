using NetTopologySuite.Geometries;
using System.ComponentModel.DataAnnotations;

namespace PeliculasApi.Entidades
{
    public class SalaDeCine:IId
    {
        public int Id { get; set; }
        [Required]
        [StringLength(120)]
        public string Nombre { get; set; }
        public Point Ubicacion { get; set; }
        public List<PeliculasSalasDeCine> peliculasSalasDeCines { get; set; }
        public double Latitud { get; set; }
        public double Longitud { get; set; }
    }
}
