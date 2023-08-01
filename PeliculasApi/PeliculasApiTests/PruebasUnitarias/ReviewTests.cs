using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PeliculasApi.Controllers;
using PeliculasApi.DTOS;
using PeliculasApi.Entidades;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Text;
using System.Threading.Tasks;

namespace PeliculasApiTests.PruebasUnitarias
{
    [TestClass]
    public class ReviewTests: BasePruebas
    {
        [TestMethod]
        public async Task UsuarioNoPuedeCrearDosReviewsParaLaMismaPelicula()
        {
            var nombreBD = Guid.NewGuid().ToString();
            var context = ConstruirContext(nombreBD);
            CrearPeliculas(nombreBD);
            var peliculaId = context.Peliculas.Select(x => x.Id).First();
            var review1 = new Review()
            {
                PeliculaId = peliculaId,
                UsuarioId = usuarioPorDefectoId,
                Puntacion = 5
            };
            context.Add(review1);
            await context.SaveChangesAsync();
            var context2 = ConstruirContext(nombreBD);
            var mapper = ConfigurarAutoMapper();
            var controller = new ReviewController(context2, mapper);
            controller.ControllerContext = ConstruirControllerContext();
            var reviewCreacionDto = new ReviewCreacionDto { Puntuacion = 5 };
            var respuesta = await controller.Post(peliculaId, reviewCreacionDto);
            var valor = respuesta as IStatusCodeActionResult;
            Assert.AreEqual(400, valor.StatusCode.Value);
        }
        [TestMethod]
        public async Task CrearReview()
        {
            var nombreBD = Guid.NewGuid().ToString();
            var context = ConstruirContext(nombreBD);
            CrearPeliculas(nombreBD);
            var peliculaId = context.Peliculas.Select(x => x.Id).First();
            var context2 = ConstruirContext(nombreBD);
            var mapper = ConfigurarAutoMapper();
            var controller = new ReviewController(context2, mapper);
            controller.ControllerContext = ConstruirControllerContext();
            var reviewCreacionDto = new ReviewCreacionDto() { Puntuacion = 5 };
            var respuesta = await controller.Post(peliculaId, reviewCreacionDto);
            var valor = respuesta as NoContentResult;
            Assert.IsNotNull(valor);
            var context3 = ConstruirContext(nombreBD);
            var reviewDB = context3.Reviews.First();
            Assert.AreEqual(usuarioPorDefectoId, reviewDB.UsuarioId);

        }
        private void CrearPeliculas(string nombreDB)
        {
            var contexto = ConstruirContext(nombreDB);

            contexto.Peliculas.Add(new Pelicula() { Titulo = "pelicula 1" });

            contexto.SaveChanges();
        }
    }
}
