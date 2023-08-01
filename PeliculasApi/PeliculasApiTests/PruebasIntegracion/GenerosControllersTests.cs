using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using PeliculasApi.DTOS;
using PeliculasApi.Entidades;

namespace PeliculasApiTests.PruebasIntegracion
{
    [TestClass]
    public class GenerosControllersTests: BasePruebas
    {
        private static readonly string url = "/api/generos";
        [TestMethod]
        public async Task ObtenerTodosLosGenerosListadoVacio()
        {
            var nombreDB = Guid.NewGuid().ToString();
            var factory = ConstruirWebApplicationFactory(nombreDB);
            var cliente = factory.CreateClient();
            var respuesta = await cliente.GetAsync(url);
            respuesta.EnsureSuccessStatusCode();
            var generos = JsonConvert.DeserializeObject<List<GeneroDto>>(await respuesta.Content.ReadAsStringAsync());
            Assert.AreEqual(0, generos.Count);
        }
        [TestMethod]
        public async Task ObtenerTodosLosGeneros()
        {
            var nombreDB = Guid.NewGuid().ToString();
            var factory = ConstruirWebApplicationFactory(nombreDB);
            var context = ConstruirContext(nombreDB);
            context.Generos.Add(new Genero() { Nombre = "Genero 1" });
            context.Generos.Add(new Genero() { Nombre = "Genero 2" });
            await context.SaveChangesAsync();
            var cliente = factory.CreateClient();
            
            var respuesta = await cliente.GetAsync(url);
            respuesta.EnsureSuccessStatusCode();
            var generos = JsonConvert.DeserializeObject<List<GeneroDto>>(await respuesta.Content.ReadAsStringAsync());
            Assert.AreEqual(2, generos.Count);
        }
        [TestMethod]
        public async Task BorrarGenero()
        {
            var nombreBD = Guid.NewGuid().ToString();
            var factory = ConstruirWebApplicationFactory(nombreBD);
            var context = ConstruirContext(nombreBD);
            context.Generos.Add(new Genero() { Nombre = "Genero 1" });
            await context.SaveChangesAsync();
            var cliente = factory.CreateClient();
            var respuesta = await cliente.DeleteAsync($"{url}/1");
            respuesta.EnsureSuccessStatusCode();
            var contexto2 = ConstruirContext(nombreBD);
            var existe = await contexto2.Generos.AnyAsync();
            Assert.IsFalse(existe);
        }
        [TestMethod]
        public async Task BorrarGeneroRetorna401()
        {
            var nombreBD = Guid.NewGuid().ToString();
            var factory = ConstruirWebApplicationFactory(nombreBD, ignorarSeguridad: false);
            
            var cliente = factory.CreateClient();
            var respuesta = await cliente.DeleteAsync($"{url}/1");
            Assert.AreEqual("Unauthorized", respuesta.ReasonPhrase);
        }
    }
}
