using AutoMapper;
using GestionEquipo.DB.DATA;
using GestionEquipo.DB.DATA.ENTITY;
using GestionEquipo.Server.Repositorio;
using GestionEquipo.Shared.DTO;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using Microsoft.EntityFrameworkCore;  

namespace GestionEquipo.Server.Controllers
{
    [ApiController]
    [Route("api/Jugadores")]
    public class JugadoresControllers : ControllerBase
    {

        private readonly IJugadorRepositorio repositorio;
        private readonly IMapper mapper;
        private readonly Context context;
        //Inyectar la dependencia en el constructor del controller y generar su campo correspondiente.
        private readonly IOutputCacheStore outputCacheStore;
        //Definir un campo (atributo privado) la clave que identifica el Cache del controller.
        private const string cacheKey = "Jugadores";

        public JugadoresControllers(IJugadorRepositorio repositorio, IMapper mapper, Context context, IOutputCacheStore outputCacheStore)
        {
            this.repositorio = repositorio;
            this.mapper = mapper;
            this.context = context;
            this.outputCacheStore = outputCacheStore;
        }

        //GET: --------------------------------------------------------------

        [HttpGet]
        [OutputCache(Tags = [cacheKey])] //En el método que recibe la petición HTTP en el controller donde se desee manejar un Cache colocar la etiqueta
        public async Task<ActionResult<List<Jugador>>> Get()
        {
            return await repositorio.Select();
        }

        [HttpGet("{ID:int}")] 
        public async Task<ActionResult<Jugador>>Get(int ID)
        {
            var jugador = await repositorio.SelectById(ID);

            if (jugador == null)
            {
                return NotFound($"El jugador con el ID: {ID} no fue encontrado");
            }
            return jugador;
        }

        //[HttpGet("Existe/{ID:int}")]
        //public async Task<ActionResult<bool>> Existe(int ID)
        //{
        //    var existe = await repositorio.Existe(ID);
        //    return existe;
        //}

        [HttpGet("{Nombre}")] 
        public async Task<ActionResult<Jugador>> GetbyCod(string Nombre)
        {
            var jugador = await context.Jugadores.FirstOrDefaultAsync(x => x.Nombre==Nombre);

            if (jugador == null)
            {
                return NotFound($"El nombre {Nombre} no fue encontrado");
            }
            return jugador;
        }

        //POST: -------------------------------------------------------------

        [HttpPost]
        public async Task<ActionResult<int>> Post(Jugador entidad)
        {
            try
            {
                var id = await repositorio.Insert(entidad);
                if (id == 0)
                {
                    return BadRequest("No se pudo agregar el jugador");
                }
                //En los métodos donde se modifica el contenido del Cache deben limpiar su contenido
                //relación a la información que cambian.
                await outputCacheStore.EvictByTagAsync(cacheKey, default);
                return id;
                
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        ////DTO:
        //[HttpPost]
        //public async Task<ActionResult<int>> Post(CrearJugadorDTO entidadDTO)
        //{
        //    try
        //    {
        //        //Jugador entidad = new Jugador();
        //        //entidad.Nombre = entidadDTO.Nombre;
        //        //entidad.Edad = entidadDTO.Edad;
        //        //entidad.Posicion = entidadDTO.Posicion;

        //        Jugador entidad = mapper.Map<Jugador>(entidadDTO);

               
        //        return await repositorio.Insert(entidad);
        //    }
        //    catch (Exception err)
        //    {
        //        return BadRequest(err.InnerException.Message);
        //    }
        //}

        [HttpPost("Confirmacion")]
        public async Task<ActionResult<string>> ConfirmacionAgregarJugador(CrearJugadorDTO entidadDTO)
        {
            try
            {
                Jugador entidad = mapper.Map<Jugador>(entidadDTO);
                int resultado = await repositorio.Insert(entidad);

                if (resultado > 0)
                {
                    return Ok($"El jugador {entidad.Nombre} fue agregado exitosamente con ID {resultado}");
                }
                else
                {
                    return BadRequest("No se pudo agregar el jugador. Inténtalo de nuevo.");
                }
            }
            catch (Exception err)
            {
                return BadRequest($"Error: {err.Message}");
            }
        }


        //PUT: ----------------------------------------------------------------

        [HttpPut("{ID:int}")]  
        public async Task<ActionResult> Put(int ID, [FromBody] Jugador entidad) 
        {
            if (ID != entidad.ID)
            { 
                return BadRequest("Datos Incorrectos");
            }
            var jugador = await repositorio.SelectById(ID);

            if (jugador == null)
            {
                return NotFound("No existe el jugador buscado");
            }
            jugador.ID = entidad.ID;
            jugador.Nombre = entidad.Nombre;
            jugador.Edad = entidad.Edad;
            jugador.Posicion = entidad.Posicion;

            try
            {
                await repositorio.Update(ID,jugador);
                return Ok();
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        //DELETE: ------------------------------------------------------------------

        [HttpDelete("{ID:int}")] 
        public async Task<ActionResult> Delete(int ID)
        {
            var existe = await repositorio.Existe(ID);
            if (!existe)
            {
                return NotFound($"El Jugador {ID} no existe");
            }
           
           if( await repositorio.Delete(ID))
            {
                return Ok();
            }
            else
            {
                return BadRequest();
            }
        }
    }
}
