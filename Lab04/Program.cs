using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab04
{
    class Program
    {
        static void Main(string[] args)
        {
            PlaceOrders();


        }



        #region Bloqueos de exclusión Mutua (mutual-exclusion)- lock

        /*
         * Mutual-exclusion
         * utilizamos la palabra reservada lock para aplicar un bloqueo de exclusión mutua
         * Este bloqueo hace que solo 1 hilo pueda acceder a la sección critica.
         * Es decir cuando un hilo acceda a la sección critica, los demás hilos seran bloqueados hasta que se libere la misma.
         * 
         * El lock se realiza contra un objeto que deberia ser privado y solamente utilizarse para esto.
         * Todo lo que este dentro del bloque lock será la sección critica que solo podra ser accedida por un unico thread.
         * 
         * Ver ejemplo en class Product
         * 
         *  private object _objectToLock = new object();
         *  
         *  lock (_objectToLock)
            {
                //Sección critica
            }
         * 
         */

        private static void PlaceOrders()
        {
            var product = new Product(1000);
            var r = new Random();

            //Simulamos 100 operaciones en paralelo

            Parallel.For(0, 100, index =>
             {
                 product.PlaceOrder(r.Next(1, 100));
             });

            Console.WriteLine();

        }

        #endregion

    }
}
