;Analizador Lexico
;Autores:
;Vega Angeles Christopher
;Moya Arreola Cristian
;Martinez Prieto Angel Josue
;Analisis Sintactico
%include 'io.inc'

segment .text
	extern scanf
	extern printf
	global main

main:
	mov eax, 10
	push eax
	pop eax
	mov dword [y], eax
	mov eax, 2
	push eax
	pop eax
	mov dword [z], eax
	PRINT_STRING msg0
;Asignacion a altura
push altura
push format
call scanf
; Termina asignacion a altura
	mov eax, 3
	push eax
	mov eax, [altura]
	push eax
	pop ebx
	pop eax
	add eax, ebx
	push eax
	mov eax, 8
	push eax
	pop ebx
	pop eax
	mul ebx
	push eax
	mov eax, 10
	push eax
	mov eax, 4
	push eax
	pop ebx
	pop eax
	sub eax, ebx
	push eax
	mov eax, 2
	push eax
	pop ebx
	pop eax
	div ebx
	push eax
	pop ebx
	pop eax
	sub eax, ebx
	push eax
	pop eax
	mov dword [x], eax
;Asignacion a x
	dec dword [x]
; Termina asignacion a x
;Asignacion a x
	mov eax, [altura]
	push eax
	mov eax, 8
	push eax
	pop ebx
	pop eax
	mul ebx
	push eax
	pop eax
	add [x], eax
; Termina asignacion a x
;Asignacion a x
	mov eax, 2
	push eax
	pop eax
	mov ebx, [x]
	mul ebx
	xor ebx,ebx
	mov dword [x], eax
; Termina asignacion a x
	mov eax, 1
	push eax
	pop eax
	mov dword [k], eax
;for 1
;Asignacion a i
	mov eax, 1
	push eax
	pop eax
	mov dword [i], eax
; Termina asignacion a i
_forIni1:
	mov eax, [k]
	push eax
	mov eax, [altura]
	push eax
	pop eax
	pop ebx
	cmp eax, ebx
	jl _forFin1
;for 2
;Asignacion a j
	mov eax, 1
	push eax
	pop eax
	mov dword [j], eax
; Termina asignacion a j
_forIni2:
	mov eax, [j]
	push eax
	mov eax, [k]
	push eax
	pop eax
	pop ebx
	cmp eax, ebx
	jl _forFin2
; if 1
	mov eax, [j]
	push eax
	mov eax, 2
	push eax
	pop ebx
	pop eax
	mov ecx, ebx
	xor edx,edx
	div ecx
	push edx
	mov eax, 0
	push eax
	pop eax
	pop ebx
	cmp eax, ebx
	jne _else1
	PRINT_STRING msg1
	jmp _finIf1
_else1:
	PRINT_STRING msg2
_finIf1:
	inc dword [j]
	jmp _forIni2
_forFin2:
	PRINT_STRING msg3
	PRINT_STRING salto
	inc dword [k]
	jmp _forIni1
_forFin1:
;Asignacion a i
	mov eax, 0
	push eax
	pop eax
	mov dword [i], eax
; Termina asignacion a i
; do-while 1
_doInicio1:
	PRINT_STRING msg4
;Asignacion a i
	inc dword [i]
; Termina asignacion a i
	mov eax, [i]
	push eax
	mov eax, [altura]
	push eax
	pop eax
	pop ebx
	cmp eax, ebx
	jle _doFin1
	jmp _doInicio1
_doFin1:
	PRINT_STRING msg5
	PRINT_STRING salto
;for 3
;Asignacion a i
	mov eax, 1
	push eax
	pop eax
	mov dword [i], eax
; Termina asignacion a i
_forIni3:
	mov eax, [i]
	push eax
	mov eax, [altura]
	push eax
	pop eax
	pop ebx
	cmp eax, ebx
	jl _forFin3
;Asignacion a j
	mov eax, 1
	push eax
	pop eax
	mov dword [j], eax
; Termina asignacion a j
; while 1
_whileIni1:
	mov eax, [j]
	push eax
	mov eax, [i]
	push eax
	pop eax
	pop ebx
	cmp eax, ebx
	jl _whileFin1
	PRINT_STRING msg6
	mov eax, [j]
	push eax
	push format
	call printf
;Asignacion a j
	inc dword [j]
; Termina asignacion a j
	jmp _whileIni1
_whileFin1:
	PRINT_STRING msg7
	PRINT_STRING salto
	inc dword [i]
	jmp _forIni3
_forFin3:
;Asignacion a i
	mov eax, 0
	push eax
	pop eax
	mov dword [i], eax
; Termina asignacion a i
; do-while 2
_doInicio2:
	PRINT_STRING msg8
;Asignacion a i
	inc dword [i]
; Termina asignacion a i
	mov eax, [i]
	push eax
	mov eax, [altura]
	push eax
	pop eax
	pop ebx
	cmp eax, ebx
	jle _doFin2
	jmp _doInicio2
_doFin2:
	PRINT_STRING msg9
	PRINT_STRING salto
	xor eax, eax
	ret

segment .data

format db "%d", 0
	altura dd 0
	i dd 0
	j dd 0
	y dd 0
	z dd 0
	c dd 0
	x dd 0
	k dd 0
	msg0 db 'Valor de altura = ' ,0
	msg1 db '*' ,0
	msg2 db '-' ,0
	msg3 db '' ,0
	msg4 db '-' ,0
	msg5 db '' ,0
	msg6 db '' ,0
	msg7 db '' ,0
	msg8 db '-' ,0
	msg9 db '' ,0
	salto db '', 10, 0
