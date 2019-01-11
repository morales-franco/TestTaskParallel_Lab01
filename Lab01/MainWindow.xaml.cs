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
            CreateTask();
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

            //Con la opción Dispatcher le decis que ejecute el codigo que le pasas desde el hilo principal.
            //Por esta razon tomamamos el ManagedThreadId fuera del Dispacher porque sino ManagedThreadId siempre seria el principal Thread
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
                $"Mensaje: {message}" +
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
            //Task.WaitAll Espera que finalicen todas las tareas lanzadas en el array de task
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
            //Task.WaitAll Espera que finalicen todas las tareas lanzadas en el array de task
            Task.WaitAny(taskGroup);
            WriteToOutput($"ALGUNA las tareas del Group han Finalizado - Segundos Now: { DateTime.Now.Second }");
        }

        void ReturnTaskValue()
        {
            Task<int> T;

            T = Task.Run<int>(() => new Random().Next(1000));

            //Cuando el Main Thread intenta acceder a la propiedad Result y la task aun NO ha terminado
            //Entonces el Main Thread espera hasta que el Resultado este disponible.
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
    }
}
