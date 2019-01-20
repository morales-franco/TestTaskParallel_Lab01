using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
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

        #region Utilizando async / await

        /*
         * Con async / await podemos realizar operaciones Asincronicas
         */


        //METODO SYNC
        private void btnGetResult_async_await_SYN_Click(object sender, RoutedEventArgs e)
        {
            lblResult_async_await.Content = "Calculando número aleatorio...";

            /*
                * lblResult_async_await.Content += $" Número obtenido: { t.Result}";
                * t.Result bloquea el hilo principal hasta que t termine de ejecutar y tenga el resultado.
                * Al bloquear el main thread (hilo UI) se bloquea toda la interface
            */
            Task<int> t = Task.Run<int>(() =>
            {
                Thread.Sleep(10000);
                return new Random().Next(50000);
            });

            lblResult_async_await.Content += $" Número obtenido: { t.Result}";
        }


        /*
         * Llevamos el metodo btnGetResult_async_await_SYN_Click para transformalo en asincronico
         * y de esta forma NO bloquear el main thread.
         * 
         *  async : Indicamos que el metodo puede ejecutarse en forn asincronica.
         *  un metodo async es SINCRONICO hasta que se encuentra con el el primer AWAIT en este punto el metodo se SUSPENDE
         *  y regresa el control al código que lo invoco hasta que la tarea sea completada.
         */

        private async void btnGetResult_async_await_Click(object sender, RoutedEventArgs e)
        {
            lblResult_async_await.Content = "Calculando número aleatorio...";

            Debug.WriteLine($"Hilo que LANZA la Task de Calculo de Numero: { Thread.CurrentThread.ManagedThreadId }");

            Task<int> t = Task.Run<int>(() =>
            {
                Debug.WriteLine($"Hilo que EJECUTA la Task de Calculo de Numero: { Thread.CurrentThread.ManagedThreadId }");
                Thread.Sleep(10000);
                return new Random().Next(50000);
            });

            /*
             * Suspendemos el metodo mientras se ejecuta una tarea de larga duración, de esta forma devolvemos el control al hilo que invoco el metodo
             * hasta que finalice de calcular el numero aleatorio
             */

            Debug.WriteLine($"Hilo ANTES DEL AWAIT: { Thread.CurrentThread.ManagedThreadId }");
            lblResult_async_await.Content += $" Número obtenido: { await t}";

            Debug.WriteLine($"Hilo DESPUES DEL AWAIT: { Thread.CurrentThread.ManagedThreadId }");
        }


        #endregion

        #region Awaitable Methods

        private async void btnGetResult_awaitable_methods_Click(object sender, RoutedEventArgs e)
        {
            lblResult_awaitable_methods.Content = "Obtener el nombre del producto";
            var productName = await GetProductName(1);
            lblResult_awaitable_methods.Content += Environment.NewLine + productName;
            await ShowNameAsync(1);
        }

        private async Task<string> GetProductName(int productId)
        {
            //Simulamos un proceso de larga duración
            string result = await Task.Run<string>(() => 
            {
                Thread.Sleep(8000);
                return "Laptop Pro - 2019";
            });

            return result;
        }

        private async Task ShowNameAsync(int productId)
        {
            string productName = await Task.Run<string>(() =>
            {
                Thread.Sleep(2000);
                return "Laptop No Pro - 2";
            });

            lblResult_awaitable_methods.Content += Environment.NewLine + productName;

        }

        #endregion

    }
}
