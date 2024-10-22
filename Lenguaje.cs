using System;
using System.Collections.Generic;
using System.Formats.Asn1;
using System.IO.Pipes;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
/*
    El proyecto genera código ASM en: nasm o masm o .... excepto emu8086
    1. Completar la asignación
    2. Console.Write & Console.WriteLine
    3. Console.Read & Console.ReadLine
    4. Considerar el else en el if 
    5. Programar el while 
    6. Programar el for
*/
//Estoy conectado
namespace Ensamblador
{
    public class Lenguaje : Sintaxis
    {
        private List<Variable> listaVariables;

        private int contIf, contDo, cWhiles;
       // private Variable.TipoD tipoDatoExpresion;

        public Lenguaje(String nombre = "prueba.cpp")
        {
            log.WriteLine("Analisis Sintactico");
            asm.WriteLine(";Analisis Sintactico");
            listaVariables = new List<Variable>();
            contIf = contDo = 1;
        }
        Variable.TipoD getTipo(String TipoDato)
        {
            Variable.TipoD Tipo = Variable.TipoD.Char;
            switch (TipoDato)
            {
                case "int": Tipo = Variable.TipoD.Int; break;
                case "float": Tipo = Variable.TipoD.Float; break;
            }
            return Tipo;
        }
        private void imprimeVariables()
        {
            log.WriteLine("Lista de variables");
            asm.WriteLine("\nsegment .data");
            foreach (Variable v in listaVariables)
            {
                log.WriteLine(v.nombre + " (" + v.tipo + ") = " + v.valor);
                if (v.tipo == Variable.TipoD.Char)
                {
                    asm.WriteLine("\t" + v.nombre + " db 0");
                }
                else if (v.tipo == Variable.TipoD.Int)
                {
                    asm.WriteLine("\t" + v.nombre + " dd 0");
                }
                else
                {
                    asm.WriteLine("\t" + v.nombre + " dw 0 ");
                }
            }
        }
        //Programa  -> Librerias? Variables? Main
        public void Programa()
        {
            if (Contenido == "using")
            {
                Librerias();
            }
            Main();
            imprimeVariables();
        }
        //Librerias -> using ListaLibrerias; Librerias?
        private void Librerias()
        {
            match("using");
            ListaLibrerias();
            match(";");
            if (Contenido == "using")//Caso base de la recursividad
            {
                Librerias();
            }
        }
        //ListaLibrerias -> identificador (.ListaLibrerias)?
        private void ListaLibrerias()
        {
            match(Tipos.Identificador);
            if (Contenido == ".")
            {
                match(".");
                ListaLibrerias();
            }
        }
        //Variables -> tipo_dato Lista_identificadores; Variables?
        private void Variables()
        {
            Variable.TipoD tipoDato = getTipo(Contenido);
            match(Tipos.TipoDato);
            ListaIdentificadores(tipoDato);
            match(";");
        }
        //ListaIdentificadores -> identificador (,ListaIdentificadores)?
        private void ListaIdentificadores(Variable.TipoD t)
        {
            listaVariables.Add(new Variable(Contenido, t));
            match(Tipos.Identificador);

            if (Contenido == "=")
            {
                match("=");
                Expresion();
            }
            if (Contenido == ",")
            {
                match(",");
                ListaIdentificadores(t);
            }
        }
        //BloqueInstrucciones -> { listaIntrucciones? }
        private void BloqueInstrucciones()
        {
            match("{");
            if (Clasificacion != Tipos.Fin)
            {
                ListaInstrucciones();
            }
            match("}");
        }
        //ListaInstrucciones -> Instruccion ListaInstrucciones?
        private void ListaInstrucciones()
        {
            Instruccion();
            if (Contenido != "}")
            {
                ListaInstrucciones();
            }
        }
        //Instruccion -> Console | If | While | do | For | Asignacion
        private void Instruccion()
        {
            if (Contenido == "Console")
            {
                console();
            }
            else if (Contenido == "if")
            {
                If();
            }
            else if (Contenido == "while")
            {
                While();
            }
            else if (Contenido == "do")
            {
                Do();
            }
            else if (Contenido == "for")
            {
                For();
            }
            else if (Clasificacion == Tipos.TipoDato)
            {
                Variables();
            }
            else
            {
                Asignacion();
                match(";");
            }
        }
        private bool ExisteVariable(string variableNombre)
        {
            return listaVariables.Exists(v => v.nombre == variableNombre);
        }
        //Identificador = Expresion;
        //            -> Identificador (++ | --); INC o DEC
        //            -> Identificador (+= | -= ) Expresion; PUSH o POP
        //            -> Identificador (*= | /= | %= ) Expresion; PUSH o POP
        private void Asignacion()
        {
            String variable = Contenido;
            match(Tipos.Identificador);
            asm.WriteLine(";Asignacion a " + variable);
            var v = listaVariables.Find(delegate (Variable x) { return x.nombre == variable; });
            float nuevoValor = v.valor;

            if (Contenido == "++")
            {
                match("++");
                asm.WriteLine("\tinc dword [" + variable + "]");
                nuevoValor++;
            }
            else if (Contenido == "--")
            {
                match("--");
                asm.WriteLine("\tdec " + variable);
                nuevoValor--;
            }
            else if (Contenido == "+=")
            {
                match("+=");
                Expresion();
                asm.WriteLine("\tpop eax");
            }
            else if (Contenido == "-=")
            {
                match("-=");
                Expresion();
                asm.WriteLine("\tpop eax");
            }
            else if (Contenido == "*=")
            {
                match("*=");
                Expresion();
                asm.WriteLine("\tpop eax");
            }
            else if (Contenido == "/=")
            {
                match("/=");
                Expresion();
                asm.WriteLine("\tpop eax");
            }
            else if (Contenido == "%=")
            {
                match("%=");
                Expresion();
                asm.WriteLine("\tpop eax");
            }
            else if (Contenido == "=")
            {
                match("=");
                if (Contenido == "Console")
                {
                    match("Console");
                    match(".");
                    if (Contenido == "ReadLine")
                    {
                        match("ReadLine");
                        match("(");
                        match(")");

                    }
                    else if (Contenido == "Read")
                    {
                        match("Read");
                        match("(");
                        match(")");
                    }
                }
                else
                {
                    Expresion();
                    asm.WriteLine("\tpop eax");
                    asm.WriteLine("\tmov dword [" + variable + "], eax");
                }
            }
            v.valor = nuevoValor;
            //asm.WriteLine("\tpush");
            
            //log.WriteLine(variable + " = " + nuevoValor);
            asm.WriteLine("; Termina asignacion a " + variable);
        }
        
