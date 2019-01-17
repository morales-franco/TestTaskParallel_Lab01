using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Lab02
{
    class ProductoDto
    {
        public ProductoDto()
        {
            //Simular proceso de larga duración
            Thread.Sleep(1);
        }

        public int ProductID { get; set; }
        public string ProductName { get; set; }
        public decimal? UnitPrice { get; set; }
        public decimal? UnitsInStock { get; set; }

    }
}
