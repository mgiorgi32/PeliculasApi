namespace PeliculasApi.DTOS
{
    public class FiltroPeliculasDto
    {
        public int Pagina { get; set; } = 1;
        public int CantidadRegistrosPorPagina { get; set; } = 10;
        public PaginacionDto paginacion
        {
            get
            {
                return new PaginacionDto() { Pagina = Pagina, CanridadRegistrosPorPagina = CantidadRegistrosPorPagina };
            }
        }
        public string Titulo { get; set; }
        public int GeneroId { get; set; }
        public bool EnCines { get; set; }
        public bool ProximoEstreno { get; set; }
        public string CampoOrdenar { get; set; }
        public bool OrdenAsendente { get; set; } = true;

    }
}
