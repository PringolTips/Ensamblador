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
	mov eax, 12
	push eax
	pop eax
	mov dword [a], eax
; do-while 1
_doInicio1:
	PRINT_STRING msg0
;Asignacion a a
	dec dword [a]
; Termina asignacion a a
	mov eax, [a]
	push eax
	mov eax, 11
	push eax
	pop eax
	pop ebx
	cmp eax, ebx
	jne _doFin1
	jmp _doInicio1
_doFin1:
	xor eax, eax
	ret

segment .data

format db "%d", 0
	a dd 0
	msg0 db 'Hello World!',10 ,0
