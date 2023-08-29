using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WSMTXCA_SRV.Entidades
{
    class Detalle
    {
        public Producto prod { get; set; }
        public int unidadesMX { get; set; }
        public decimal cantidad { get; set; }
        public decimal peso { get; set; }
        public decimal bultos { get; set; }
        public decimal precio { get; set; }
        public decimal porcBonif { get; set; }
        public decimal bonif { get; set; }
        public short codIva { get; set; }
        public decimal importeIva { get; set; }
        public decimal totalItem { get; set; }
    }
}
