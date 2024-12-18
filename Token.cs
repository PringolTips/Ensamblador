using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ensamblador
{
    public class Token
    {
        public enum Tipos
        {
            Identificador, Numero, FinSentencia, OpTermino, OpFactor,
            OpLogico, OpRelacional, OpTernario, Asignacion, IncTermino,
            IncFactor, Cadena, Inicio, Fin, Caracter, TipoDato, Ciclo, 
            Condicion
            
        };
        private string contenido;
        private Tipos clasificacion;
        public Token()
        {
            Contenido = "";
        }
         public string Contenido
        {
            get => contenido;
            set => contenido = value;
        }
        public Tipos Clasificacion
        {
            get => clasificacion; 
            set => clasificacion = value;
        }

    }
}