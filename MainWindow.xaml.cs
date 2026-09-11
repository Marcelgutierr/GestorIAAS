using System.Windows;
using GestorIAAS.Data;
using GestorIAAS.Models;
using System.Windows.Controls;
using GestorIAAS.Utils;
using Microsoft.EntityFrameworkCore;
using Microsoft.Win32;
using ClosedXML.Excel;
using System.Globalization;
using System.Text;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using System.IO;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;
using LiveChartsCore.Measure;

namespace GestorIAAS
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            Loaded += MainWindow_Loaded;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            CargarCatalogos();
            CargarRegistros();
            CargarGrafico();
        }

        private void CargarCatalogos()
        {
            using var db = new AppDbContext();

            var tipos = db.TiposIAAS.OrderBy(t => t.Nombre).ToList();
            var servicios = db.ServiciosClinicos.OrderBy(s => s.Nombre).ToList();
            var anios = db.RegistrosIAAS.Select(r => r.Anio).Distinct().OrderByDescending(a => a).ToList();

            ListaTiposIAAS.ItemsSource = tipos;
            ListaServicios.ItemsSource = servicios;

            CmbTipoIAAS.ItemsSource = tipos;
            ListaServiciosSeleccion.ItemsSource = servicios;
            CmbFiltroServicio.ItemsSource = servicios;
            CmbFiltroAnio.ItemsSource = anios;
            CmbGraficoAnio.ItemsSource = anios;
        }

        private static readonly string[] OrdenMeses =
        {
            "ENERO", "FEBRERO", "MARZO", "ABRIL", "MAYO", "JUNIO",
            "JULIO", "AGOSTO", "SEPTIEMBRE", "OCTUBRE", "NOVIEMBRE", "DICIEMBRE"
        };

        private List<(string Etiqueta, int Cantidad)> ObtenerDatosVista()
        {
            using var db = new AppDbContext();
            var query = db.RegistrosIAAS.Include(r => r.ServiciosClinicos).AsQueryable();

            if (CmbGraficoAnio.SelectedItem is int anioSeleccionado)
                query = query.Where(r => r.Anio == anioSeleccionado);

            var registros = query.ToList();
            var vista = (CmbVistaGrafico.SelectedItem as ComboBoxItem)?.Content.ToString();

            if (vista == "Por Mes")
            {
                return registros
                    .GroupBy(r => r.Mes.Trim().ToUpperInvariant())
                    .Select(g => (Etiqueta: g.Key, Cantidad: g.Count()))
                    .OrderBy(x => Array.IndexOf(OrdenMeses, x.Etiqueta))
                    .ToList();
            }

            // Por defecto: Por Servicio Clínico
            return registros
                .SelectMany(r => r.ServiciosClinicos.Select(s => s.Nombre))
                .GroupBy(nombre => nombre)
                .Select(g => (Etiqueta: g.Key, Cantidad: g.Count()))
                .OrderByDescending(x => x.Cantidad)
                .ToList();
        }

        private void CargarGrafico()
        {
            var datos = ObtenerDatosVista();
            double anchoNecesario = Math.Max(800, datos.Count * 160);
            GrupoGraficos.Width = anchoNecesario;
            var tipo = (CmbTipoGrafico.SelectedItem as ComboBoxItem)?.Content.ToString();

            bool esTorta = tipo == "Torta";
            GraficoTorta.Visibility = esTorta ? Visibility.Visible : Visibility.Collapsed;
            GraficoCartesiano.Visibility = esTorta ? Visibility.Collapsed : Visibility.Visible;

            if (esTorta)
            {
                GraficoTorta.Series = datos.Select(d => new PieSeries<int>
                {
                    Values = new[] { d.Cantidad },
                    Name = d.Etiqueta,
                    DataLabelsPaint = new SolidColorPaint(SKColors.Black),
                    DataLabelsFormatter = point => $"{d.Etiqueta}: {point.Coordinate.PrimaryValue}"
                }).ToArray();
            }
            else if (tipo == "Línea")
            {
                GraficoCartesiano.Series = new ISeries[]
                {
                    new LineSeries<int>
                    {
                        Values = datos.Select(d => d.Cantidad).ToArray(),
                        Name = "Cantidad de IAAS",
                        DataLabelsPaint = new SolidColorPaint(SKColors.Black),
                        DataLabelsPosition = DataLabelsPosition.Top
                    }
                            };
                GraficoCartesiano.XAxes = new[]
                {
                    new LiveChartsCore.SkiaSharpView.Axis
                    {
                        Labels = datos.Select(d => d.Etiqueta).ToArray(),
                        LabelsRotation = 15,
                        MinStep = 1,
                        ForceStepToMin = true
                    }
                };
            }
            else // Barras (por defecto)
            {
                GraficoCartesiano.Series = new ISeries[]
                {
                    new ColumnSeries<int>
                    {
                        Values = datos.Select(d => d.Cantidad).ToArray(),
                        Name = "Cantidad de IAAS",
                        DataLabelsPaint = new SolidColorPaint(SKColors.Black),
                        DataLabelsPosition = DataLabelsPosition.Top
                    }
                            };
                GraficoCartesiano.XAxes = new[]
                {
                    new LiveChartsCore.SkiaSharpView.Axis
                    {
                        Labels = datos.Select(d => d.Etiqueta).ToArray(),
                        LabelsRotation = 15,
                        MinStep = 1,
                        ForceStepToMin = true
                    }
                };
            }
        }

        private void ActualizarGrafico(object sender, SelectionChangedEventArgs e)
        {
            if (!IsLoaded) return;
            CargarGrafico();
        }

        private void CmbGraficoAnio_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            CargarGrafico();
        }
        private void BtnAgregarTipoIAAS_Click(object sender, RoutedEventArgs e)
        {
            var nombre = TxtNuevoTipoIAAS.Text.Trim();
            if (string.IsNullOrEmpty(nombre))
            {
                MessageBox.Show("Escribe un nombre para el Tipo de IAAS.");
                return;
            }

            using var db = new AppDbContext();

            db.TiposIAAS.Add(new TipoIAAS
            {
                Nombre = nombre,
                NotificaMinsal = ChkNotificaMinsal.IsChecked == true
            });
            db.SaveChanges();

            TxtNuevoTipoIAAS.Clear();
            ChkNotificaMinsal.IsChecked = false;
            CargarCatalogos();
        }

        private void BtnAgregarServicio_Click(object sender, RoutedEventArgs e)
        {
            var nombre = TxtNuevoServicio.Text.Trim();
            if (string.IsNullOrEmpty(nombre))
            {
                MessageBox.Show("Escribe un nombre para el Servicio Clínico.");
                return;
            }

            using var db = new AppDbContext();

            db.ServiciosClinicos.Add(new ServicioClinico
            {
                Nombre = nombre,
                EsEspecialidad = ChkEsEspecialidad.IsChecked == true
            });
            db.SaveChanges();

            TxtNuevoServicio.Clear();
            ChkEsEspecialidad.IsChecked = false;
            CargarCatalogos();
        }

        private void BtnEliminarTipoIAAS_Click(object sender, RoutedEventArgs e)
        {
            if (ListaTiposIAAS.SelectedItem is not TipoIAAS seleccionado)
            {
                MessageBox.Show("Selecciona un Tipo de IAAS de la lista primero.");
                return;
            }

            var confirmacion = MessageBox.Show($"¿Eliminar '{seleccionado.Nombre}'?", "Confirmar", MessageBoxButton.YesNo);
            if (confirmacion != MessageBoxResult.Yes) return;

            using var db = new AppDbContext();
            var entidad = db.TiposIAAS.Find(seleccionado.Id);
            if (entidad != null)
            {
                db.TiposIAAS.Remove(entidad);
                db.SaveChanges();
            }

            CargarCatalogos();
        }

        private void BtnEliminarServicio_Click(object sender, RoutedEventArgs e)
        {
            if (ListaServicios.SelectedItem is not ServicioClinico seleccionado)
            {
                MessageBox.Show("Selecciona un Servicio Clínico de la lista primero.");
                return;
            }

            var confirmacion = MessageBox.Show($"¿Eliminar '{seleccionado.Nombre}'?", "Confirmar", MessageBoxButton.YesNo);
            if (confirmacion != MessageBoxResult.Yes) return;

            using var db = new AppDbContext();
            var entidad = db.ServiciosClinicos.Find(seleccionado.Id);
            if (entidad != null)
            {
                db.ServiciosClinicos.Remove(entidad);
                db.SaveChanges();
            }

            CargarCatalogos();
        }

        private void BtnGuardarRegistro_Click(object sender, RoutedEventArgs e)
        {
            // Validaciones básicas
            if (CmbMes.SelectedItem is not ComboBoxItem mesSeleccionado)
            {
                MessageBox.Show("Selecciona un mes.");
                return;
            }

            if (!int.TryParse(TxtAnio.Text.Trim(), out int anio))
            {
                MessageBox.Show("Ingresa un año válido (solo números).");
                return;
            }

            var rut = RutHelper.Formatear(TxtRut.Text.Trim());
            if (string.IsNullOrEmpty(rut))
            {
                MessageBox.Show("Ingresa el Rut.");
                return;
            }

            var nombreTipoIAAS = QuitarSufijoNotifica(CmbTipoIAAS.Text.Trim());
            if (string.IsNullOrEmpty(nombreTipoIAAS))
            {
                MessageBox.Show("Selecciona o escribe un Tipo de IAAS.");
                return;
            }

            var serviciosSeleccionados = ListaServiciosSeleccion.SelectedItems.Cast<ServicioClinico>().ToList();
            if (serviciosSeleccionados.Count == 0)
            {
                MessageBox.Show("Selecciona al menos un Servicio Clínico.");
                return;
            }

            using var db = new AppDbContext();

            // Buscar si el Tipo de IAAS ya existe (por nombre exacto); si no, crearlo
            var tipoIAAS = db.TiposIAAS.FirstOrDefault(t => t.Nombre == nombreTipoIAAS);
            if (tipoIAAS == null)
            {
                tipoIAAS = new TipoIAAS { Nombre = nombreTipoIAAS, NotificaMinsal = false };
                db.TiposIAAS.Add(tipoIAAS);
                db.SaveChanges(); // Guardamos ya para que tenga un Id válido
            }

            // Como los servicios seleccionados vienen de una lista cargada en otro contexto de BD,
            // los volvemos a buscar en este contexto para que EF los trackee correctamente
            var idsSeleccionados = serviciosSeleccionados.Select(s => s.Id).ToList();
            var serviciosDb = db.ServiciosClinicos.Where(s => idsSeleccionados.Contains(s.Id)).ToList();

            var registro = new RegistroIAAS
            {
                Mes = mesSeleccionado.Content.ToString()!,
                Anio = anio,
                Rut = rut,
                NumeroIAAS = 1,
                Microorganismo = TxtMicroorganismo.Text.Trim(),
                Observacion = string.IsNullOrWhiteSpace(TxtObservacion.Text) ? null : TxtObservacion.Text.Trim(),
                TipoIAASId = tipoIAAS.Id,
                ServiciosClinicos = serviciosDb,
                EsBrote = ChkEsBrote.IsChecked == true,
                NotificaMinsal = ChkNotificaMinsal2.IsChecked == true
            };

            db.RegistrosIAAS.Add(registro);
            db.SaveChanges();

            MessageBox.Show("Registro guardado correctamente.");

            // Limpiar formulario
            CmbMes.SelectedItem = null;
            TxtAnio.Clear();
            TxtRut.Clear();
            CmbTipoIAAS.Text = "";
            TxtMicroorganismo.Clear();
            TxtObservacion.Clear();
            ListaServiciosSeleccion.SelectedItems.Clear();
            ChkEsBrote.IsChecked = false;

            CargarCatalogos(); // por si se creó un Tipo de IAAS nuevo
            CargarRegistros();
            ChkNotificaMinsal2.IsChecked = false;
        }

        private void CargarRegistros()
        {
            using var db = new AppDbContext();

            var query = db.RegistrosIAAS
                .Include(r => r.TipoIAAS)
                .Include(r => r.ServiciosClinicos)
                .AsQueryable();

            if (CmbFiltroAnio.SelectedItem is int anioSeleccionado)
            {
                query = query.Where(r => r.Anio == anioSeleccionado);
            }

            if (CmbFiltroServicio.SelectedItem is ServicioClinico servicioSeleccionado)
            {
                query = query.Where(r => r.ServiciosClinicos.Any(s => s.Id == servicioSeleccionado.Id));
            }

            if (ChkFiltroBrote.IsChecked == true)
            {
                query = query.Where(r => r.EsBrote);
            }

            if (ChkFiltroMinsal.IsChecked == true)
            {
                query = query.Where(r => r.NotificaMinsal);
            }

            var lista = query
                .OrderByDescending(r => r.Anio)
                .ThenByDescending(r => r.Id)
                .ToList();

            GridRegistros.ItemsSource = lista;
            TxtTotalRegistros.Text = $"Total de registros mostrados: {lista.Count}";
        }

        private void Filtro_Changed(object sender, RoutedEventArgs e)
        {
            if (!IsLoaded) return;
            CargarRegistros();
        }

        private void BtnQuitarFiltro_Click(object sender, RoutedEventArgs e)
        {
            CmbFiltroAnio.SelectedItem = null;
            CmbFiltroServicio.SelectedItem = null;
            ChkFiltroBrote.IsChecked = false;
            ChkFiltroMinsal.IsChecked = false;
            CargarRegistros();
        }

        private void BtnImportarExcel_Click(object sender, RoutedEventArgs e)
        {
            if (!int.TryParse(TxtAnioImportacion.Text.Trim(), out int anio))
            {
                MessageBox.Show("Ingresa el año de los registros antes de importar.");
                return;
            }

            var dialogo = new OpenFileDialog { Filter = "Archivos Excel (*.xlsx)|*.xlsx" };
            if (dialogo.ShowDialog() != true) return;

            using var db = new AppDbContext();
            using var libro = new XLWorkbook(dialogo.FileName);
            var hoja = libro.Worksheet(1);

            IXLRow? filaEncabezado = null;
            foreach (var row in hoja.RowsUsed())
            {
                if (row.CellsUsed().Any(c => NormalizarTexto(c.GetString()) == "MES"))
                {
                    filaEncabezado = row;
                    break;
                }
            }

            if (filaEncabezado == null)
            {
                MessageBox.Show("No se encontró la fila de encabezados (se buscó una celda que diga 'MES'). Revisa el archivo.");
                return;
            }

            var columnas = new Dictionary<string, int>();
            foreach (var cell in filaEncabezado.CellsUsed())
            {
                var clave = NormalizarTexto(cell.GetString());
                if (!string.IsNullOrEmpty(clave) && !columnas.ContainsKey(clave))
                    columnas[clave] = cell.Address.ColumnNumber;
            }

            string[] obligatorias = { "MES", "IAAS", "RUT" };
            var faltantes = obligatorias.Where(o => !columnas.ContainsKey(o)).ToList();
            if (faltantes.Count > 0)
            {
                MessageBox.Show($"Faltan columnas obligatorias en el Excel: {string.Join(", ", faltantes)}");
                return;
            }

            // Precargar catálogos existentes, indexados por nombre normalizado (evita duplicados por acentos/mayúsculas)
            var tiposPorNombre = db.TiposIAAS.ToList()
                .GroupBy(t => NormalizarTexto(t.Nombre))
                .ToDictionary(g => g.Key, g => g.First());

            var serviciosPorNombre = db.ServiciosClinicos.ToList()
                .GroupBy(s => NormalizarTexto(s.Nombre))
                .ToDictionary(g => g.Key, g => g.First());

            // Precargar los DOT ya importados para este año, para no duplicar
            var dotsExistentes = db.RegistrosIAAS
                .Where(r => r.Anio == anio && r.DotOriginal != null)
                .Select(r => r.DotOriginal!.Value)
                .ToHashSet();

            int filaActual = filaEncabezado.RowNumber() + 1;
            int importados = 0;
            int duplicados = 0;
            int errores = 0;

            while (!hoja.Row(filaActual).IsEmpty())
            {
                try
                {
                    string LeerColumna(string clave) =>
                        columnas.TryGetValue(clave, out int col) ? hoja.Cell(filaActual, col).GetString().Trim() : "";

                    var mes = LeerColumna("MES");
                    var nombreIAAS = LeerColumna("IAAS");
                    var rutCrudo = LeerColumna("RUT");
                    var minsalTexto = LeerColumna("NOTIFICACION MINSAL");
                    var servicioTexto = LeerColumna("SERVICIO CLINICO");
                    var numeroIAASTexto = LeerColumna("NIAAS");
                    var microorganismo = LeerColumna("MICROORGANISMO");
                    var observacion = LeerColumna("OBSERVACION");
                    var dotTexto = LeerColumna("DOT");

                    if (string.IsNullOrEmpty(nombreIAAS) || string.IsNullOrEmpty(rutCrudo))
                    {
                        filaActual++;
                        continue;
                    }

                    // Control de duplicados por DOT + Año
                    int? dotOriginal = null;
                    if (int.TryParse(dotTexto, out int dotParseado))
                    {
                        if (dotsExistentes.Contains(dotParseado))
                        {
                            duplicados++;
                            filaActual++;
                            continue;
                        }
                        dotOriginal = dotParseado;
                    }

                    bool notificaMinsal = minsalTexto.ToUpperInvariant().Contains("SI");
                    bool esBrote = NormalizarTexto(minsalTexto).Contains("BROTE");
                    if (esBrote) notificaMinsal = true;
                    int.TryParse(numeroIAASTexto, out int numeroIAAS);
                    if (numeroIAAS == 0) numeroIAAS = 1;

                    var claveIAAS = NormalizarTexto(nombreIAAS);
                    if (!tiposPorNombre.TryGetValue(claveIAAS, out var tipoIAAS))
                    {
                        tipoIAAS = new TipoIAAS { Nombre = nombreIAAS, NotificaMinsal = notificaMinsal };
                        db.TiposIAAS.Add(tipoIAAS);
                        db.SaveChanges();
                        tiposPorNombre[claveIAAS] = tipoIAAS;
                    }

                    var nombresServicios = servicioTexto.Split('+').Select(s => s.Trim()).Where(s => !string.IsNullOrEmpty(s)).ToList();
                    var serviciosDelRegistro = new List<ServicioClinico>();
                    foreach (var nombreServicio in nombresServicios)
                    {
                        var claveServicio = NormalizarTexto(nombreServicio);
                        if (!serviciosPorNombre.TryGetValue(claveServicio, out var servicio))
                        {
                            servicio = new ServicioClinico { Nombre = nombreServicio, EsEspecialidad = false };
                            db.ServiciosClinicos.Add(servicio);
                            db.SaveChanges();
                            serviciosPorNombre[claveServicio] = servicio;
                        }
                        serviciosDelRegistro.Add(servicio);
                    }

                    var registro = new RegistroIAAS
                    {
                        Mes = mes,
                        Anio = anio,
                        Rut = RutHelper.Formatear(rutCrudo),
                        NumeroIAAS = numeroIAAS,
                        Microorganismo = microorganismo,
                        Observacion = string.IsNullOrWhiteSpace(observacion) ? null : observacion,
                        TipoIAASId = tipoIAAS.Id,
                        ServiciosClinicos = serviciosDelRegistro,
                        DotOriginal = dotOriginal,
                        EsBrote = esBrote,
                        NotificaMinsal = notificaMinsal
                    };

                    db.RegistrosIAAS.Add(registro);
                    db.SaveChanges();

                    if (dotOriginal.HasValue) dotsExistentes.Add(dotOriginal.Value);
                    importados++;
                }
                catch
                {
                    errores++;
                }

                filaActual++;
            }

            TxtResultadoImportacion.Text = $"Importación terminada: {importados} nuevos, {duplicados} ya existían (omitidos), {errores} filas con error.";
            CargarCatalogos();
            CargarRegistros();
        }

        private static string NormalizarTexto(string texto)
        {
            var sinAcentos = new string(texto
                .Normalize(NormalizationForm.FormD)
                .Where(c => CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark)
                .ToArray());

            return sinAcentos.Replace("°", "").ToUpperInvariant().Trim();
        }

        private void BtnExportarExcel_Click(object sender, RoutedEventArgs e)
        {
            var dialogo = new SaveFileDialog
            {
                Filter = "Archivo Excel (*.xlsx)|*.xlsx",
                FileName = "RegistrosIAAS.xlsx"
            };

            if (dialogo.ShowDialog() != true) return;

            using var db = new AppDbContext();
            var registros = db.RegistrosIAAS
                .Include(r => r.TipoIAAS)
                .Include(r => r.ServiciosClinicos)
                .OrderBy(r => r.Anio).ThenBy(r => r.Id)
                .ToList();

            using var libro = new XLWorkbook();
            var hoja = libro.Worksheets.Add("Registros IAAS");

            // Encabezados
            string[] encabezados = { "MES", "IAAS", "RUT", "NOTIFICACION MINSAL", "SERVICIO CLINICO", "N°IAAS", "MICROORGANISMO", "OBSERVACION", "DOT" };
            for (int col = 0; col < encabezados.Length; col++)
            {
                hoja.Cell(1, col + 1).Value = encabezados[col];
                hoja.Cell(1, col + 1).Style.Font.Bold = true;
            }

            int fila = 2;
            foreach (var r in registros)
            {
                hoja.Cell(fila, 1).Value = r.Mes;
                hoja.Cell(fila, 2).Value = r.TipoIAAS?.Nombre ?? "";
                hoja.Cell(fila, 3).Value = r.Rut;

                bool esMinsal = r.NotificaMinsal;
                var celdaMinsal = hoja.Cell(fila, 4);
                celdaMinsal.Value = esMinsal ? (r.EsBrote ? "SI, BROTE" : "SI") : "NO";
                if (esMinsal)
                {
                    celdaMinsal.Style.Fill.BackgroundColor = XLColor.Red;
                    celdaMinsal.Style.Font.FontColor = XLColor.White;
                }

                hoja.Cell(fila, 5).Value = string.Join(" + ", r.ServiciosClinicos.Select(s => s.Nombre));
                hoja.Cell(fila, 6).Value = r.NumeroIAAS;
                hoja.Cell(fila, 7).Value = r.Microorganismo;
                hoja.Cell(fila, 8).Value = r.Observacion ?? "";
                hoja.Cell(fila, 9).Value = r.Id;

                fila++;
            }

            hoja.Columns().AdjustToContents();
            libro.SaveAs(dialogo.FileName);

            MessageBox.Show($"Exportación completa: {registros.Count} registros guardados en {dialogo.FileName}");
        }

        private void CmbTipoIAAS_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CmbTipoIAAS.SelectedItem is TipoIAAS tipo)
            {
                ChkNotificaMinsal2.IsChecked = tipo.NotificaMinsal;
            }
        }

        private static string QuitarSufijoNotifica(string texto) =>
            texto.Replace(" (Se notifica)", "").Trim();

        private void ChkEsBrote_CheckedChanged(object sender, RoutedEventArgs e)
        {
            if (ChkEsBrote.IsChecked == true)
            {
                ChkNotificaMinsal2.IsChecked = true;
                ChkNotificaMinsal2.IsEnabled = false; // no se puede desmarcar mientras sea brote
            }
            else
            {
                ChkNotificaMinsal2.IsEnabled = true;
                ChkNotificaMinsal2.IsChecked = CmbTipoIAAS.SelectedItem is TipoIAAS tipo && tipo.NotificaMinsal;
            }
        }

        private void BtnDescargarGrafico_Click(object sender, RoutedEventArgs e)
        {
            var dialogo = new SaveFileDialog
            {
                Filter = "Imagen PNG (*.png)|*.png",
                FileName = "GraficoIAAS.png"
            };

            if (dialogo.ShowDialog() != true) return;

            var ancho = (int)GrupoGraficos.ActualWidth;
            var alto = (int)GrupoGraficos.ActualHeight;

            var renderTarget = new RenderTargetBitmap(ancho, alto, 96, 96, PixelFormats.Pbgra32);
            renderTarget.Render(GrupoGraficos);

            var encoder = new PngBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(renderTarget));

            using var stream = new FileStream(dialogo.FileName, FileMode.Create);
            encoder.Save(stream);

            MessageBox.Show("Gráfico guardado correctamente.");
        }
    }
}