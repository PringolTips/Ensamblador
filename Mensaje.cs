using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Threading.Tasks;

namespace Ensamblador
{
    public class Mensaje
    {

        public Mensaje(string _contenido, string _nomrbre)
        {
            Contenido = _contenido;
            nombre = _nomrbre;
        }
        public string Contenido{get; set;}
        public string nombre {get; set;}
        
    }
}