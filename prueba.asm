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
	mov eax, 5
	push eax
	pop eax
	mov dword [a], eax
;Asignacion a a
push a
push format
call scanf
; Termina asignacion a a
; if 1
	mov eax, [a]
	push eax
	mov eax, 5
	push eax
	pop eax
	pop ebx
	cmp eax, ebx
	jle _else1
	PRINT_STRING msg0
	jmp _finIf1
_else1:
	PRINT_STRING msg1
_finIf1:
	xor eax, eax
	ret

segment .data

format db "%d", 0
	a dd 0
	msg0 db 'Hello World!',10 ,0
	msg1 db 'Adios',10 ,0
