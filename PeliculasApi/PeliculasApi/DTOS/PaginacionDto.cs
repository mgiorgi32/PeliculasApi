namespace PeliculasApi.DTOS
{
    public class PaginacionDto
    {
        public int Pagina { get; set; }
        private int cantidadRegistrosPaginas = 10;
        private readonly int cantidadMaximaPagina = 50;
        public int CanridadRegistrosPorPagina
        {
            get => cantidadRegistrosPaginas; 
            set
            {
                cantidadRegistrosPaginas = (value > cantidadMaximaPagina) ? cantidadMaximaPagina : value;
            }
        }
    }
}
