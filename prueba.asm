;Analizador Lexico
;Autores:
;Vega Angeles Christopher
;Moya Arreola Cristian
;Martinez Prieto Angel Josue
;Analisis Sintactico
%include 'io.inc'

segment .text
	global main

main:
	PRINT_STRING msg0
	PRINT_STRING msg1
	PRINT_STRING msg2
	xor eax, eax
	 ret

segment .data
	msg0 db '"Hola mundo"' ,0
	msg1 db '"Hola mundo"' ,0
	msg2 db '"Hola mundo"' ,0
