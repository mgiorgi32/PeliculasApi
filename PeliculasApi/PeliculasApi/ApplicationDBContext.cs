﻿using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite;
using NetTopologySuite.Geometries;
using PeliculasApi.Entidades;
using System.Security.Claims;

namespace PeliculasApi
{
    public class ApplicationDBContext: IdentityDbContext
    {
        public ApplicationDBContext(DbContextOptions options): base(options)
        {

        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<PeliculaActor>().HasKey(x => new { x.ActorId, x.PeliculaId });
            modelBuilder.Entity<PeliculasGeneros>().HasKey(x => new { x.GeneroId, x.PeliculaId });
            modelBuilder.Entity<PeliculasSalasDeCine>().HasKey(x => new { x.PeliculaId, x.SalaDeCineId });
            base.OnModelCreating(modelBuilder);
        }
       private void SeedData(ModelBuilder modelBuilder)
        {
            var rolAdminId = "6ce7cb13-d24c-436c-9f44-3394c4ae6316";
            var usuarioAdminId = "ceb14232-d510-44c7-a7fa-1cc9ba69a185";
            var rolAdmin = new IdentityRole()
            {
                Id = rolAdminId,
                Name = "Admin",
                NormalizedName = "Admin"
            };
            var passwordHasher = new PasswordHasher<IdentityUser>();
            var username = "marcosgiorgi01@gmail.com";
            var usuarioAdmin = new IdentityUser()
            {
                Id = usuarioAdminId,
                UserName = username,
                NormalizedUserName = username,
                Email = username,
                NormalizedEmail = username,
                PasswordHash = passwordHasher.HashPassword(null, "aa23671A")
            };
            modelBuilder.Entity<IdentityUser>().HasData(usuarioAdmin);
            modelBuilder.Entity<IdentityRole>().HasData(new IdentityUserClaim<string>()
            {
                Id = 1,
                ClaimType = ClaimTypes.Role,
                UserId = usuarioAdminId,
                ClaimValue = "Admin"
            });
            var geometryFactory = NtsGeometryServices.Instance.CreateGeometryFactory(srid: 4326);
            modelBuilder.Entity<SalaDeCine>()
               .HasData(new List<SalaDeCine>
               {
                    //new SalaDeCine{Id = 1, Nombre = "Nine", Ubicacion = geometryFactory.CreatePoint(new Coordinate(-58.793000, -34.635250))},
                    new SalaDeCine{Id = 4, Nombre = "LujanWalk", Ubicacion = geometryFactory.CreatePoint(new Coordinate(-58.793023, -34.546253))},
                    new SalaDeCine{Id = 5, Nombre = "Norcenter", Ubicacion = geometryFactory.CreatePoint(new Coordinate(-58.523137, -34.515128))},
                    new SalaDeCine{Id = 6, Nombre = "Village East Cinema", Ubicacion = geometryFactory.CreatePoint(new Coordinate(-73.986227, 40.730898))}
               });

            var aventura = new Genero() { Id = 4, Nombre = "Aventura" };
            var animation = new Genero() { Id = 5, Nombre = "Animación" };
            var suspenso = new Genero() { Id = 6, Nombre = "Suspenso" };
            var romance = new Genero() { Id = 7, Nombre = "Romance" };

            modelBuilder.Entity<Genero>()
                .HasData(new List<Genero>
                {
                    aventura, animation, suspenso, romance
                });

            var jimCarrey = new Actor() { Id = 5, Nombre = "Jim Carrey", FechaNacimiento = new DateTime(1962, 01, 17) };
            var robertDowney = new Actor() { Id = 6, Nombre = "Robert Downey Jr.", FechaNacimiento = new DateTime(1965, 4, 4) };
            var chrisEvans = new Actor() { Id = 7, Nombre = "Chris Evans", FechaNacimiento = new DateTime(1981, 06, 13) };

            modelBuilder.Entity<Actor>()
                .HasData(new List<Actor>
                {
                    jimCarrey, robertDowney, chrisEvans
                });

            var endgame = new Pelicula()
            {
                Id = 2,
                Titulo = "Avengers: Endgame",
                EnCines = true,
                FechaEstreno = new DateTime(2019, 04, 26)
            };

            var iw = new Pelicula()
            {
                Id = 3,
                Titulo = "Avengers: Infinity Wars",
                EnCines = false,
                FechaEstreno = new DateTime(2019, 04, 26)
            };

            var sonic = new Pelicula()
            {
                Id = 4,
                Titulo = "Sonic the Hedgehog",
                EnCines = false,
                FechaEstreno = new DateTime(2020, 02, 28)
            };
            var emma = new Pelicula()
            {
                Id = 5,
                Titulo = "Emma",
                EnCines = false,
                FechaEstreno = new DateTime(2020, 02, 21)
            };
            var wonderwoman = new Pelicula()
            {
                Id = 6,
                Titulo = "Wonder Woman 1984",
                EnCines = false,
                FechaEstreno = new DateTime(2020, 08, 14)
            };

            modelBuilder.Entity<Pelicula>()
                .HasData(new List<Pelicula>
                {
                    endgame, iw, sonic, emma, wonderwoman
                });

            modelBuilder.Entity<PeliculasGeneros>().HasData(
                new List<PeliculasGeneros>()
                {
                    new PeliculasGeneros(){PeliculaId = endgame.Id, GeneroId = suspenso.Id},
                    new PeliculasGeneros(){PeliculaId = endgame.Id, GeneroId = aventura.Id},
                    new PeliculasGeneros(){PeliculaId = iw.Id, GeneroId = suspenso.Id},
                    new PeliculasGeneros(){PeliculaId = iw.Id, GeneroId = aventura.Id},
                    new PeliculasGeneros(){PeliculaId = sonic.Id, GeneroId = aventura.Id},
                    new PeliculasGeneros(){PeliculaId = emma.Id, GeneroId = suspenso.Id},
                    new PeliculasGeneros(){PeliculaId = emma.Id, GeneroId = romance.Id},
                    new PeliculasGeneros(){PeliculaId = wonderwoman.Id, GeneroId = suspenso.Id},
                    new PeliculasGeneros(){PeliculaId = wonderwoman.Id, GeneroId = aventura.Id},
                });

            modelBuilder.Entity<PeliculaActor>().HasData(
                new List<PeliculaActor>()
                {
                    new PeliculaActor(){PeliculaId = endgame.Id, ActorId = robertDowney.Id, Personaje = "Tony Stark", Orden = 1},
                    new PeliculaActor(){PeliculaId = endgame.Id, ActorId = chrisEvans.Id, Personaje = "Steve Rogers", Orden = 2},
                    new PeliculaActor(){PeliculaId = iw.Id, ActorId = robertDowney.Id, Personaje = "Tony Stark", Orden = 1},
                    new PeliculaActor(){PeliculaId = iw.Id, ActorId = chrisEvans.Id, Personaje = "Steve Rogers", Orden = 2},
                    new PeliculaActor(){PeliculaId = sonic.Id, ActorId = jimCarrey.Id, Personaje = "Dr. Ivo Robotnik", Orden = 1}
                });
        }

        public DbSet<Genero> Generos { get; set; }
        public DbSet<Actor> Actores { get; set; }
        public DbSet<Pelicula> Peliculas { get; set;}
        public DbSet<PeliculaActor> PeliculasActores { get; set;}
        public DbSet<PeliculasGeneros> PeliculasGeneros { get; set;}
        public DbSet<SalaDeCine> SalaDeCines { get; set; }
        public DbSet<PeliculasSalasDeCine> peliculasSalasDeCines { get; set; }
        public DbSet<Review> Reviews { get; set; }  
    }

}