        //If -> if (Condicion) bloqueInstrucciones | instruccion
        //     (else bloqueInstrucciones | instruccion)?
        private void If()
        {
            asm.WriteLine("; if " + contIf);
            string etiqueta = "_if" + contIf++;
            match("if");
            match("(");
            Condicion(etiqueta);
            match(")");
            if (Contenido == "{")
            {
                BloqueInstrucciones();
            }
            else
            {
                Instruccion();
            }
            if (Contenido == "else")
            {
                match("else");
                if (Contenido == "{")
                {
                    BloqueInstrucciones();
                }
                else
                {
                    Instruccion();
                }
            }
            asm.WriteLine(etiqueta + ":");
            //Generar etiqueta
        }
        //Condicion -> Expresion operadorRelacional Expresion
        private void Condicion(string etiqueta)
        {
            Expresion();
            String operador = Contenido;
            match(Tipos.OpRelacional);
            Expresion();
            asm.WriteLine("\tpop eax");
            asm.WriteLine("\tpop ebx");
            asm.WriteLine("\tcmp eax, ebx");
            switch (operador)
            {
                case ">": break;
                case ">=": break;
                case "<": break;
                case "<=": break;
                case "==":
                    asm.WriteLine("\tjne " + etiqueta);
                    break;
                default:
                    asm.WriteLine("\tje " + etiqueta);
                    break;
            }
        }
        //While -> while(Condicion) bloqueInstrucciones | instruccion
        private void While()
        {
            asm.WriteLine("; while " + ++cWhiles);
            string etiquetaIni = "_whileIni" + cWhiles; 
            string etiquetaFin = "_whileFin" + cWhiles; 
            match("while");
            match("(");
            asm.WriteLine(etiquetaIni + ":");
            Condicion(etiquetaFin);
            match(")");
            if (Contenido == "{")
            {
                BloqueInstrucciones();
            }
            else
            {
                Instruccion();
            }
            asm.WriteLine("jmp "+etiquetaIni);
            asm.WriteLine(etiquetaFin + ":");
        }
        //Do -> do 
        //        bloqueInstrucciones | intruccion 
        //      while(Condicion);
        private void Do()
        {
            asm.WriteLine("do " + contDo);
            string etiqueta = "_do" + contDo++;
            asm.WriteLine(etiqueta + ";");
            match("do");
            if (Contenido == "{")
            {
                BloqueInstrucciones();
            }
            else
            {
                Instruccion();
            }
            match("while");
            match("(");
            Condicion(etiqueta);
            match(")");
            match(";");
        }
        //For -> for(Asignacion Condicion; Incremento) 
        //       BloqueInstrucciones | Intruccion 
        private void For()
        {
            match("for");
            match("(");
            Asignacion();
            match(";");
            Condicion("");
            match(";");
            Asignacion();
            match(")");
        }
        //Incremento -> Identificador ++ | --
        private void Incremento()
        {
            match(Tipos.Identificador);
            if (Contenido == "++")
            {
                match("++");
            }
            else
            {
                match("--");
            }

        }
        //Console -> Console.(WriteLine|Write) (cadena); |
        //           Console.(Read | ReadLine) ();
        private void console()
        {
            match("Console");
            match(".");
            if (Contenido == "WriteLine")
            {
                match("WriteLine");
            }
            else
            {
                match("Write");
            }
            match("(");
            if (Clasificacion == Tipos.Cadena)
            {
                match(Tipos.Cadena);

                if (Contenido == "+")
                {
                    listaConcatenacion();

                }

                else
                {
                    if (Contenido == "+")
                    {
                        listaConcatenacion();
                    }
                }
            }
            else
            {
                match(Tipos.Identificador);
                if (Contenido == "+")
                {
                    listaConcatenacion();
                }
                else
                {
                    if (Contenido == "+")
                    {
                        listaConcatenacion();

                    }
                }
            }
            match(")");
            match(";");
        }
        private void listaConcatenacion()
        {
            match("+");
            if (Clasificacion == Tipos.Identificador)
            {
                if (!ExisteVariable(Contenido))
                {
                    throw new Error("Semantico: la variable no existe: " + Contenido, log, linea);
                }
                var v = listaVariables.Find(variable => variable.nombre == Contenido);
                match(Tipos.Identificador);
            }
            if (Clasificacion == Tipos.Cadena)
            {
                match(Tipos.Cadena);
            }
            if (Contenido == "+")
            {
                listaConcatenacion();
            }
        }
        private void asm_Main()
        {
            asm.WriteLine();
            asm.WriteLine("extern fflush");
            asm.WriteLine("extern printf");
            asm.WriteLine("extern scanf");
            asm.WriteLine("extern stdout");
            asm.WriteLine("\nsegment .text");
            asm.WriteLine("\tglobal _main");
            asm.WriteLine("\n_main:");
        }
        private void asm_endMain()
        {
            asm.WriteLine("\tadd esp, 4\n");
            asm.WriteLine("\tmov eax, 1");
            asm.WriteLine("\txor ebx, ebx");
            asm.WriteLine("\tint 0x80");
        }
        //Main      -> static void Main(string[] args) BloqueInstrucciones 
        private void Main()
        {
            asm_Main();
            match("static");
            match("void");
            match("Main");
            match("(");
            match("string");
            match("[");
            match("]");
            match("args");
            match(")");
            BloqueInstrucciones();
            asm_endMain();
        }
        //Expresion -> Termino MasTermino
        private void Expresion()
        {
            Termino();
            MasTermino();
        }
        //MasTermino -> (OperadorTermino Termino)?
        private void MasTermino()
        {
            if (Clasificacion == Tipos.OpTermino)
            {
                string operador = Contenido;
                match(Tipos.OpTermino);
                Termino();
                  asm.WriteLine("\tpop ebx");
                asm.WriteLine("\tpop eax");
                switch (operador)
                {
                    case "+":
                        asm.WriteLine("\tadd eax, ebx");
                        asm.WriteLine("\tpush eax");
                        break;
                    case "-":
                        asm.WriteLine("\tsub eax, ebx");
                        asm.WriteLine("\tpush eax");
                        break;
                }
            }
        }
        //Termino -> Factor PorFactor
        private void Termino()
        {
            Factor();
            PorFactor();
        }
        //PorFactor -> (OperadorFactor Factor)?
        private void PorFactor()
        {
            if (Clasificacion == Tipos.OpFactor)
            {
                string operador = Contenido;
                match(Tipos.OpFactor);
                Factor();
                asm.WriteLine("\tpop ebx");
                asm.WriteLine("\tpop eax");
                switch (operador)
                {
                    case "*":
                        asm.WriteLine("\tmul ebx");
                        asm.WriteLine("\tpush eax");
                        break;
                    case "/":
                        asm.WriteLine("\tdiv ebx");
                        asm.WriteLine("\tpush eax");
                        break;
                    case "%":
                        asm.WriteLine("\tdiv ebx");
                        asm.WriteLine("\tpush edx");
                        break;;
                }
            }
        }
        //Factor -> numero | identificador | (Expresion)
        private void Factor()
        {
            if (Clasificacion == Tipos.Numero)
            {
               asm.WriteLine("\tmov eax, " + Contenido);
                asm.WriteLine("\tpush eax");
                match(Tipos.Numero);
            }
            else if (Clasificacion == Tipos.Identificador)
            {
               var v = listaVariables.Find(delegate (Variable x) { return x.nombre == Contenido; });
                asm.WriteLine("\tmov eax, " + Contenido);
                asm.WriteLine("\tpush eax");
                match(Tipos.Identificador);
            }
             else
            {
                match("(");
                Expresion();
                match(")");
            }
        }
    }
}