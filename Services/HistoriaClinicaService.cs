using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HistoriasClinicas.Api.BusinessRules;
using HistoriasClinicas.Api.Models;
using MongoDB.Driver;
using HistoriasClinicas.Api.Repositories;

namespace HistoriasClinicas.Api.Services
{
	public class HistoriaClinicaService
	{
		private const int MaxMotivo = 500;
		private const int MaxSintomatologia = 1000;
		private const int MaxDiagnostico = 1000;
		private const int MaxNumeroOrden = 999999;

		private readonly HistoriaClinicaValidator _validator;
		private readonly HistoriaClinicaRepository _repo;

		public HistoriaClinicaService(HistoriaClinicaRepository repo)
		{
			_repo = repo;
			_validator = new HistoriaClinicaValidator();
		}

		public async Task<(bool IsValid, Dictionary<string, List<string>> Errors)> CrearRegistroAsync(string cedulaPaciente, RegistroClinico registro)
		{
			var errores = new Dictionary<string, List<string>>();

			if (registro == null)
			{
				AddError(errores, "registro", "El registro clínico es obligatorio.");
				return (false, errores);
			}

			ValidarDatosBasicos(cedulaPaciente, registro, errores);
			ValidarColecciones(registro, errores);

			if (errores.Count > 0)
				return (false, errores);

			var historiaExistente = await _repo.GetByCedulaAsync(cedulaPaciente);
			var fechaClave = FormatearFechaClave(registro.FechaAtencion);

			var contexto = new RegistroValidacionContext
			{
				CedulaPaciente = cedulaPaciente,
				HistoriaExistente = historiaExistente,
				EsActualizacion = false,
				FechaClave = fechaClave
			};

			var erroresReglas = await _validator.Validar(registro, contexto);
			foreach (var kvp in erroresReglas)
			{
				if (!errores.ContainsKey(kvp.Key))
					errores[kvp.Key] = new List<string>();
				errores[kvp.Key].AddRange(kvp.Value);
			}

			if (errores.Count > 0)
				return (false, errores);

			try
			{
				await _repo.UpsertRegistroAsync(cedulaPaciente, fechaClave, registro);
				return (true, errores);
			}
			catch (MongoWriteException ex)
			{
				AddError(errores, "persistencia", $"No se pudo guardar el registro clínico: {ex.Message}");
				return (false, errores);
			}
			catch (Exception ex)
			{
				AddError(errores, "persistencia", $"Se produjo un error inesperado al guardar la historia: {ex.Message}");
				return (false, errores);
			}
		}

		public Task<HistoriaClinica?> ObtenerHistoriaAsync(string cedulaPaciente)
		{
			return _repo.GetByCedulaAsync(cedulaPaciente);
		}

		public Task<List<HistoriaClinica>> ObtenerTodasAsync()
		{
			return _repo.GetAllAsync();
		}

		private void ValidarDatosBasicos(string cedulaPaciente, RegistroClinico registro, Dictionary<string, List<string>> errores)
		{
			if (string.IsNullOrWhiteSpace(cedulaPaciente))
				AddError(errores, "cedulaPaciente", "La cédula del paciente es obligatoria.");
			else if (cedulaPaciente.Length > 10 || !cedulaPaciente.All(char.IsDigit))
				AddError(errores, "cedulaPaciente", "La cédula del paciente debe contener máximo 10 dígitos.");

			if (registro.FechaAtencion == default)
				AddError(errores, "fechaAtencion", "La fecha de atención es obligatoria.");

			if (string.IsNullOrWhiteSpace(registro.CedulaMedico))
				AddError(errores, "cedulaMedico", "La cédula del médico es obligatoria.");
			else if (registro.CedulaMedico.Length > 10 || !registro.CedulaMedico.All(char.IsDigit))
				AddError(errores, "cedulaMedico", "La cédula del médico debe contener máximo 10 dígitos.");

			if (!string.IsNullOrWhiteSpace(registro.MotivoConsulta) && registro.MotivoConsulta.Length > MaxMotivo)
				AddError(errores, "motivoConsulta", $"El motivo de consulta no puede exceder {MaxMotivo} caracteres.");

			if (!string.IsNullOrWhiteSpace(registro.Sintomatologia) && registro.Sintomatologia.Length > MaxSintomatologia)
				AddError(errores, "sintomatologia", $"La sintomatología no puede exceder {MaxSintomatologia} caracteres.");

			if (!string.IsNullOrWhiteSpace(registro.Diagnostico) && registro.Diagnostico.Length > MaxDiagnostico)
				AddError(errores, "diagnostico", $"El diagnóstico no puede exceder {MaxDiagnostico} caracteres.");
		}

		private void ValidarColecciones(RegistroClinico registro, Dictionary<string, List<string>> errores)
		{
			if (registro.Medicamentos != null)
			{
				for (var i = 0; i < registro.Medicamentos.Count; i++)
				{
					var med = registro.Medicamentos[i];
					if (med.NumeroOrden > MaxNumeroOrden)
						AddError(errores, $"medicamentos[{i}].numeroOrden", "Número de orden no puede exceder 999999 (6 dígitos).");
				}
			}

			if (registro.Procedimientos != null)
			{
				for (var i = 0; i < registro.Procedimientos.Count; i++)
				{
					var proc = registro.Procedimientos[i];
					if (proc.NumeroOrden > MaxNumeroOrden)
						AddError(errores, $"procedimientos[{i}].numeroOrden", "Número de orden no puede exceder 999999 (6 dígitos).");
					if (proc.RequiereAsistenciaEspecialista && (proc.Especialidades == null || !proc.Especialidades.Any()))
						AddError(errores, $"procedimientos[{i}].especialidades", "Debe indicar las especialidades requeridas.");
				}
			}

			if (registro.AyudasDiagnosticas != null)
			{
				for (var i = 0; i < registro.AyudasDiagnosticas.Count; i++)
				{
					var ayuda = registro.AyudasDiagnosticas[i];
					if (ayuda.NumeroOrden > MaxNumeroOrden)
						AddError(errores, $"ayudasDiagnosticas[{i}].numeroOrden", "Número de orden no puede exceder 999999 (6 dígitos).");
					if (ayuda.RequiereAsistenciaEspecialista && (ayuda.Especialidades == null || !ayuda.Especialidades.Any()))
						AddError(errores, $"ayudasDiagnosticas[{i}].especialidades", "Debe indicar las especialidades requeridas.");
				}
			}
		}

		private void AddError(Dictionary<string, List<string>> errors, string key, string message)
		{
			if (!errors.ContainsKey(key))
				errors[key] = new List<string>();

			errors[key].Add(message);
		}

		private static string FormatearFechaClave(DateTime fecha)
		{
			var utc = DateTime.SpecifyKind(fecha, DateTimeKind.Utc);
			if (fecha.Kind == DateTimeKind.Local)
				utc = fecha.ToUniversalTime();
			else if (fecha.Kind == DateTimeKind.Unspecified)
				utc = DateTime.SpecifyKind(fecha, DateTimeKind.Utc);

			return utc.ToString("yyyyMMdd'T'HHmmssfff'Z'");
		}
	}
}
