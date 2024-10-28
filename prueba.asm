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
;for 1
;Asignacion a i
	mov eax, 0
	push eax
	pop eax
	mov dword [i], eax
; Termina asignacion a i
_forIni1:
	mov eax, [i]
	push eax
	mov eax, 5
	push eax
	pop eax
	pop ebx
	cmp eax, ebx
	jl _forFin1
;Asignacion a i
	inc dword [i]
; Termina asignacion a i
	PRINT_STRING msg0
	jmp _forIni1
_forFin1:
	xor eax, eax
	ret

segment .data
	i dd 0
	msg0 db 'Hello World!',10 ,0
