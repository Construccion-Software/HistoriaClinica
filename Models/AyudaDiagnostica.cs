namespace HistoriasClinicas.Api.Models
{
    public class AyudaDiagnostica
    {
        public int NumeroOrden { get; set; }  
        public string? IdAyudaDiagnostica { get; set; }  
        public int Cantidad { get; set; }
        public bool RequiereAsistenciaEspecialista { get; set; }
        public List<Especialista>? Especialidades { get; set; }  
        public int Item { get; set; }                 
    }

}
