using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlexSpace.DAL
{
    public class Puesto
    {
        public int Id { get; set; }
        public string Codigo { get; set; } = string.Empty;
        public string TipoPuesto { get; set; } = string.Empty;
        public decimal TarifaBasePorHora { get; set; }
    }
}
