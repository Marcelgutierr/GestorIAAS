using System.Windows;
using GestorIAAS.Data;
using GestorIAAS.Models;
using System.Windows.Controls;
using GestorIAAS.Utils;

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
        }

        private void CargarCatalogos()
        {
            using var db = new AppDbContext();

            var tipos = db.TiposIAAS.OrderBy(t => t.Nombre).ToList();
            var servicios = db.ServiciosClinicos.OrderBy(s => s.Nombre).ToList();

            ListaTiposIAAS.ItemsSource = tipos;
            ListaServicios.ItemsSource = servicios;

            CmbTipoIAAS.ItemsSource = tipos;
            ListaServiciosSeleccion.ItemsSource = servicios;
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

            var nombreTipoIAAS = CmbTipoIAAS.Text.Trim();
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
                ServiciosClinicos = serviciosDb
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

            CargarCatalogos(); // por si se creó un Tipo de IAAS nuevo
        }
    }
}