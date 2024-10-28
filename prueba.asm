;Analizador Lexico
;Autores:
;Vega Angeles Christopher
;Moya Arreola Cristian
;Martinez Prieto Angel Josue
;Analisis Sintactico
%include 'io.inc'

segment .text
	extern scanf
	global main

main:
;Asignacion a a
push a
push format
call scanf
; Termina asignacion a a
	xor eax, eax
	ret

segment .data

format db "%d", 0
	a dd 0
