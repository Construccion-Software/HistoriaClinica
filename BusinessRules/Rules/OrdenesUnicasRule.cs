using System.Collections.Generic;
using System.Linq;
using HistoriasClinicas.Api.Models;

namespace HistoriasClinicas.Api.BusinessRules.Rules
{
    public class OrdenesUnicasRule : IValidationRule
    {
        public string NombreRegla => "Órdenes Únicas";

        public async Task<Dictionary<string, List<string>>> Validar(RegistroClinico registro, RegistroValidacionContext contexto)
        {
            var errors = new Dictionary<string, List<string>>();

            ValidarUnicidadOrdenItems(registro.Medicamentos, "medicamentos", errors);
            ValidarUnicidadOrdenItems(registro.Procedimientos, "procedimientos", errors);
            ValidarUnicidadOrdenItems(registro.AyudasDiagnosticas, "ayudasDiagnosticas", errors);

            ValidarCruceMedicamentosProcedimientos(registro, errors);

            return await Task.FromResult(errors);
        }

        private void ValidarUnicidadOrdenItems<T>(IEnumerable<T>? registros, string key, Dictionary<string, List<string>> errors)
            where T : class
        {
            if (registros == null)
                return;

            var ordenItemMap = new Dictionary<int, HashSet<int>>();

            foreach (var entry in registros)
            {
                switch (entry)
                {
                    case MedicamentoRecetado med:
                        RegistrarOrdenItem(med.NumeroOrden, med.Item, key, errors, ordenItemMap);
                        break;
                    case ProcedimientoRealizado proc:
                        RegistrarOrdenItem(proc.NumeroOrden, proc.Item, key, errors, ordenItemMap);
                        break;
                    case AyudaDiagnostica ayuda:
                        RegistrarOrdenItem(ayuda.NumeroOrden, ayuda.Item, key, errors, ordenItemMap);
                        break;
                }
            }
        }

        private void ValidarCruceMedicamentosProcedimientos(RegistroClinico registro, Dictionary<string, List<string>> errors)
        {
            if (registro.Medicamentos == null || registro.Procedimientos == null)
                return;

            var combinaciones = new HashSet<string>();

            foreach (var med in registro.Medicamentos)
            {
                var clave = $"{med.NumeroOrden}-{med.Item}";
                combinaciones.Add(clave);
            }

            foreach (var proc in registro.Procedimientos)
            {
                var clave = $"{proc.NumeroOrden}-{proc.Item}";
                if (combinaciones.Contains(clave))
                {
                    AddError(errors, "ordenes",
                        $"El ítem {proc.Item} aparece en medicamentos y procedimientos dentro de la orden {proc.NumeroOrden}.");
                }
            }
        }

        private void RegistrarOrdenItem(int numeroOrden, int item, string key, Dictionary<string, List<string>> errors, Dictionary<int, HashSet<int>> ordenItemMap)
        {
            if (!ordenItemMap.TryGetValue(numeroOrden, out var items))
            {
                items = new HashSet<int>();
                ordenItemMap[numeroOrden] = items;
            }

            if (!items.Add(item))
            {
                AddError(errors, key, $"El ítem {item} se repite dentro de la orden {numeroOrden}.");
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
