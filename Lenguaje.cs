using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Formats.Asn1;
using System.IO.Pipes;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
/*
    El proyecto genera código ASM en: nasm o masm o .... excepto emu8086
    1. Completar la asignación                                              -listo
    2. Console.Write & Console.WriteLine                                    -concatenacion
    3. Console.Read & Console.ReadLine                                      -Listo  
    4. Considerar el else en el if --                                       - Listo
    5. Programar el while                                                   - Listo
    6. Programar el for                                                     - Listo
*/
//Estoy conectado
namespace Ensamblador
{
    public class Lenguaje : Sintaxis
    {
        private List<Variable> listaVariables;
        private List<Mensaje> msg;

        private int contIf, contDo, cWhiles, cCadena, contFor;
        // private Variable.TipoD tipoDatoExpresion;

        public Lenguaje(String nombre = "prueba.cpp")
        {
            log.WriteLine("Analisis Sintactico");
            asm.WriteLine(";Analisis Sintactico");
            listaVariables = new List<Variable>();
            msg = new List<Mensaje>();
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
            asm.WriteLine("\nformat db " + '"' + "%d" + '"' + ", 0");
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
            foreach (Mensaje v in msg)
            {
                    asm.WriteLine("\t" + v.nombre + " db '" + v.Contenido + "' ,0");
            }
            asm.WriteLine("\tsalto db "+"''"+", 10, 0");
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
            string variable = Contenido;
            match(Tipos.Identificador);

            if (Contenido == "=")
            {
                match("=");
                Expresion();
                asm.WriteLine("\tpop eax");
                asm.WriteLine("\tmov dword [" + variable + "], eax");
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

            if (Contenido == "++")
            {
                match("++");
                asm.WriteLine("\tinc dword [" + variable + "]");
            }
            else if (Contenido == "--")
            {
                match("--");
                asm.WriteLine("\tdec dword [" + variable + "]");
            }
            else if (Contenido == "+=")
            {
                match("+=");
                Expresion();
                asm.WriteLine("\tpop eax");
                asm.WriteLine("\tadd [" + variable + "], eax");

            }
            else if (Contenido == "-=")
            {
                match("-=");
                Expresion();
                asm.WriteLine("\tpop eax");
                asm.WriteLine("\tsub [" + variable + "], eax");
            }
            else if (Contenido == "*=")
            {
                match("*=");
                Expresion();
                asm.WriteLine("\tpop eax");
                asm.WriteLine("\tmov ebx, " + "[" + variable + "]");
                asm.WriteLine("\tmul ebx");
                asm.WriteLine("\txor ebx,ebx");
                asm.WriteLine("\tmov dword [" + variable + "], eax");

            }
            else if (Contenido == "/=")
            {
                match("/=");
                Expresion();
                asm.WriteLine("\tpop eax");
                asm.WriteLine("\tmov ecx, eax");
                asm.WriteLine("\tmov eax, " + "[" + variable + "]");
                asm.WriteLine("\txor edx,edx");
                asm.WriteLine("\tdiv ecx");
                asm.WriteLine("\tmov dword [" + variable + "], eax");

            }
            else if (Contenido == "%=")
            {
                match("%=");
                Expresion();
                asm.WriteLine("\tpop eax");
                asm.WriteLine("\tmov ecx, eax");
                asm.WriteLine("\tmov eax, " + "[" + variable + "]");
                asm.WriteLine("\txor edx,edx");
                asm.WriteLine("\tdiv ecx");
                asm.WriteLine("\tmov dword [" + variable + "], edx");

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
                        asm.WriteLine("push " + variable);
                        asm.WriteLine("push format");
                        asm.WriteLine("call scanf");
                    }
                    else if (Contenido == "Read")
                    {
                        match("Read");
                        match("(");
                        match(")");
                        asm.WriteLine("push " + variable);
                        asm.WriteLine("push format");
                        asm.WriteLine("call scanf");
                    }
                }
                else
                {
                    Expresion();
                    asm.WriteLine("\tpop eax");
                    asm.WriteLine("\tmov dword [" + variable + "], eax");
                }
            }
            //asm.WriteLine("\tpush");

