using back_end_peliculas.Filtros;
using back_end_peliculas.Repositorios;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace back_end_peliculas
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

            //Esto es un filtro a nivel de acci�n, al habilitar la cache de nuestra aplicaci�n,
            //permite aplicarlo a alguna acci�n para evitar que ese servicio sea consumido s�lo cuando el tiempo en cach� expire
            //en el servicio o acci�n se debe agregar la etiqueta [ResponseCache(Duration = 60)]
            services.AddResponseCaching();

            //Este es un fltro de autenticaci�n
            //en el servicio o acci�n se debe agregar la etiqueta [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
            //sin embargo se recomienda que sea a nivel del controlador [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer();


            //configuramos el servicio Transitorio de la interf�z IRepositorio, para habilitar la inyecci�n de dependencias a nivel de aplicaci�n
            //AddSingleton trabaja con la misma instancia de clase (a nivel de cliente, navegador, usuarios)
            //AddTransient trabaja con una instancia diferente cada vez con todos -> mas usado
            //AddScoped trabaja con la misma instancia dentro del mismo contexto HTTP
            services.AddTransient<IRepositorio, RepositorioEnMemoria>();

            services.AddTransient<MiFiltroDeAccion>();

            services.AddControllers(options => {
                options.Filters.Add(typeof(FiltroDeExcepcion)); //Filtro a general en todos los controladores sin importar la acci�n consumida
            });
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "back_end_peliculas", Version = "v1" });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILogger<Startup> logger)
        {

            //Este middleware interceptamos la peticion, no la detiene y captura en memoria las respuestas de las peticiones y las almacena en el servicio del Ilogger  
            app.Use(async (context, next) =>
            {
                using (var swapStream = new MemoryStream()) {

                    var respuestaOriginal = context.Response.Body;
                    context.Response.Body = swapStream;

                    await next.Invoke();//se le indica que contin�e con la ejecuci�n del pipeline

                    swapStream.Seek(0, SeekOrigin.Begin);
                    string respuesta = new StreamReader(swapStream).ReadToEnd();
                    swapStream.Seek(0, SeekOrigin.Begin);

                    await swapStream.CopyToAsync(respuestaOriginal);
                    context.Response.Body = respuestaOriginal;

                    logger.LogInformation(respuesta);
                };
            });

            //Este es un tipo de middleware que intercepta la peticion, pero la detiene al intentar acceder a esa url creada
            app.Map("/mapa1", (app) =>
            {
                //Este es un tipo de middleware tambiente dediene la petici�n 
                app.Run(async context =>
                {
                    await context.Response.WriteAsync("Estoy interceptando el pipeline");
                });
            });
            

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "back_end_peliculas v1"));
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseResponseCaching();

            app.UseAuthentication();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
