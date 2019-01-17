using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Lab03
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        #region Dispatcher

        /*
         * En .Net Framework cada Thread es asociado a un Dispatcher.
         * Este se encarga de administrar la cola de tareas de un hilo (agrega,saca, etc tasks de un hilo)
         * Podemos utilizar este objeto para mandar ejecutar logica a un hilo puntual
         */

        delegate void ShowMessageDelegate(string message);

        private void btnGetResult_Dispatcher_Click(object sender, RoutedEventArgs e)
        {
            /*
             *  Task.Run(() => 
            {
                string result = "Resultado obtenido - Dispatcher";
                ShowMessage(result);
            }
            );
            * System.InvalidOperationException: 'El subproceso que realiza la llamada no puede obtener acceso a este 
            * objeto porque el propietario es otro subproceso.'
            * 
            * Esto se debe a que estamos tratando de utilizar el objeto Label lblResult_Dispatcher que pertence a un hilo distinto que el que ejecuta ShowMessage().
            * El label es propiedad del thread principal.
            * Entonces necesitamos al Dispatcher que maneja al hilo principal para poder acceder a sus funcionalidades.
            * En este caso podemos acceder al dispatcher asociado al label. De esta forma le decimos al dispatcher del label (que maneja el hilo principal) que se él el que 
            * ejecute el ShowMessage, entonces no hay problema debido a que el podra acceder al label lblResult_Dispatcher sin problema.
            * 
            * BeginInvoke coloca la logica que queremos ejecutar en la cola del hilo que administra al label lblResult_Dispatcher en este caso.
            */

            Task.Run(() => 
            {
                string result = "Resultado obtenido - Dispatcher [Action]";
                lblResult_Dispatcher.Dispatcher.BeginInvoke(new Action(() => ShowMessage(result)));
            }
            );

            //Invocación con delegado
            //Task.Run(() =>
            //{
            //    string result = "Resultado obtenido - Dispatcher [Delegate]";
            //    lblResult_Dispatcher.Dispatcher.BeginInvoke(new ShowMessageDelegate(ShowMessage), result);
            //}
            //);

        }

        private void ShowMessage(string message)
        {
            lblResult_Dispatcher.Content = message;

        }

        #endregion
    }
}
