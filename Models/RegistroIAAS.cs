namespace GestorIAAS.Models
{
    public class RegistroIAAS
    {
        public int Id { get; set; }
        public string Mes { get; set; } = string.Empty;
        public int Anio { get; set; }
        public string Rut { get; set; } = string.Empty;
        public int NumeroIAAS { get; set; } = 1;
        public string Microorganismo { get; set; } = string.Empty;
        public string? Observacion { get; set; }

        public int TipoIAASId { get; set; }
        public TipoIAAS? TipoIAAS { get; set; }

        public List<ServicioClinico> ServiciosClinicos { get; set; } = new();

        public int? DotOriginal { get; set; }

        public bool EsBrote { get; set; }

        public bool NotificaMinsal { get; set; }

        public string ServiciosTexto => string.Join(", ", ServiciosClinicos.Select(s => s.Nombre));
    }
}