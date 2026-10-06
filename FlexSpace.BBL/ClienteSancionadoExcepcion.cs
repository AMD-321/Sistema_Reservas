using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlexSpace.BLL
{
    public class ClienteSancionadoException : Exception
    {
        public ClienteSancionadoException(string mensaje) : base(mensaje)
        {
        }
    }
}