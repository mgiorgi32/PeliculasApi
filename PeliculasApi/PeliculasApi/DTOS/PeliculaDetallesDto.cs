namespace PeliculasApi.DTOS
{
    public class PeliculaDetallesDto: PeliculaDto
    {
        public List<GeneroDto> Generos { get; set; }
        public List<ActorPeliculaDetalleDto> Actores { get; set; }
    }
}
