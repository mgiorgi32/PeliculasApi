using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PeliculasApi.DTOS;
using PeliculasApi.Entidades;

namespace PeliculasApi.Controllers
{
    [ApiController]
    [Route("api/generos")]
    public class GenerosController:CustomBaseController
    {
        public  GenerosController(ApplicationDBContext context, IMapper mapper): base(context, mapper)
        {
        }
        [HttpGet]
        public async Task<ActionResult<List<GeneroDto>>> Get()
        {
            return await Get<Genero, GeneroDto>();
        }
        [HttpGet("{id:int}")]
        public async Task<ActionResult<GeneroDto>> Get(int id)
        {
            return await Get<Genero, GeneroDto>(id);
        }

        [HttpPost]
        public async Task<ActionResult> post([FromBody] GeneroCreacionDto creacionDto)
        {
            return await Post<GeneroCreacionDto, Genero, GeneroDto>(creacionDto, "obtenerGenero");
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> Put(int id, [FromBody] GeneroCreacionDto creacionDto)
        {
            return await Put<GeneroCreacionDto, Genero>(id, creacionDto);
        }

        [HttpDelete("{id}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult> Delete(int id)
        {
           return await Delete<Genero>(id);
        }
    }
}
