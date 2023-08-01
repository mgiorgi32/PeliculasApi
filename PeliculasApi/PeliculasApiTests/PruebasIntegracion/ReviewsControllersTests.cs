using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using PeliculasApi.DTOS;
using PeliculasApi.Entidades;

namespace PeliculasApiTests.PruebasIntegracion
{
    [TestClass]
    public class ReviewsControllersTests:BasePruebas
    {
        private static readonly string url = "api/pelicula/1/review";
        [TestMethod]
        public async Task ObtenerReviewsDevuelve404PeliculaInexistente()
        {
            var nombreBD = Guid.NewGuid().ToString();
            var factory = ConstruirWebApplicationFactory(nombreBD);
            var cliente = factory.CreateClient();
            var respuesta = await cliente.GetAsync(url);
            Assert.AreEqual(404, (int)respuesta.StatusCode);
        }
        [TestMethod]
        public async Task ObtenerReviewsDevuelveListadoVacio()
        {
            var nombreBD = Guid.NewGuid().ToString();
            var factory = ConstruirWebApplicationFactory(nombreBD);
            var context = ConstruirContext(nombreBD);
            context.Peliculas.Add(new Pelicula() { Titulo = "Película 1" });
            await context.SaveChangesAsync();
            var cliente = factory.CreateClient();
            var respuesta = await cliente.GetAsync(url);
            respuesta.EnsureSuccessStatusCode();
            var reviews = JsonConvert.DeserializeObject<List<ReviewDto>>(await respuesta.Content.ReadAsStringAsync());
            Assert.AreEqual(0, reviews.Count);
        }
    }
}
