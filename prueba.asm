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
	mov eax, 6
	push eax
	pop eax
	mov dword [i], eax
;Asignacion a i
	mov eax, 7
	push eax
	mov eax, 4
	push eax
	pop ebx
	pop eax
	add eax, ebx
	push eax
	pop eax
	mov dword [i], eax
; Termina asignacion a i
	PRINT_STRING msg0
	xor eax, eax
	ret

segment .data
	i dd 0
	msg0 db 'Hola',10 ,0
