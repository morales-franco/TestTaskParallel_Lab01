using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Lab04
{
    class Product
    {
        private object _objectToLock = new object();

        int _unitInStock;

        public Product(int initialUnitInStock)
        {
            _unitInStock = initialUnitInStock;
        }

        public bool PlaceOrder(int requestedUnits)
        {
            bool accepted = false;

            if (_unitInStock < 0)
            {
                throw new Exception("No puede existir Stock NEGATIVO");
            }

            //Bloqueo de exlusión mutua
            lock (_objectToLock)
            {
                //Sección critica
                if (_unitInStock >= requestedUnits)
                {
                    //Simulamos proceso de larga duración
                    Thread.Sleep(3000);
                    Console.WriteLine($"Stock antes del pedido: { _unitInStock }");

                    //Restamos cantidad solicitada
                    _unitInStock -= requestedUnits;
                    Console.WriteLine($"Stock despues del pedido: { _unitInStock }");
                    accepted = true;
                }
                else
                {
                    Console.WriteLine($"No hay stock suficiente");
                }
            }
            return accepted;

        }

    }
}
