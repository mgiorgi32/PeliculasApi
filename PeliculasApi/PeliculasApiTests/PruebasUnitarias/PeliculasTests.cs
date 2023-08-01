﻿using Azure.Storage.Blobs.Models;
using Castle.Core.Logging;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using PeliculasApi.Controllers;
using PeliculasApi.DTOS;
using PeliculasApi.Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Text;
using System.Threading.Tasks;

namespace PeliculasApiTests.PruebasUnitarias
{
    [TestClass]
    public class PeliculasTests : BasePruebas
    {
        private string CrearDataPrueba()
        {
            var databaseName = Guid.NewGuid().ToString();
            var context = ConstruirContext(databaseName);
            var genero = new Genero() { Nombre = "genre 1" };

            var peliculas = new List<Pelicula>()
            {
                new Pelicula(){Titulo = "Película 1", FechaEstreno = new DateTime(2010, 1,1), EnCines = false},
                new Pelicula(){Titulo = "No estrenada", FechaEstreno = DateTime.Today.AddDays(1), EnCines = false},
                new Pelicula(){Titulo = "Película en Cines", FechaEstreno = DateTime.Today.AddDays(-1), EnCines = true}
            };

            var peliculaConGenero = new Pelicula()
            {
                Titulo = "Película con Género",
                FechaEstreno = new DateTime(2010, 1, 1),
                EnCines = false
            };
            peliculas.Add(peliculaConGenero);

            context.Add(genero);
            context.AddRange(peliculas);
            context.SaveChanges();

            var peliculaGenero = new PeliculasGeneros() { GeneroId = genero.Id, PeliculaId = peliculaConGenero.Id };
            context.Add(peliculaGenero);
            context.SaveChanges();

            return databaseName;
        }
        [TestMethod]
        public async Task FiltrarPorTitulo()
        {
            var nombreBD = CrearDataPrueba();
            var mapper = ConfigurarAutoMapper();
            var contexto = ConstruirContext(nombreBD);
            var controller = new PeliculasController(contexto, mapper, null, null);
            controller.ControllerContext.HttpContext = new DefaultHttpContext();
            var tituloPelicula = "Película 1";
            var filtroDTo = new FiltroPeliculasDto()
            {
                Titulo = tituloPelicula,
                CantidadRegistrosPorPagina = 10
            };
            var respuesta = await controller.Filtrar(filtroDTo);
            var peliculas = respuesta.Value;
            Assert.AreEqual(1, peliculas.Count);
            Assert.AreEqual(tituloPelicula, peliculas[0].Titulo);

        }
        [TestMethod]
        public async Task FiltrarEnCines()
        {
            var nombreBD = CrearDataPrueba();
            var mapper = ConfigurarAutoMapper();
            var contexto = ConstruirContext(nombreBD);
            var controller = new PeliculasController(contexto, mapper, null, null);
            controller.ControllerContext.HttpContext = new DefaultHttpContext();
            var filtroDTO = new FiltroPeliculasDto() { EnCines = true};
            var respuesta = await controller.Filtrar(filtroDTO);
            var peliculas = respuesta.Value;
            Assert.AreEqual(1, peliculas.Count);
            Assert.AreEqual("Película en Cines", peliculas[0].Titulo);

        }
        [TestMethod]
        public async Task FiltrarProximosEstrenos()
        {
            var nombreBD = CrearDataPrueba();
            var mapper = ConfigurarAutoMapper();
            var contexto = ConstruirContext(nombreBD);
            var controller = new PeliculasController(contexto, mapper, null, null);
            controller.ControllerContext.HttpContext = new DefaultHttpContext();
            var filtroDTO = new FiltroPeliculasDto() { ProximoEstreno = true };
            var respuesta = await controller.Filtrar(filtroDTO);
            var peliculas = respuesta.Value;
            Assert.AreEqual(1, peliculas.Count);
            Assert.AreEqual("No estrenada", peliculas[0].Titulo);
        }

