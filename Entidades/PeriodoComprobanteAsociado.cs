using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WSMTXCA_SRV.Entidades
{
    class PeriodoComprobanteAsociado
    {
        public DateTime desde;
        public DateTime hasta;

        public PeriodoComprobanteAsociado(DateTime desde, DateTime hasta)
        {
            this.desde = desde;
            this.hasta = hasta;
        }

    }
}
