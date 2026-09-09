using System.Linq;

namespace GestorIAAS.Utils
{
    public static class RutHelper
    {
        public static string Formatear(string rutCrudo)
        {
            if (string.IsNullOrWhiteSpace(rutCrudo))
                return rutCrudo;

            // Quitar puntos, guiones y espacios, y pasar a mayúscula (por si el dígito verificador es 'k')
            var limpio = rutCrudo.Replace(".", "").Replace("-", "").Replace(" ", "").ToUpper();

            if (limpio.Length < 2)
                return rutCrudo; // muy corto para ser un Rut, lo dejamos tal cual vino

            var cuerpo = limpio[..^1];       // todo menos el último caracter
            var digitoVerificador = limpio[^1..]; // el último caracter

            if (!cuerpo.All(char.IsDigit))
                return rutCrudo; // no parece un Rut válido, no lo tocamos

            // Insertar puntos cada 3 dígitos, de derecha a izquierda
            var cuerpoFormateado = "";
            int contador = 0;
            for (int i = cuerpo.Length - 1; i >= 0; i--)
            {
                cuerpoFormateado = cuerpo[i] + cuerpoFormateado;
                contador++;
                if (contador % 3 == 0 && i != 0)
                    cuerpoFormateado = "." + cuerpoFormateado;
            }

            return $"{cuerpoFormateado}-{digitoVerificador}";
        }
    }
}