namespace PeliculasApi.Entidades
{
    public class PeliculaActor
    {
        public int ActorId { get; set; }
        public int PeliculaId { get; set; }
        public string Personaje { get; set; }
        public int Orden { get; set; }
        public Actor actor { get; set; }
        public Pelicula Pelicula { get; set; }
    }
}
