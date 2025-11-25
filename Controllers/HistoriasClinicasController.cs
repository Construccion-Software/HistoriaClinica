using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using HistoriasClinicas.Api.Models;
using HistoriasClinicas.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace HistoriasClinicas.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HistoriasClinicasController : ControllerBase
    {
        private readonly HistoriaClinicaService _service;

        public HistoriasClinicasController(HistoriaClinicaService service)
        {
            _service = service;
        }

        /// <summary>
        /// Health check endpoint para Docker y orquestadores
        /// </summary>
        [HttpGet("health")]
        [ProducesResponseType(200)]
        public IActionResult Health()
        {
            return Ok(new
            {
                status = "healthy",
                timestamp = DateTime.UtcNow,
                service = "HistoriasClinicas.Api",
                version = "1.0.0"
            });
        }

        [HttpGet]
        public async Task<ActionResult<List<HistoriaClinica>>> ObtenerTodas()
        {
            var historias = await _service.ObtenerTodasAsync();
            return Ok(historias);
        }

        [HttpGet("{cedulaPaciente}")]
        public async Task<ActionResult<HistoriaClinica>> ObtenerPorCedula(string cedulaPaciente)
        {
            if (string.IsNullOrWhiteSpace(cedulaPaciente))
                return BadRequest("La cédula es requerida");

            var historia = await _service.ObtenerHistoriaAsync(cedulaPaciente);
            if (historia == null)
                return NotFound();

            return Ok(historia);
        }

        [ProducesResponseType(typeof(ValidationErrorResponse), 400)]
        [HttpPost("{cedulaPaciente}/historico")]
        public async Task<ActionResult> RegistrarAtencion(string cedulaPaciente, [FromBody] RegistroClinico registro)
        {
            try
            {
                var (isValid, errores) = await _service.CrearRegistroAsync(cedulaPaciente, registro);
                if (!isValid)
                {
                    var resp = new ValidationErrorResponse
                    {
                        TraceId = HttpContext.TraceIdentifier,
                        Errors = errores
                    };
                    return BadRequest(resp);
                }

                var historia = await _service.ObtenerHistoriaAsync(cedulaPaciente);
                return CreatedAtAction(nameof(ObtenerPorCedula), new { cedulaPaciente }, historia);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    error = ex.Message,
                    traceId = HttpContext.TraceIdentifier,
                    details = ex.InnerException?.Message
                });
            }
        }
    }
}
