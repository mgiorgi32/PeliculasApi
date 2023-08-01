using System.ComponentModel.DataAnnotations;

namespace PeliculasApi.DTOS
{
    public class ReviewCreacionDto
    {
        public string Comentario { get; set; }
        [Range(1,5)]
        public int Puntuacion { get; set; }
    }
}
