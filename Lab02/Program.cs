using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Lab02
{
    class Program
    {
        static void Main(string[] args)
        {
            //RunParallelTasks();
            //ParallelLoopIterate();
            //RunLinq();

            //RunContinuesTasks();

            //RunNestedTasks();
            //RunChildTasks();
            HandledTaskExceptions();
            Console.WriteLine("Presiones <enter> para finalizar.");
            Console.ReadLine();
        }

        private static void HandledTaskExceptions()
        {
            /*
             * Si una tarea produce una exception, esta se propaga al hilo que genero dicha tarea.
             * Las biblioteca TaskParallel empaqueta todas las exceptions en un AggregateException y propaga esto al hilo que genero la tarea.
             * Para capturar exceptions tenemos que esperar que finalice con Wait() y luego en un bloque try catch capturamos AggregateException.
             */

            var tokenSource = new CancellationTokenSource();
            var token = tokenSource.Token;

            var longRunningTask = Task.Run(() => RunLongTask(token), token);

            //Cancelamos tarea
            tokenSource.Cancel();

            try
            {
                longRunningTask.Wait();
            }
            catch (AggregateException ex)
            {
                foreach (var inner in ex.InnerExceptions)
                {
                    if(inner is TaskCanceledException)
                    {
                        Console.WriteLine("TaskCanceledException - Tarea Cancelada!!");
                    }
                    else
                    {
                        //Exceptions distintas a la cancelación
                        Console.WriteLine(inner.Message);
                    }
                }
            }
            catch (Exception)
            {
                Console.WriteLine("General Exception!!!");
            }

        }

        static void RunLongTask(CancellationToken token)
        {
            for (int i = 0; i < 5; i++)
            {
                Console.WriteLine($"{i} iteration from RunLongTask");
                //Simular proceso largo
                Thread.Sleep(1000);
                //Lanzar un OperationCanceledException si se solicita una cancelación
                token.ThrowIfCancellationRequested();
            }
        }

        private static void RunChildTasks()
        {
            /*
              * Tareas Hijas
              * Basicamente son tareas anidadas pero DEPENDIENTES
              * Le podemos decir al parent que espere a las hijas para finalizar utilizando: AttachedToParent al crear la tarea hija.
              * De esta forma tamb las tareas hijas pueden propagar sus exceptions en el parent.
              */

            var outerTask = Task.Factory.StartNew(() =>
            {
                Console.WriteLine("Iniciando tarea ######## Externa ########");

                var innerTask = Task.Factory.StartNew(() =>
                {
                    Console.WriteLine("Iniciando tarea ######## Interna ########");
                    //Simulamos proceso largo
                    Thread.Sleep(6000);
                    Console.WriteLine("Fin tarea ######## Interna ########");
                }, TaskCreationOptions.AttachedToParent);


            });
            outerTask.Wait();
            Console.WriteLine("Fin tarea ######## Externa ########");
        }

        private static void RunNestedTasks()
        {
            /*
             * Tareas Anidadas
             * La tarea externa y las tareas internas son indepedientes.
             * A un nivel que las tareas internas pueden terminar antes que la externa y viceversa.
             * La tarea externa no necesita esperar a las internas.
             */

            var outerTask = Task.Factory.StartNew(() =>
            {
                Console.WriteLine("Iniciando tarea ######## Externa ########");

                var innerTask = Task.Factory.StartNew(() =>
                {
                    Console.WriteLine("Iniciando tarea ######## Interna ########");
                    //Simulamos proceso largo
                    Thread.Sleep(6000);
                    Console.WriteLine("Fin tarea ######## Interna ########");
                });


            });
            outerTask.Wait();
            Console.WriteLine("Fin tarea ######## Externa ########");

        }

        private static void RunContinuesTasks()
        {
            //Continuation Tasks
            var firsTask = new Task<List<string>>(GetProductNames);

            //Si la tarea termina exitosamente entonces procesamos los nombres

            var secondTask = firsTask.ContinueWith(antecedent =>
            {
                return ProcessData(antecedent.Result);
            });

            //Comienzo la primera tarea que tiene seteada la continuación con la segunda task (ProccessData) al terminar
            firsTask.Start();

            Console.WriteLine($"Result { secondTask.Result }");
        }

        static int ProcessData(List<string> productNames)
        {
            //Simulamos procesamiento
            foreach (var name in productNames)
            {
                Console.WriteLine(name);
            }

            return productNames.Count;
        }
        static List<string> GetProductNames()
        {
            Thread.Sleep(3000);
            return NorthWind.Repository.Products.Select(p => p.ProductName).ToList();
        }



        private static void RunLinq()
        {
            var timer = Stopwatch.StartNew();

            var products = NorthWind.Repository.Products.Select(p =>new ProductoDto()
            {
                ProductID = p.ProductID,
                ProductName = p.ProductName,
                UnitPrice = p.UnitPrice,
                UnitsInStock = p.UnitsInStock

            }).ToList();

            //Tick = 100 nanosegundos
            Console.WriteLine($"[Sequentially - LINQ]Calculate Square WithOut Parallel: { timer.ElapsedTicks } Ticks");
            timer.Stop();

            /*
             * Utilizamos Parallel LINQ (PLINQ)
             */

            var timerParallel = Stopwatch.StartNew();

            var productsParallel = NorthWind.Repository.Products.AsParallel().Select(p => new ProductoDto()
            {
                ProductID = p.ProductID,
                ProductName = p.ProductName,
                UnitPrice = p.UnitPrice,
                UnitsInStock = p.UnitsInStock

            }).ToList();

            timerParallel.Stop();

            //Tick = 100 nanosegundos
            Console.WriteLine($"[Parallel - LINQ]Calculate Square With Parallel: { timerParallel.ElapsedTicks } Ticks");

            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.WriteLine($"PLINQ ha sido más rápido en: {  timer.ElapsedTicks - timerParallel.ElapsedTicks } ticks");

        }

        private static void ParallelLoopIterate()
        {
            CalculaCuadrado();
        }

        private static void CalculaCuadrado()
        {
            var _INDEX_ = 300000000;
            int[] squareNumbers = new int[_INDEX_];

            /*
             * Las tareas son ejecutadas en paralelo pero no hay garantia de orden.
             * Para tareas simples y pocas iteraciones secuencialmente no esta mal, es más raspido que parallel
             *
             *
             * 
             */

            var timer = Stopwatch.StartNew();
            timer.Start();

            Parallel.For(0, _INDEX_, i =>
             {
                 squareNumbers[i] = i * i;
                 //Console.WriteLine($"[Parallel] Calculando el Cuadrado de { i }");

             });

            timer.Stop();

            Console.WriteLine($"[Parallel]Calculate Square With Parallel: { timer.ElapsedMilliseconds }");

            var timer1 = Stopwatch.StartNew();
            int[] aux = new int[_INDEX_];

            for (int i = 0; i < _INDEX_; i++)
            {
                aux[i] = i * i;
                //Console.WriteLine($"[Sequentially] Calculando el Cuadrado de { i }");
            }

            timer1.Stop();
            Console.WriteLine($"[Sequentially] Calculate Square Without Parallel: { timer1.ElapsedMilliseconds }");

            //Parallel.ForEach(squareNumbers, n =>
            //{
            //    //Console.WriteLine($"Cuadrado de { Array.IndexOf(squareNumbers, n) } : {n}");
            //});


        }

        private static void RunParallelTasks()
        {
            Console.WriteLine($"Thead Principal:  { Thread.CurrentThread.ManagedThreadId } - Ejecutar tareas en paralelo");

            /*
             * Parallel clase estatica que nos permiten ejecutar tareas en forma simultanea.
             */

            /*
             * Realizamos 3 tareas en Paralelo ==> No existe garantia de orden.
             * El metodo Invoke NO retorna hasta que las 3 tareas hayan FINALIZADO (Ya sea de forma norma o por exception)
            */
            Parallel.Invoke(
                () => { WriteToConsole("Tarea 1"); },
                () => { WriteToConsole("Tarea 2"); },
                () => { WriteToConsole("Tarea 3"); }
                );
        }

        private static void WriteToConsole(string message)
        {
            Console.WriteLine($"{ message } - Current Thread: { Thread.CurrentThread.ManagedThreadId }");
            Thread.Sleep(5000); //Simular un proceso de larga duración
            Console.WriteLine($"Fin de Tarea { Thread.CurrentThread.ManagedThreadId }");
        }
    }
}
