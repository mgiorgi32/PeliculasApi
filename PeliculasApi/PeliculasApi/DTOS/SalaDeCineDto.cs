using NetTopologySuite.Geometries;

namespace PeliculasApi.DTOS
{
    public class SalaDeCineDto
    {
        public int Id { get; set; }
        public string Nombre{ get; set; }
        public double Latitud { get; set; }
        public double Longitud { get; set; }
        
    }
}
