// RAM[4] = (RAM[0] * 3 + (RAM[1] | RAM[2])) & !RAM[3] + 11

// D = RAM[1] | RAM[2]
@1
D=M
@2
A=M
D=D|A

// D += RAM[0] * 3
@0
D=D+M
D=D+M
D=D+M

// D = D & !RAM[3]
@3
A=!M
D=D&A

// D += 11
@11
D=D+A

// RAM[4] = D
@4
M=D