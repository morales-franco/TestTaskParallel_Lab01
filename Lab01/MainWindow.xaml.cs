using System;
using System.Collections.Generic;
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

namespace Lab01
{
    /// <summary>
    /// Lógica de interacción para MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            //CreateTask();
            //RunTaskGroupAndWaitAll();
            //RunTaskGroupAndWaitAny();
            //ReturnTaskValue();
        }

        void CreateTask()
        {
            //La clase Task permite realizar multiples tareas al mismo tiempo, cada una en thread distinto
            //Una Task ejecuta un bloque de codigo
            //Tarea que va a ejecutar un Action
            Task T1;

            //Action: Delegado que no retorna un resultado (void) pero puede recibir parametros. Representa un bloque de código a ejecutar
            Action code = new Action(ShowMessage);

            //La tarea va a ejecutar el delegado que le pasamos en el constructor.
            T1 = new Task(code);

            //Si no volvemos a utilizar el Delegado entonces podemos pasarle directamente el Metodo
            //o generar un Delegado con metodo anonimo
            //T1 = new Task(ShowMessage);

            //Delegado con metodo anonimo
            Task T2 = new Task(delegate
            {
                MessageBox.Show("Ejecutando una tarea en un delegado anonimo!");
            });

            //Crear tarea mediante una Labda expression
            //(Paramtros de entrada) => Expression
            Task T3 = new Task(() => ShowMessage());

            Task T4 = new Task(() => MessageBox.Show("Ejecutando la tarea 4"));

            Task T5 = new Task(() =>
           {
               DateTime curretDate = DateTime.Today;
               DateTime startDate = curretDate.AddDays(30);
               MessageBox.Show($"Tarea 5. Fecha Calculada: {startDate}");
           });

            //Utilizamos sobrecarga Action con 1 Parametro
            Task T6 = new Task((message) =>
                MessageBox.Show(message.ToString()), "Valor de Message");

            Task T7 = new Task(() => AddMessage("Ejecutando en en otro Thread - Task 7"));

            //Cuando stateamos la Task la bibilioteca TaskParallel asigna un Thread a esa Tarea 
            //y esta comieza a ejecutarse en un Thread Independiente
            //Entonces la task T7 y el Thead Principal se ejecutaran en Paralelo
            T7.Start();

            AddMessage("Ejecutando en el Main Thread");

            /*
             * Podemos delegar la creación de Tasks a la TaskFactory para crear y poner en cola una tarea. 
             * Nos permite iniciar y Ejecutar en una unica linea de codigo.
             */
            Task T8 = Task.Factory.StartNew(() => AddMessage("Task T8 Iniciada con TaskFactory"));

            //Metodo Run crea y ejecuta al mismo tiempo
            //Run() es un atajo / shorcut del factory
            //Task.Factory. es altamente configurable, si solo deseamos ejecutar algún código sin opciones adicionales de ejecución --> Podemos utilizar:
            var T9 = Task.Run(() => AddMessage("Task T9 ejecutada desde Task Run"));

            var T10 = Task.Run(() =>
            {
                WriteToOutput("Iniciando Tarea 10...");
                //Simulamos un proceso de 10 segundos
                Thread.Sleep(10000);
                WriteToOutput("Fin Tarea 10");
            });

            WriteToOutput($"Esperando a la tarea 10 - Segundos Now: { DateTime.Now.Second }");

            //Esperamos a que se termine de ejecutar la T10 esto bloquea el Hilo Principal
            //Es decir NO veriamos la UI hasta que la T10 termine
            T10.Wait();
            WriteToOutput($"Finalizo tarea 10 - Segundos Now: { DateTime.Now.Second }");



        }

        void ShowMessage()
        {
            MessageBox.Show("Ejecutando el método ShowMessage");
        }

        void AddMessage(string message)
        {
            //Messages No puede ser accedido por otro thread que no sea el principal
            //Messages pertenece a la UI, solo puede ser accedido con principal thread

            //Es necesario utilizar el Distpacher ya que sino da Exception cuando la T7 quiere modificar la UI
            //Esto sucede en wpf cuando un thead distinto al principal intenta modificar elementos de la interfaz de usuario.

            int currentThreadID = Thread.CurrentThread.ManagedThreadId;

            //Con la opción Dispatcher le decis que ejecute el codigo que le pasas desde el hilo principal. (El codigo que le pasemos lo ejecuta el Main Thread)
            //Por esta razon tomamos el ManagedThreadId fuera del Dispacher porque sino ManagedThreadId siempre seria el principal Thread
            /*
             * this.Dispatcher.Invoke(() =>
            {
                Messages.Content +=
                $"Mensaje: {message}" +
                $"Hilo Actual: { Thread.CurrentThread.ManagedThreadId } \n"; --> Siempre mostraria el ManagedThreadId del hilo principal
            });
             */
            this.Dispatcher.Invoke(() =>
            {
                Messages.Content +=
                $"Mensaje: {message} " +
                $"Hilo Actual: { currentThreadID } \n";
            });

        }

        void WriteToOutput(string message)
        {
            System.Diagnostics.Debug.WriteLine(
                $"Mensaje: {message}," +
                $"Thread Actual: { Thread.CurrentThread.ManagedThreadId }"
                );
        }

        //Proceso que dura 10 segundos
        void RunTask(byte taskNumber)
        {
            WriteToOutput($"Iniciando Tarea {taskNumber}");
            Thread.Sleep(10000); //Thread suspendido por 10 segundos
            WriteToOutput($"Fin Tarea {taskNumber}");
        }

        void RunTaskGroupAndWaitAll()
        {
            Task[] taskGroup = new Task[]
            {
                Task.Run(() => RunTask(1) ),
                Task.Run(() => RunTask(2) ),
                Task.Run(() => RunTask(3) ),
                Task.Run(() => RunTask(4) ),
                Task.Run(() => RunTask(5) )
            };

            WriteToOutput($"Esperando a que Finalice TODO el conjunto de Tareas - Segundos Now: { DateTime.Now.Second }");
            //Task.WaitAll: El hilo principal espera a que todas las tareas lanzadas en el array de task finalicen su ejecución
            Task.WaitAll(taskGroup);
            WriteToOutput($"Todas las tareas del Group han Finalizado - Segundos Now: { DateTime.Now.Second }");
        }

        void RunTaskGroupAndWaitAny()
        {
            Task[] taskGroup = new Task[]
            {
                Task.Run(() => RunTask(1) ),
                Task.Run(() => RunTask(2) ),
                Task.Run(() => RunTask(3) ),
                Task.Run(() => RunTask(4) ),
                Task.Run(() => RunTask(5) )
            };

            WriteToOutput($"Esperando a que Finalice ALGUNA TAREA del conjunto - Segundos Now: { DateTime.Now.Second }");
            //Task.WaitAny Espera que al menos una tarea finalice (pueden ser varias pero al menos 1 termino)
            Task.WaitAny(taskGroup);
            WriteToOutput($"ALGUNA las tareas del Group han Finalizado - Segundos Now: { DateTime.Now.Second }");
        }

        void ReturnTaskValue()
        {
            Task<int> T;

            T = Task.Run<int>(() => new Random().Next(1000));

            //Cuando el Main Thread intenta acceder a la propiedad Result y la task aun NO ha terminado
            //Entonces el Main Thread espera hasta que el Resultado este disponible. ==> Hce que la operación se vuelva Sincrona
            WriteToOutput($"Valor devuelto por la tarea: { T.Result }");

            Task<int> T2 = Task.Run<int>(() =>
            {
                WriteToOutput("TAREA 2 : Obteniendo el numero aleatorio...");
                Thread.Sleep(10000); //Simulamos un procesamiento de 10 segundos
                return new Random().Next(1000);
            });

            WriteToOutput("Esperar el resultado de la TAREA 2...");
            WriteToOutput($"Valor devuelto por la TAREA 2: { T2.Result }");
            WriteToOutput("Fin de ejecución del método ReturnTaskValue...");
        }

        #region Cancelar Tarea

        /*
         * En algunos casos necesitamos cancelar tareas de larga duración.
         * Para esto utilizamos los Cancellation Tokens de la biblioteca Task parallel.
         * 
         */

        CancellationTokenSource CTS;
        CancellationToken CT;
        Task LongRunningTask;

        private void StartTask_Click(object sender, RoutedEventArgs e)
        {
            CTS = new CancellationTokenSource();
            //Obtenemos el token
            CT = CTS.Token;

            /*
             * Cuando cancelamos una tarea si no nos interesa saber el Porque se cancelo entonces podemos solamente pasar
             * el Token de cancelación como Parametro al delegado, y no como argumento al Run.
             *  LongRunningTask = Task.Run(() =>
                {
                    DoLongRunningTask(CT);
                });
             * 
             * De esta forma siempre el estado de la Task al momento de cancelar o finalizar será: RanToCompletion.
             * Ahora si necesitamos saber si la tarea finalizo a causa de un pedido del usuario (mediante el CT) o porque finalizo la task
             * necesitamos pasarle el CT como parametro al Run.
             */
            //LongRunningTask = Task.Run(() =>
            //{
            //    DoLongRunningTask(CT);
            //}, CT);


            /*
             * Para manejar la TaskCanceledException en el hilo que lanzo la task debemos esperar que finalice la ejecución mediante wait()
             */

            //######## Manejando TaskCanceledException  ########

            //Alternativa 1 ==> MAL!
            //NO manejamos la excepción debido a que la exception se da en otro hilo.
            //Por lo tanto NUNCA salta el catch, el hilo queda en estado Cancelado por el ThrowIfCancellationRequested
            //pero NO se maneja la exception.
            //try
            //{
            //    LongRunningTask = Task.Run(() =>
            //    {
            //        DoLongRunningTask(CT);
            //    }, CT);

            //}
            //catch (Exception ex)
            //{

            //}

            //Alternativa2 ==> MAL!
            //En este caso manejamos la exception en el misma Task, si NOSOTROS manejamos la exception
            //Entonces NO lo hace la bibilioteca Task Parallel Library ==> La Task NO queda como Cancelada!
            //Porque TPL maneja la exception en el hilo que invoco la task!
            //Si la exception NO llega en este caso al main thread no se cancela.

            //LongRunningTask = Task.Run(() =>
            //{
            //    try
            //    {
            //        DoLongRunningTask(CT);
            //    }
            //    catch (Exception ex)
            //    {
            //    }

            //}, CT);

            //Alternativa 3 ==> NO !
            /*
             * Conceptualmente esta bien: el HILO principal tiene que esperar a terminar el LongRunningTask con wait
             * y luego manejamos la exception. Pero aca tenemos un deadlock.
             * El Hilo principal manda a ejecutar el LongRunningTask ==> Lo encola
             * El hilo principal continua y se queda a la espera que LongRunningTask finalice la ejecución. Hilo principal BLOQUEADO.
             * El Hilo 2 intenta utilizar el metodo AddMessage() ==> Que en un punto tiene un Dispatcher.Invoke() para decirle al hilo principal que ejecuta una operación.
             * Pero el thread principal esta BLOQUEADO ! ==> Entonces el hilo 2 le dice al hilo principal hace esto! pero el hilo principal le dice estoy bloqueado esperandote
             * y no puedo hacer nada. Entonces Hilo2 espera al main thread ==> main thread espera al hilo2 ==> ambos se esperan eternamente ==> DEADLOCK!
             */
            //LongRunningTask = Task.Run(() =>
            //{
            //    DoLongRunningTask(CT);
            //}, CT);

            //try
            //{
            //    LongRunningTask.Wait();
            //}
            //catch (Exception)
            //{
            //}

            //Alternativa 4 ==> OK
            // Para manejar la TaskCanceledException en el hilo que lanzo la task debemos esperar que finalice 
            // la ejecución mediante wait()
            Task.Run(() =>
            {
                LongRunningTask = Task.Run(() =>
                {
                    DoLongRunningTask(CT);
                }, CT);

            //Controlamos la exception en el thread que invoco la task LongRunningTask
                try
                {
                    LongRunningTask.Wait();
                }
                catch (AggregateException ex)
                {
                    foreach (var inner in ex.InnerExceptions)
                    {
                        if (inner is TaskCanceledException)
                        {
                            AddMessage("Tarea Cancelada y TaskCanceledException manejada.");
                        }
                        else
                        {
                            AddMessage(inner.Message);
                        }
                    }
                }
            });
        }

        private void DoLongRunningTask(CancellationToken ct)
        {
            List<int> ids = new List<int>() { 1, 3, 4, 7, 11, 18, 29, 47, 76, 100 };

            /*
             * Si se solicita una cancelación entonces se activa ThrowIfCancellationRequested() del cancellation token.
             * Lanzando un OperationCanceledException.
             * La biblioteca Task Parallel cuando se produce una OperationCanceledException:
             * 1) Examina el CT para ver si efectivamente se pidio una cancelación. Si es así:
             * 1.1) Maneja la OperationCanceledException
             * 1.2) Establece el status de la task en Canceled
             * 1.3) Lanza una: TaskCanceledException empaquetada en una AggregateException
             * 1.4) Si NO manejamos la exception y luego consultamos el status mostrara ==> Canceled
             */

            //try
            //{
            foreach (var value in ids)
            {

                ct.ThrowIfCancellationRequested();
                AddMessage($"Procesando ID: {value} - DoLongRunningTask");
                Thread.Sleep(2000);
                //AddMessage("Proceso cancelado correctamente");
            }

            //}
            //catch (TaskCanceledException ex)
            //{

            //    if (ct.IsCancellationRequested)
            //    {
            //        //Lógica para finalizar la tarea
            //        AddMessage("Proceso cancelado");
            //    }
            //}
        }

        private void CancelTask_Click(object sender, RoutedEventArgs e)
        {
            //Solicitamos cancelación
            CTS.Cancel();
        }

        private void ShowStatus_Click(object sender, RoutedEventArgs e)
        {
            AddMessage($"Status de la tarea: { LongRunningTask.Status }");
        }

        #endregion
    }
}
