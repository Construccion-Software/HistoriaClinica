using HistoriasClinicas.Api.Models;
using HistoriasClinicas.Api.Repositories;
using HistoriasClinicas.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace HistoriasClinicas.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HistoriasClinicasController : ControllerBase
    {
        private readonly HistoriaClinicaRepository _repo;
        private readonly HistoriaClinicaService _service;

        public HistoriasClinicasController(HistoriaClinicaRepository repo, HistoriaClinicaService service)
        {
            _repo = repo;
            _service = service;
        }

        /// <summary>
        /// Health check endpoint para Docker y orquestadores
        /// </summary>
        [HttpGet("health")]
        [ProducesResponseType(200)]
        public IActionResult Health()
        {
            return Ok(new { 
                status = "healthy", 
                timestamp = DateTime.UtcNow,
                service = "HistoriasClinicas.Api",
                version = "1.0.0"
            });
        }

        [HttpGet]
        public async Task<ActionResult<List<HistoriaClinica>>> GetAll()
        {
            var historias = await _repo.GetAllAsync();
            return Ok(historias);
        }

  
        [HttpGet("{id}")]
        public async Task<ActionResult<HistoriaClinica>> GetById(string id)
        {
            var historia = await _repo.GetByIdAsync(id);
            if (historia == null)
                return NotFound();
            return Ok(historia);
        }


        [ProducesResponseType(typeof(ValidationErrorResponse), 400)]
        [HttpPost]
        public async Task<ActionResult> Create([FromBody] HistoriaClinica historia)
        {
            var errores = await _service.ValidarHistoria(historia);
            if (errores.Count > 0)
            {
                var resp = new ValidationErrorResponse
                {
                    TraceId = HttpContext.TraceIdentifier,
                    Errors = errores
                };
                return BadRequest(resp);
            }
            await _repo.CreateAsync(historia);
            return CreatedAtAction(nameof(GetById), new { id = historia.Id }, historia);
        }


        [HttpPut("{id}")]
        public async Task<ActionResult> Update(string id, HistoriaClinica historia)
        {
            var existing = await _repo.GetByIdAsync(id);
            if (existing == null)
                return NotFound();
            historia.Id = id;
            await _repo.UpdateAsync(id, historia);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(string id)
        {
            var existing = await _repo.GetByIdAsync(id);
            if (existing == null)
                return NotFound();
            await _repo.DeleteAsync(id);
            return NoContent();
        }

        [HttpGet("paciente/{cedula}")]
        public async Task<ActionResult<List<HistoriaClinica>>> GetByCedula(string cedula)
        {
            if (string.IsNullOrWhiteSpace(cedula))
                return BadRequest("La cédula es requerida");

            var historias = await _repo.GetByCedulaAsync(cedula);
            if (historias == null || historias.Count == 0)
                return NotFound($"No se encontraron atenciones para el paciente con cédula {cedula}");

            return Ok(historias);
        }
    }
}
