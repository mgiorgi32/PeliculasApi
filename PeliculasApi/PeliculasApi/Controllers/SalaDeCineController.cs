using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;
using PeliculasApi.DTOS;
using PeliculasApi.Entidades;

namespace PeliculasApi.Controllers
{
    [Route("api/SalasDeCine")]
    [ApiController]
    public class SalaDeCineController: CustomBaseController
    {
        private readonly ApplicationDBContext context;
        private readonly IMapper mapper;
        private readonly GeometryFactory geometryFactory;

        public SalaDeCineController(ApplicationDBContext context, IMapper mapper, GeometryFactory geometryFactory): base(context, mapper)
        {
            this.context = context;
            this.mapper = mapper;
            this.geometryFactory = geometryFactory;
        }
        [HttpGet]
        public async Task<ActionResult<List<SalaDeCineDto>>> Get()
        {
            return await Get<SalaDeCine, SalaDeCineDto>();
        }
        [HttpGet("{id:int}", Name ="obtenerSalaDeCine")]
        public async Task<ActionResult<SalaDeCineDto>> Get(int id)
        {
            return await Get<SalaDeCine, SalaDeCineDto>(id);
        }
        [HttpGet("Cercanos")]
        public async Task<ActionResult<List<SalaDeCineCercanoDto>>> Cercanos([FromQuery] SalaDeCineCercanoFiltroDto filtro)
        {
            var ubicacionUsusario = geometryFactory.CreatePoint(new Coordinate(filtro.Longitud, filtro.Latitud));
            var salasDeCine = await context.SalaDeCines.OrderBy(x => x.Ubicacion.Distance(ubicacionUsusario))
                .Where(x => x.Ubicacion.IsWithinDistance(ubicacionUsusario, filtro.DistanciaEnKms * 1000))
                .Select(x => new SalaDeCineCercanoDto
            {
                Id = x.Id,
                Nombre = x.Nombre,
                Latitud = x.Ubicacion.Y,
                Longitud = x.Ubicacion.X,
                distanciaEnMetros = Math.Round(x.Ubicacion.Distance(ubicacionUsusario))
            }).ToListAsync();
            return salasDeCine;
                
        }
        [HttpPost]
        public async Task<ActionResult> Post([FromBody] SalaDeCineCreacionDto salaDeCineCreacionDto)
        {
            return await Post<SalaDeCineCreacionDto, SalaDeCine, SalaDeCineDto>(salaDeCineCreacionDto, "obtenerSalaDeCine");
        }
        [HttpPut("{id:int}")]
        public async Task<ActionResult> Put(int id, [FromBody] SalaDeCineCreacionDto salaDeCineCreacionDto)
        {
            return await Put<SalaDeCineCreacionDto, SalaDeCine>(id, salaDeCineCreacionDto);
        }
        [HttpDelete("{id:int}")]
        public async Task<ActionResult> Delete(int id)
        {
            return await Delete<SalaDeCine>(id);
        }
    }
}
