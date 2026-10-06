using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlexSpace.DAL
{
    public class Reserva
    {
        public int Id { get; set; }
        public int ClienteId { get; set; }
        public int PuestoId { get; set; }
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }
        public string Estado { get; set; } = "Confirmada"; 
        public decimal CostoTotal { get; set; }
    }
}