using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PeliculasApi.DTOS;
using PeliculasApi.Entidades;
using System.Security.Claims;
using PeliculasApi.Helpers;
using RouteAttribute = Microsoft.AspNetCore.Components.RouteAttribute;

namespace PeliculasApi.Controllers
{
    [Route("api/pelicula/{peliculaId:int}/review")]
    [ServiceFilter(typeof(PeliculaExisteAttribute))]
    public class ReviewController: CustomBaseController
    {
        private readonly ApplicationDBContext context;
        private readonly IMapper mapper;

        public ReviewController(ApplicationDBContext context, IMapper mapper): base(context, mapper)
        {
            this.context = context;
            this.mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<List<ReviewDto>>> Get(int peliculaId, [FromQuery] PaginacionDto paginacion)
        {
            

            var queryable = context.Reviews.Include(x => x.Usuario).AsQueryable();
            queryable = queryable.Where(x => x.PeliculaId == peliculaId);
            return await Get<Review, ReviewDto>(paginacion, queryable);
        }
        [HttpPost]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult> Post(int peliculaId, [FromBody] ReviewCreacionDto creacionDto)
        {
            
            var usuarioId = HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier).Value;
            var reviewExiste = await context.Reviews.AnyAsync(x => x.PeliculaId == peliculaId && x.UsuarioId == usuarioId);
            if (reviewExiste) { return BadRequest("El usuario ya le hizo una review a esta pelicula"); }
            var review = mapper.Map<Review>(creacionDto);
            review.PeliculaId = peliculaId;
            review.UsuarioId = usuarioId;
            context.Add(review);
            await context.SaveChangesAsync();
            return NoContent();
        }

        [HttpPut("{reviewId:int}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult> Put(int peliculaId, int reviewId, [FromBody] ReviewCreacionDto creacionDto)
        {
          
            var reviewDB = await context.Reviews.FirstOrDefaultAsync(x => x.Id == reviewId);
            if (reviewDB == null) { return NotFound(); }
            var usuarioId = HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier).Value;
            if (reviewDB.UsuarioId != usuarioId) { return BadRequest("No puede editar el review porque no tiene permisos"); }
            reviewDB = mapper.Map(creacionDto, reviewDB);
            await context.SaveChangesAsync();
            return NoContent();
        }
        [HttpDelete("{reviewId}")]
        public async Task<ActionResult> Delete(int reviewId)
        {
            var reviewDB = await context.Reviews.FirstOrDefaultAsync(x => x.Id == reviewId);
            if (reviewDB == null) { return NotFound(); }
            var usuarioId = HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier).Value;
            if (reviewDB.UsuarioId != usuarioId) { return Forbid(); }
            context.Remove(reviewDB);
            await context.SaveChangesAsync();
            return NoContent();
        }
    }
}
