using Microsoft.AspNetCore.Mvc;
using PeliculasApi.Entidades;
using PeliculasApi.Helpers;
using PeliculasApi.Validaciones;
using System.ComponentModel.DataAnnotations;

namespace PeliculasApi.DTOS
{
    public class PeliculaCreacionDto
    {
        public int Id { get; set; }
        [Required]
        [StringLength(300)]
        public string Titulo { get; set; }
        public bool EnCines { get; set; }
        public DateTime FechaEstreno { get; set; }
        [PesoArchivoValiidacion(pesoMaximoEnMegaBytes: 4)]
        [TipoArchivoValidacion(GrupoTipoArchivo.Imagen)]
        public IFormFile Poster { get; set; }
        [ModelBinder(BinderType  = typeof(TypeBinder<List<int>>))]
        public List<int> GenerosIds { get; set; }
        [ModelBinder(BinderType = typeof(TypeBinder<List<ActorPeliculasCreacionDto>>))]
        public List<ActorPeliculasCreacionDto> Actores {  get; set; }
    }
}
