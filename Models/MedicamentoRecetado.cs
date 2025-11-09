namespace HistoriasClinicas.Api.Models
{
    public class MedicamentoRecetado
    {
        public int NumeroOrden { get; set; }      
        public string? IdMedicamento { get; set; }     
        public string? Dosis { get; set; }
        public string? DuracionTratamiento { get; set; }
        public int Item { get; set; }             
    }
}
