using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite;
using PeliculasApi;
using PeliculasApi.Helpers;
using PeliculasAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace PeliculasApiTests
{
    public class BasePruebas
    {
        protected string usuarioPorDefectoEmail = "ejemplo@hotmail.com";
        protected string usuarioPorDefectoId = "a8cee743-dbf5-46c6-9d6c-b27cbe56787b";
        protected ApplicationDBContext ConstruirContext(string nombreDB)
        {
            var opciones = new DbContextOptionsBuilder<ApplicationDBContext>().UseInMemoryDatabase(nombreDB).Options;
            var dbContext = new ApplicationDBContext(opciones);
            return dbContext;
        }
        protected IMapper ConfigurarAutoMapper()
        {
            var config = new MapperConfiguration(options =>
            {
                var geometryFactory = NtsGeometryServices.Instance.CreateGeometryFactory(srid: 4326);
                options.AddProfile(new AutoMapperProfiles(geometryFactory));
            });
            return config.CreateMapper();
        }
        protected ControllerContext ConstruirControllerContext()
        {
            var usuario = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.Name, usuarioPorDefectoEmail),
                new Claim(ClaimTypes.Email, usuarioPorDefectoEmail),
                new Claim(ClaimTypes.NameIdentifier, usuarioPorDefectoId)
            }));
            return new ControllerContext()
            {
                HttpContext = new DefaultHttpContext() { User = usuario }
            };
        }
        protected WebApplicationFactory<Startup> ConstruirWebApplicationFactory(string nombreDB, bool ignorarSeguridad = true)
        {
            var factory = new WebApplicationFactory<Startup>();
            factory = factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureTestServices(services =>
                {
                    var descriptorDBContext = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<ApplicationDBContext>));
                    if (descriptorDBContext!= null)
                    {
                        services.Remove(descriptorDBContext);
                    }
                    services.AddDbContext<ApplicationDBContext>(options => options.UseInMemoryDatabase(nombreDB));
                    if (ignorarSeguridad)
                    {
                        services.AddControllers(options =>
                        {
                            options.Filters.Add(new UsuarioFiltroFalso());
                        });
                        services.AddSingleton<IAuthorizationHandler, AllowAnonymousHandler>();
                    }
                });
            });
            return factory;
        }
    }
}
