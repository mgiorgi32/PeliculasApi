namespace PeliculasApi.Entidades
{
    public class PeliculasGeneros
    {
        public int GeneroId { get; set; }
        public int PeliculaId { get; set; }
        public Genero genero { get; set; }
        public Pelicula pelicula { get; set; }
    }
}
