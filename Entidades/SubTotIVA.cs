using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WSMTXCA_SRV.Entidades
{
    class SubTotIVA
    {
        public short codIva { get; private set; }
        public decimal importe { get; private set; }

        public SubTotIVA(short codIva, decimal importe)
        {
            this.codIva = codIva;
            this.importe = importe;
        }
    }
}
