using System.Collections.Generic;
using System.Linq;
using HistoriasClinicas.Api.Models;

namespace HistoriasClinicas.Api.BusinessRules.Rules
{
    public class ItemsUnicosEnOrdenRule : IValidationRule
    {
        public string NombreRegla => "Items Únicos en Orden";

        public async Task<Dictionary<string, List<string>>> Validar(RegistroClinico registro, RegistroValidacionContext contexto)
        {
            var errors = new Dictionary<string, List<string>>();

            ValidarSecuenciaMedicamentos(registro.Medicamentos, errors);
            ValidarSecuenciaProcedimientos(registro.Procedimientos, errors);
            ValidarSecuenciaAyudas(registro.AyudasDiagnosticas, errors);

            return await Task.FromResult(errors);
        }

        private void ValidarSecuenciaMedicamentos(IEnumerable<MedicamentoRecetado>? lista, Dictionary<string, List<string>> errors)
        {
            if (lista == null)
                return;

            foreach (var grupo in lista.GroupBy(m => m.NumeroOrden))
                ValidarSecuencia(grupo.Key, grupo.Select(m => m.Item).OrderBy(i => i).ToList(), "medicamentos", errors);
        }

        private void ValidarSecuenciaProcedimientos(IEnumerable<ProcedimientoRealizado>? lista, Dictionary<string, List<string>> errors)
        {
            if (lista == null)
                return;

            foreach (var grupo in lista.GroupBy(p => p.NumeroOrden))
                ValidarSecuencia(grupo.Key, grupo.Select(p => p.Item).OrderBy(i => i).ToList(), "procedimientos", errors);
        }

        private void ValidarSecuenciaAyudas(IEnumerable<AyudaDiagnostica>? lista, Dictionary<string, List<string>> errors)
        {
            if (lista == null)
                return;

            foreach (var grupo in lista.GroupBy(a => a.NumeroOrden))
                ValidarSecuencia(grupo.Key, grupo.Select(a => a.Item).OrderBy(i => i).ToList(), "ayudasDiagnosticas", errors);
        }

        private void ValidarSecuencia(int numeroOrden, List<int> itemsOrdenados, string key, Dictionary<string, List<string>> errors)
        {
            var esperado = 1;
            foreach (var item in itemsOrdenados)
            {
                if (item != esperado)
                {
                    AddError(errors, key,
                        $"La orden {numeroOrden} debe numerar sus ítems secuencialmente desde 1. Se encontró el ítem {item}.");
                    break;
                }
                esperado++;
            }
        }

        private void AddError(Dictionary<string, List<string>> errors, string key, string message)
        {
            if (!errors.ContainsKey(key))
                errors[key] = new List<string>();

            errors[key].Add(message);
        }
    }
}
