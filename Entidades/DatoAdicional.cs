using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WSMTXCA_SRV.Entidades
{
    class DatoAdicional
    {
        public short t { get; private set; }
        public String c1 { get; private set; }
        public String c2 { get; private set; }
        public String c3 { get; private set; }
        public String c4 { get; private set; }
        public String c5 { get; private set; }
        public String c6 { get; private set; }

        public DatoAdicional(short t, String c1, String c2 = "", String c3 = "", String c4 = "", String c5 = "", String c6 = "")
        {
            this.t = t;
            this.c1 = c1;
            this.c2 = c2;
            this.c3 = c3;
            this.c4 = c4;
            this.c5 = c5;
            this.c6 = c6;
        }
    }
}
