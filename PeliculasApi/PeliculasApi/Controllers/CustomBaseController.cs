using AutoMapper;
using Azure;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PeliculasApi.DTOS;
using PeliculasApi.Entidades;
using PeliculasApi.Helpers;

namespace PeliculasApi.Controllers
{
    public class CustomBaseController : ControllerBase
    {
        private readonly ApplicationDBContext context;
        private readonly IMapper mapper;

        public CustomBaseController(ApplicationDBContext context, IMapper mapper)
        {
            this.context = context;
            this.mapper = mapper;
        }

        protected async Task<List<TDTO>> Get<TEntidad, TDTO>() where TEntidad : class
        {
            var entidades = await context.Set<TEntidad>().AsNoTracking().ToListAsync();
            var dtos = mapper.Map<List<TDTO>>(entidades);
            return dtos;

        }
        protected async Task<List<TDTO>> Get<TEntidad, TDTO>(PaginacionDto paginacion) where TEntidad: class
        {
            var queryable = context.Set<TEntidad>().AsQueryable();
            return await Get<TEntidad, TDTO>(paginacion, queryable);
        }
        protected async Task<List<TDTO>> Get<TEntidad, TDTO>(PaginacionDto paginacion, IQueryable<TEntidad> queryable) where TEntidad : class
        {
            await HttpContext.InsertarParametrosPaginacion(queryable, paginacion.CanridadRegistrosPorPagina);
            var entidades = await queryable.Paginar(paginacion).ToListAsync();
            return mapper.Map<List<TDTO>>(entidades);
        }
        protected async Task<ActionResult<TDTO>> Get<TEntidad, TDTO>(int id) where TEntidad : class, IId
        {
            var entidad = await context.Set<TEntidad>().AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
            if(entidad == null)
            {
                return NotFound();
            }
            return mapper.Map<TDTO>(entidad);
        }
        protected async Task<ActionResult>Post<TCreacion, TEntidad, TLectura>(TCreacion creacion, string nombreRuta) where TEntidad: class, IId
        {
            var entidad = mapper.Map<TEntidad>(creacion);
            context.Add(entidad);
            await context.SaveChangesAsync();
            var DTO = mapper.Map<TEntidad>(entidad);
            return new CreatedAtRouteResult(nombreRuta, new { id = entidad.Id }, DTO);
        }
        protected async Task<ActionResult> Put<TCreacion, TEntidad> (int id, TCreacion creacion) where TEntidad: class, IId
        {
            var entidad = mapper.Map<TEntidad>(creacion);
            entidad.Id = id;
            context.Entry(entidad).State = EntityState.Modified;
            await context.SaveChangesAsync();
            return NoContent();
        }
        protected async Task<ActionResult> Patch<TEntidad, TDTO>(int id, JsonPatchDocument<TDTO> patchDocument) where TDTO: class where TEntidad : class, IId
        {
            if (patchDocument == null) { return BadRequest(); }
            var entidadDB = await context.Set<TEntidad>().FirstOrDefaultAsync(x => x.Id == id);
            if (entidadDB == null)
            {
                return NotFound();
            }
            var entidadDto = mapper.Map<TDTO>(entidadDB);
            patchDocument.ApplyTo(entidadDto, ModelState);
            var esValido = TryValidateModel(entidadDto);
            if (!esValido)
            {
                return BadRequest(ModelState);
            }
            mapper.Map(entidadDto, entidadDB);
            await context.SaveChangesAsync();
            return NoContent();
        }
        protected async Task<ActionResult> Delete<TEntidad>(int id) where TEntidad : class, IId, new()
        {
            var existe = await context.Set<TEntidad>().AnyAsync(x => x.Id == id);
            if (!existe)
            {
                return NotFound();
            }
            context.Remove(new TEntidad() { Id = id });
            await context.SaveChangesAsync();
            return NoContent();
        }
    }
}