            //log.WriteLine(variable + " = " + nuevoValor);
            asm.WriteLine("; Termina asignacion a " + variable);
        }
        //If -> if (Condicion) bloqueInstrucciones | instruccion
        //     (else bloqueInstrucciones | instruccion)?
        private void If()
        {
            asm.WriteLine("; if " + contIf);
            string etiquetaElse = "_else" + contIf;
            string etiquetaFinIf = "_finIf" + contIf;
            contIf++;

            match("if");
            match("(");
            Condicion(etiquetaElse);
            match(")");

            if (Contenido == "{")
            {
                BloqueInstrucciones();
            }
            else
            {
                Instruccion();
            }
            asm.WriteLine("\tjmp " + etiquetaFinIf);
            asm.WriteLine(etiquetaElse + ":");

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
            asm.WriteLine(etiquetaFinIf + ":");
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
                case ">":
                    asm.WriteLine("\tjge " + etiqueta);
                    break;
                case ">=":
                    asm.WriteLine("\tjg " + etiqueta);
                    break;
                case "<":
                    asm.WriteLine("\tjle " + etiqueta);
                    break;
                case "<=":
                    asm.WriteLine("\tjl " + etiqueta);
                    break;
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
            asm.WriteLine("\tjmp " + etiquetaIni);
            asm.WriteLine(etiquetaFin + ":");
        }
        //Do -> do 
        //        bloqueInstrucciones | intruccion 
        //      while(Condicion);
        private void Do()
        {
            asm.WriteLine("; do-while " + contDo);
            string etiquetaIni = "_doInicio" + contDo;
            string etiquetaFin = "_doFin" + contDo++;

            asm.WriteLine(etiquetaIni + ":");
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

            Condicion(etiquetaFin);

            asm.WriteLine("\tjmp " + etiquetaIni);
            asm.WriteLine(etiquetaFin + ":");

            match(")");
            match(";");
        }
        //For -> for(Asignacion Condicion; Incremento) 
        //       BloqueInstrucciones | Intruccion 
        private void For()
        {
            asm.WriteLine(";for " + ++contFor);
            string etiquetaIni = "_forIni" + contFor;
            string etiquetaFin = "_forFin" + contFor;
            String inc_dec = "";

            match("for");
            match("(");
            Asignacion();
            asm.WriteLine(etiquetaIni + ":");
            match(";");
            Condicion(etiquetaFin);
            match(";");
            inc_dec = Incremento();
            match(")");
            if (Contenido == "{")
            {
                BloqueInstrucciones();
            }
            else
            {
                Instruccion();
            }
            asm.WriteLine(inc_dec);
            asm.WriteLine("\tjmp " + etiquetaIni);
            asm.WriteLine(etiquetaFin + ":");
        }
        //Incremento -> Identificador ++ | --
        private string Incremento()
        {
            string variable = Contenido;
            string tem = "";
            var v = listaVariables.Find(delegate (Variable x) { return x.nombre == variable; });
            match(Tipos.Identificador);
            if (Contenido == "++")
            {
                match("++");
                tem = "\tinc dword [" + variable + "]";
            }
            else
            {
                match("--");
                tem = "\tdec dword [" + variable + "]";
            }
            return tem;
        }
        //Console -> Console.(WriteLine|Write) (cadena); |
        //           Console.(Read | ReadLine) ();
        private void console()
        {
            bool salto = false;
            match("Console");
            match(".");
            if (Contenido == "WriteLine")
            {
                match("WriteLine");
                salto = true;
                
            }
            else
            {
                match("Write");
            }

            string temp = "";
            match("(");
            if (Clasificacion == Tipos.Cadena)
            {
                
                temp = Contenido.Trim('"');
                msg.Add(new Mensaje(temp, "msg" + cCadena));
                match(Tipos.Cadena);
                asm.Write("\tPRINT_STRING ");
                asm.WriteLine("msg" + cCadena);
                cCadena++;
            }
            else
            {
                asm.WriteLine("\tmov eax, [" + Contenido + "]");
                asm.WriteLine("\tpush eax");
                asm.WriteLine("\tpush format");
                asm.WriteLine("\tcall printf");
                match(Tipos.Identificador);
                
            }
            if (Contenido == "+")
            {
                listaConcatenacion();
            }
            match(")");
            match(";");
            if(salto == true)
            {
                asm.WriteLine("\tPRINT_STRING salto");
            }
        }
        private void listaConcatenacion()
        {
            string temp = "";
            match("+");

            if (Clasificacion == Tipos.Identificador)
            {

                var variable = listaVariables.Find(v => v.nombre == Contenido);
                asm.WriteLine("\tmov eax, [" + variable.nombre + "]");
                asm.WriteLine("\tpush eax");
                asm.WriteLine("\tpush format");
                asm.WriteLine("\tcall printf");
                match(Tipos.Identificador);
            }
            else if (Clasificacion == Tipos.Cadena)
            {
                temp = Contenido.Trim('"');
                msg.Add(new Mensaje(temp, "msg" + cCadena));
                match(Tipos.Cadena);
                asm.Write("\tPRINT_STRING ");
                asm.WriteLine("msg" + cCadena);
                cCadena++;
            }
            if (Contenido == "+")
            {
                listaConcatenacion();
            }
        }
        private void asm_Main()
        {
            asm.WriteLine("%include 'io.inc'");
            asm.WriteLine("\nsegment .text");
            asm.WriteLine("\textern scanf");
            asm.WriteLine("\textern printf");
            asm.WriteLine("\tglobal main");
            asm.WriteLine("\nmain:");
        }
        private void asm_endMain()
        {
            asm.WriteLine("\txor eax, eax");
            asm.WriteLine("\tret");
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
                        /*
                        asm.WriteLine("\tpop eax");
                        
                        asm.WriteLine("\tmov eax, " + "[" + variable + "]");
                        asm.WriteLine("\txor edx,edx");
                        asm.WriteLine("\tdiv ecx");
                        asm.WriteLine("\tmov dword [" + variable + "], edx");
                        */
                        asm.WriteLine("\tmov ecx, ebx");
                        asm.WriteLine("\txor edx,edx");
                        asm.WriteLine("\tdiv ecx");
                        asm.WriteLine("\tpush edx");
                        break; ;
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
                asm.WriteLine("\tmov eax, " + "[" + Contenido + "]");
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