using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Threading.Tasks;

namespace Ensamblador
{
    public class Mensaje
    {

        public Mensaje(string _contenido, string _nomrbre,bool salto)
        {
            Contenido = _contenido;
            nombre = _nomrbre;
            Salto = salto;
        }
        public string Contenido{get; set;}
        public string nombre {get; set;}

        public bool Salto {get; set;}
        
    }
}