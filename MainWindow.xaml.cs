using System.Windows;
using GestorIAAS.Data;
using GestorIAAS.Models;

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

            ListaTiposIAAS.ItemsSource = db.TiposIAAS.OrderBy(t => t.Nombre).ToList();
            ListaServicios.ItemsSource = db.ServiciosClinicos.OrderBy(s => s.Nombre).ToList();
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
    }
}