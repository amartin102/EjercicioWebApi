using back_end_peliculas.Entidades;
using back_end_peliculas.Filtros;
using back_end_peliculas.Repositorios;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace back_end_peliculas.Controllers
{
    [Route("api/generos")]
    [ApiController]
    //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class GenerosController: ControllerBase
    {
        private readonly IRepositorio _repositorio;
        private readonly ILogger<GenerosController> logger;

        //ILogger es una interfaz que permite controlar mejor el log en cuanto a mensajes de error y por categorias (warning, debug, error, critical, information, trace)
        public GenerosController(IRepositorio repositorio, ILogger<GenerosController> logger)
        {
            _repositorio = repositorio;
            this.logger = logger;
        }

        [HttpGet]
        [HttpGet("listado")]
        [HttpGet("/listadogeneros")]
        //[ResponseCache(Duration = 60)]        
        [ServiceFilter(typeof(MiFiltroDeAccion))]//permite inicializar un servicio
        public List<Genero> Get() {

            logger.LogInformation("vamos a mostrar los generos");
            return _repositorio.ObtenerGeneros();
        }

        [HttpGet("guid")] //api/generos/guid
        public ActionResult<Guid> GetGuid() {
            return _repositorio.ObtenerGuid();
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<Genero>> Get(int id, [FromHeader] string nombre)
        {            
            var genero = await _repositorio.ObtenerId(id);

            logger.LogDebug($"Obteniendo el género por su id: {id}");
            if (genero == null)
            {
                throw new ApplicationException($"El género con id: {id} no fue encontrado");
                logger.LogWarning($"No se logró encontrar el género con id: {id}");
                return NotFound();
            }
                

            return genero;
        }

        [HttpPost]
        public ActionResult Post([FromBody] Genero genero) 
        {
            _repositorio.CrearGenero(genero);
            return NoContent();
        }



        [HttpPut]
        public ActionResult Put([FromBody] Genero genero)
        {
           
            return NoContent();
        }

        [HttpDelete]
        public ActionResult Delete()
        {
            return NoContent();
        }

    }
}
