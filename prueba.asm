Analisis Sintactico

extern fflush
extern printf
extern scanf
extern stdout

segment .text
	global _main

_main:
	add esp, 4

	mov eax, 1
	xor ebx, ebx
	int 0x80

segment .data
	i dd 0
	y dq 0 
	z db 0
