using AutoMapper;
using PeliculasApi.Entidades;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;

namespace PeliculasApi.Helpers
{
    public class PeliculaExisteAttribute : Attribute, IAsyncResourceFilter
    {
        private readonly ApplicationDBContext Dbcontext;

        public PeliculaExisteAttribute(ApplicationDBContext Dbcontext)
        {
            this.Dbcontext = Dbcontext;

        }

        public async Task OnResourceExecutionAsync(ResourceExecutingContext context, ResourceExecutionDelegate next)
        {
            var peliculaIdObject = context.HttpContext.Request.RouteValues["peliculaId"];
            if(peliculaIdObject == null) { return; }
            var peliculaId = int.Parse(peliculaIdObject.ToString());
            var existePelicula = await Dbcontext.Peliculas.AnyAsync(x => x.Id == peliculaId);
            if (!existePelicula) 
            {
                context.Result = new NotFoundResult(); 
            }
            else
            {
                await next();
            }
        }
    }
}
