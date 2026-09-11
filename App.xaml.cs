using System.Windows;

namespace GestorIAAS
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            this.DispatcherUnhandledException += (s, ex) =>
            {
                MessageBox.Show(ex.Exception.ToString(), "Error no controlado");
                ex.Handled = true;
            };
        }
    }
}