        [TestMethod]
        public async Task FiltrarPorGenero()
        {
            var nombreBD = CrearDataPrueba();
            var mapper = ConfigurarAutoMapper();
            var contexto = ConstruirContext(nombreBD);
            var controller = new PeliculasController(contexto, mapper, null, null);
            controller.ControllerContext.HttpContext = new DefaultHttpContext();
            var generoId = contexto.Generos.Select(x => x.Id).First();
            var filtroDto = new FiltroPeliculasDto()
            {
                GeneroId = generoId
            };
            var respuesta = await controller.Filtrar(filtroDto);
            var peliculas = respuesta.Value;
            Assert.AreEqual(1, peliculas.Count);
            Assert.AreEqual("Película con Género", peliculas[0].Titulo);
        }
        [TestMethod]
        public async Task FiltrarOrdenaTituloAscendente()
        {
            var nombreBD = CrearDataPrueba();
            var mapper = ConfigurarAutoMapper();
            var contexto = ConstruirContext(nombreBD);
            var controller = new PeliculasController(contexto, mapper, null, null);
            controller.ControllerContext.HttpContext = new DefaultHttpContext();
            var filtroDTO = new FiltroPeliculasDto() { CampoOrdenar = "titulo", OrdenAsendente = true };
            var respuesta = await controller.Filtrar(filtroDTO);
            var peliculas = respuesta.Value;
            var contexto2 = ConstruirContext(nombreBD);
            var peliculasDB = contexto2.Peliculas.OrderBy(x => x.Titulo).ToList();
            Assert.AreEqual(peliculasDB.Count, peliculas.Count);
            for (int i = 0; i < peliculasDB.Count; i++)
            {
                var peliculaDelControlador = peliculas[i];
                var peliculaDB = peliculasDB[i];
                Assert.AreEqual(peliculaDB.Id, peliculaDelControlador.Id);
            }


        }
        [TestMethod]
        public async Task FiltrarOrdenaTituloDescendente()
        {
            var nombreBD = CrearDataPrueba();
            var mapper = ConfigurarAutoMapper();
            var contexto = ConstruirContext(nombreBD);
            var controller = new PeliculasController(contexto, mapper, null, null);
            controller.ControllerContext.HttpContext = new DefaultHttpContext();
            var filtroDTO = new FiltroPeliculasDto() { CampoOrdenar = "titulo", OrdenAsendente = false };
            var respuesta = await controller.Filtrar(filtroDTO);
            var peliculas = respuesta.Value;
            var contexto2 = ConstruirContext(nombreBD);
            var peliculasDB = contexto2.Peliculas.OrderByDescending(x => x.Titulo).ToList();
            Assert.AreEqual(peliculasDB.Count, peliculas.Count);
            for (int i = 0; i < peliculasDB.Count; i++)
            {
                var peliculaDelControlador = peliculas[i];
                var peliculaDB = peliculasDB[i];
                Assert.AreEqual(peliculaDB.Id, peliculaDelControlador.Id);
            }

        }
        [TestMethod]
        public async Task FiltrarPorCampoIncorrectoDevuelvePelicula()
        {
            var nombreBD = CrearDataPrueba();
            var mapper = ConfigurarAutoMapper();
            var contexto = ConstruirContext(nombreBD);
            
            var mock = new Mock<ILogger<PeliculasController>>();

            
            var controller = new PeliculasController(contexto, mapper, null, mock.Object);
            controller.ControllerContext.HttpContext = new DefaultHttpContext();
            var filtroDto = new FiltroPeliculasDto()
            {
                CampoOrdenar = "abc",
                OrdenAsendente = true
            };
            var respuesta = await controller.Filtrar(filtroDto);
            var peliculas = respuesta.Value;
            var contexto2 = ConstruirContext(nombreBD);
            var peliculasDB = contexto2.Peliculas.ToList();
            Assert.AreEqual(peliculasDB.Count, peliculas.Count);
            Assert.AreEqual(1, mock.Invocations.Count);
        }
    }
}