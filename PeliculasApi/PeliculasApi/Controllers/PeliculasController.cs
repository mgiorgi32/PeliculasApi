using AutoMapper;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PeliculasApi.DTOS;
using PeliculasApi.Entidades;
using PeliculasApi.Helpers;
using PeliculasApi.Servicios;
using System.Linq.Dynamic.Core;

namespace PeliculasApi.Controllers
{
    [ApiController]
    [Route("api/peliculas")]
    public class PeliculasController : CustomBaseController
    {
        private readonly ApplicationDBContext context;
        private readonly IMapper mapper;
        private readonly IAlmacenadorArchivos almacenadorArchivos;
        private readonly ILogger logger;
        private readonly string contenedor = "pelicula";

        public PeliculasController(ApplicationDBContext context, IMapper mapper, IAlmacenadorArchivos almacenadorArchivos, ILogger logger): base(context, mapper)
        {
            this.context = context;
            this.mapper = mapper;
            this.almacenadorArchivos = almacenadorArchivos;
            this.logger = logger;
        }
        [HttpGet]
        public async Task<ActionResult<PeliculasIndexDto>> Get()
        {
            var top = 5;
            var hoy = DateTime.Today;
            var proximosEstrenos = await context.Peliculas.Where(x => x.FechaEstreno > hoy).OrderBy(x => x.FechaEstreno).Take(top).ToListAsync();
            var enCines = await context.Peliculas.Where(x => x.EnCines).Take(top).ToListAsync();
            var resultado = new PeliculasIndexDto();
            resultado.FuturosEstrenos = mapper.Map<List<PeliculaDto>>(proximosEstrenos);
            resultado.EnCines = mapper.Map<List<PeliculaDto>>(enCines);
            return resultado;
        }
        [HttpGet("filtro")]
        public async Task<ActionResult<List<PeliculaDto>>> Filtrar([FromQuery]FiltroPeliculasDto filtroPeliculasDto)
        {
            var peliculasQueryable = context.Peliculas.AsQueryable();
            if (!string.IsNullOrEmpty(filtroPeliculasDto.Titulo))
            {
                peliculasQueryable = peliculasQueryable.Where(x => x.Titulo.Contains(filtroPeliculasDto.Titulo));
            }
            if (filtroPeliculasDto.EnCines)
            {
                peliculasQueryable = peliculasQueryable.Where(x => x.EnCines);
            }
            if (filtroPeliculasDto.ProximoEstreno)
            {
                var hoy = DateTime.Today;
                peliculasQueryable = peliculasQueryable.Where(x => x.FechaEstreno > hoy);
            }
            if (filtroPeliculasDto.GeneroId != 0)
            {
                peliculasQueryable = peliculasQueryable.Where(x => x.peliculasGeneros.Select(y => y.GeneroId).Contains(filtroPeliculasDto.GeneroId));
            }
            if (!string.IsNullOrEmpty(filtroPeliculasDto.CampoOrdenar))
            {
                var tipoOrden = filtroPeliculasDto.OrdenAsendente ? "ascending" : "descending";
                try
                {
                    peliculasQueryable = peliculasQueryable.OrderBy($"{filtroPeliculasDto.CampoOrdenar} {tipoOrden}");
                }
                catch  ( Exception ex )
                {
                    logger.LogError(ex.Message, ex);
                }
                
            }
            await HttpContext.InsertarParametrosPaginacion(peliculasQueryable, filtroPeliculasDto.CantidadRegistrosPorPagina);
            var peliculas = await peliculasQueryable.Paginar(filtroPeliculasDto.paginacion).ToListAsync();
            return mapper.Map<List<PeliculaDto>>(peliculas);
        }
        [HttpGet("{id}", Name = "obtenerPelicula")]
        public async Task<ActionResult<PeliculaDetallesDto>> Get(int id)
        {
            var pelicula = await context.Peliculas.Include(x => x.peliculasActores).ThenInclude(x => x.actor).Include(x => x.peliculasGeneros).ThenInclude(x => x.genero).FirstOrDefaultAsync(x => x.Id == id);
            if (pelicula == null) { return NotFound(); }
            pelicula.peliculasActores = pelicula.peliculasActores.OrderBy(x => x.Orden).ToList();
            return mapper.Map<PeliculaDetallesDto>(pelicula);
        }
        [HttpPost]
        public async Task<ActionResult> Post([FromForm] PeliculaCreacionDto peliculaDto)
        {
            var pelicula = mapper.Map<Pelicula>(peliculaDto);
            
            if (peliculaDto.Poster != null)
            {
                using (var memoryStream = new MemoryStream())
                {
                    await peliculaDto.Poster.CopyToAsync(memoryStream);
                    var contenido = memoryStream.ToArray();
                    var extension = Path.GetExtension(peliculaDto.Poster.FileName);
                    pelicula.Poster = await almacenadorArchivos.GuardarArchivo(contenido, extension, contenedor, peliculaDto.Poster.ContentType);
                }
            }
            AsignarOrdenActores(pelicula);
            context.Add(pelicula);
            await context.SaveChangesAsync();
            var dto = mapper.Map<PeliculaDto>(pelicula);
            return new CreatedAtRouteResult("obtenerPelicula", new { id = pelicula.Id }, dto);
        }
        private void AsignarOrdenActores(Pelicula pelicula)
        {
            if (pelicula.peliculasActores != null)
            {
                for(int i = 0; i< pelicula.peliculasActores.Count; i++)
                {
                    pelicula.peliculasActores[i].Orden = i;
                }
            }
        }
        [HttpPut]
        public async Task<ActionResult> Put(int id, [FromForm] PeliculaCreacionDto peliculaCreacionDto)
        {
            var peliculaDB = await context.Peliculas.Include(x => x.peliculasActores).Include(x => x.peliculasGeneros).FirstOrDefaultAsync(x => x.Id == id);
            if (peliculaDB == null) { return NotFound(); }
            peliculaDB = mapper.Map(peliculaCreacionDto, peliculaDB);
            if (peliculaCreacionDto.Poster != null)
            {
                using (var memoryStream = new MemoryStream())
                {
                    await peliculaCreacionDto.Poster.CopyToAsync(memoryStream);
                    var contenido = memoryStream.ToArray();
                    var extension = Path.GetExtension(peliculaCreacionDto.Poster.FileName);
                    peliculaDB.Poster = await almacenadorArchivos.EditarArchivo(contenido, extension, contenedor, peliculaDB.Poster, peliculaCreacionDto.Poster.ContentType);
                }
            }
            AsignarOrdenActores(peliculaDB);
            await context.SaveChangesAsync();
            return NoContent();
        }
        [HttpPatch("{id}")]
        public async Task<ActionResult> Patch(int id, [FromBody] JsonPatchDocument<PeliculaPatchDto> patchDocument)
        {
            return await Patch<Pelicula, PeliculaPatchDto>(id, patchDocument);
        }
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            return await Delete<Pelicula>(id);
        }
    }
}
