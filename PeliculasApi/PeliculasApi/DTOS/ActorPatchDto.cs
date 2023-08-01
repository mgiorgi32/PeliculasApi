using System.ComponentModel.DataAnnotations;

namespace PeliculasApi.DTOS
{
    public class ActorPatchDto
    {
        [Required]
        [StringLength(120)]
        public string Nombre { get; set; }
        public DateTime FechaNacimiento { get; set; }
    }
}
