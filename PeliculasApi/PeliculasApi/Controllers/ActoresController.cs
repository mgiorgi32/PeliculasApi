using AutoMapper;
using Azure;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PeliculasApi.DTOS;
using PeliculasApi.Entidades;
using PeliculasApi.Helpers;
using PeliculasApi.Servicios;

namespace PeliculasApi.Controllers
{
    [ApiController]
    [Route("api/actores")]
    public class ActoresController : CustomBaseController
    {
        private readonly ApplicationDBContext context;
        private readonly IMapper mapper;
        private readonly IAlmacenadorArchivos almacenadorArchivos;
        private readonly string contenedor = "actores";

        public ActoresController(ApplicationDBContext context, IMapper mapper, IAlmacenadorArchivos almacenadorArchivos) : base(context, mapper)
        {
            this.context = context;
            this.mapper = mapper;
            this.almacenadorArchivos = almacenadorArchivos;
        }
        [HttpGet]
        public async Task<ActionResult<List<ActorDto>>> Get([FromQuery] PaginacionDto paginacionDto)
        {
            return await Get<Actor, ActorDto>(paginacionDto);
        }

        [HttpGet("{id}", Name = "obtenerActor")]
        public async Task<ActionResult<ActorDto>> Get(int id)
        {
            return await Get<Actor, ActorDto>(id);
        }

        [HttpPost]
        public async Task<ActionResult> Post([FromBody] ActorCreacionDto creacionDto)
        {
            var entidades = mapper.Map<Actor>(creacionDto);
            if (creacionDto.Foto != null)
            {
                using (var memoryStream = new MemoryStream())
                {
                    await creacionDto.Foto.CopyToAsync(memoryStream);
                    var contenido = memoryStream.ToArray();
                    var extension = Path.GetExtension(creacionDto.Foto.FileName);
                    entidades.Foto = await almacenadorArchivos.GuardarArchivo(contenido, extension, contenedor, creacionDto.Foto.ContentType);
                }
            }
            context.Add(entidades);
            await context.SaveChangesAsync();
            var dto = mapper.Map<ActorDto>(entidades);
            return new CreatedAtRouteResult("obtenerActor", new { id = entidades.Id }, dto);

        }

        [HttpPut("{id}")]
        public async Task<ActionResult> Put(int id, [FromForm] ActorCreacionDto creacionDto)
        {
            var actorDB = await context.Actores.FirstOrDefaultAsync(x => x.Id == id);
            if (actorDB == null) { return NotFound(); }
            actorDB = mapper.Map(creacionDto, actorDB);
            if (creacionDto.Foto != null)
            {
                using (var memoryStream = new MemoryStream())
                {
                    await creacionDto.Foto.CopyToAsync(memoryStream);
                    var contenido = memoryStream.ToArray();
                    var extension = Path.GetExtension(creacionDto.Foto.FileName);
                    actorDB.Foto = await almacenadorArchivos.EditarArchivo(contenido, extension, contenedor, actorDB.Foto, creacionDto.Foto.ContentType);
                }
            }
            await context.SaveChangesAsync();
            return NoContent();
        }
        [HttpPatch("{id}")]
        public async Task<ActionResult> Patch(int id, [FromBody] JsonPatchDocument<ActorPatchDto> patchDocument)
        {
            return await Patch<Actor, ActorPatchDto>(id, patchDocument);
        }
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            return await Delete<Actor>(id);
        }
    }
}
