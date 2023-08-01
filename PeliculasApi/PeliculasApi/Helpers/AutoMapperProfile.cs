using AutoMapper;
using Microsoft.AspNetCore.Identity;
using NetTopologySuite;
using NetTopologySuite.Geometries;
using PeliculasApi.DTOS;
using PeliculasApi.Entidades;

namespace PeliculasApi.Helpers
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles(GeometryFactory geometryFactory)
        {
            CreateMap<Genero, GeneroDto>().ReverseMap();
            CreateMap<GeneroCreacionDto, Genero>();
            CreateMap<Review, ReviewDto>().ForMember(x => x.NombreUsuario, x=> x.MapFrom(y => y.Usuario.UserName));
            CreateMap<ReviewCreacionDto, Review>();
            CreateMap<ReviewDto, Review>();

            CreateMap<SalaDeCine, SalaDeCineDto>().ForMember(x => x.Latitud, x => x.MapFrom(y => y.Ubicacion.Y)).ForMember(x => x.Longitud, x => x.MapFrom(y => y.Ubicacion.X));
            CreateMap<SalaDeCineDto, SalaDeCine>().ForMember(x => x.Ubicacion, x => x.MapFrom(y => geometryFactory.CreatePoint(new Coordinate(y.Longitud, y.Latitud))));
            CreateMap<SalaDeCine, SalaDeCineCreacionDto>().ForMember(x => x.Ubicacion, x => x.MapFrom(y => geometryFactory.CreatePoint(new Coordinate(y.Longitud, y.Latitud))));
            CreateMap<Actor, ActorDto>().ReverseMap();
            CreateMap<ActorCreacionDto, Actor>().ForMember(x => x.Foto, options => options.Ignore());
            CreateMap<ActorPatchDto, Actor>().ReverseMap();
            CreateMap<Pelicula, PeliculaDto>().ReverseMap();
            CreateMap<PeliculaCreacionDto, Pelicula>().ForMember(x => x.Poster, options => options.Ignore())
                .ForMember(x => x.peliculasGeneros, options => options.MapFrom(MapPeliculasGeneros))
                .ForMember(x => x.peliculasActores, options => options.MapFrom(MapPeliculasActores));
            CreateMap<Pelicula, PeliculaDetallesDto>().ForMember(x => x.Generos, options => options.MapFrom(MapPeliculaGeneros)).ForMember(x => x.Actores, options => options .MapFrom(MapPeliculaActores));
            CreateMap<PeliculaPatchDto, Pelicula>().ReverseMap();
            CreateMap<IdentityUser, UsuarioDTO>();
        }
        private List<ActorPeliculaDetalleDto> MapPeliculaActores(Pelicula pelicula, PeliculaDetallesDto detallesDto)
        {
            var resultado = new List<ActorPeliculaDetalleDto>();
            if (pelicula.peliculasActores == null) { return resultado; }
            foreach (var actorPelicula in pelicula.peliculasActores)
            {
                resultado.Add(new ActorPeliculaDetalleDto { ActorId = actorPelicula.ActorId, Personaje = actorPelicula.Personaje, NombrePersona= actorPelicula.actor.Nombre});
            }
            return resultado;
        }
            private List<GeneroDto> MapPeliculaGeneros(Pelicula pelicula, PeliculaDetallesDto detallesDto)
        {
            var resultado = new List<GeneroDto>();
            if(pelicula.peliculasGeneros == null) { return resultado; }
            foreach (var generoPelicula in pelicula.peliculasGeneros)
            {
                resultado.Add(new GeneroDto() { Id = generoPelicula.GeneroId, Nombre = generoPelicula.genero.Nombre });
            }
            return resultado;
        }
        private List<PeliculasGeneros> MapPeliculasGeneros(PeliculaCreacionDto peliculaCreacionDto, Pelicula pelicula)
        {
            var resultado = new List<PeliculasGeneros>();
            if(peliculaCreacionDto.GenerosIds == null) { return resultado; }
            foreach(var id in peliculaCreacionDto.GenerosIds)
            {
                resultado.Add(new PeliculasGeneros() { GeneroId = id });
            }
            return resultado;
        }
        private List<PeliculaActor> MapPeliculasActores(PeliculaCreacionDto peliculaCreacionDto, Pelicula pelicula)
        {
            var resultado = new List<PeliculaActor>();
            if (peliculaCreacionDto.Actores == null) { return resultado; }
            foreach (var actores in peliculaCreacionDto.Actores)
            {
                resultado.Add(new PeliculaActor() { ActorId = actores.ActorId });
            }
            return resultado;
        }
    }
}
