// Нажатая кнопка                	Что рисовать	Расстояние от левого края экрана	Расстояние от верхнего края экрана
// LEFT (130)	                       Смайл	                   160                             	124
// UP (131)	                           Смайл	                   256                             	82
// RIGHT (132)	                       Смайл	                   336                             	124
// DOWN (133)	                       Смайл	                   256                             	165
// другая кнопка или никакая        Пустой экран	                —	                             —
// Если нажатие одной кнопки курсора моментально сменилось на нажатие другой кнопки курсора, то рисовать нужно только один смайл.
// нужный регистр вычисляется по формуле RAM = [SCREEN + 32*row + col//16] (SCREEN = 16384)

// CURRENT_OFFSET and PREVIOUS_OFFSET == top*32 + left/16


(LOOP)
    // проверка на нажатие кнопки UP, DOWN, LEFT, RIGHT
    @KBD
    D=M
    @130
    D=D-A
    @LEFT
    D;JEQ
    @UP 
    D=D-1
    D;JEQ
    @RIGHT
    D=D-1
    D;JEQ
    @DOWN
    D=D-1
    D;JEQ
    // если не была нажата клавиша, обнуляем текущую позицию
    @CURRENT_OFFSET
    M=0
    // стираем, если не была нажата клавиша, и при этом ранее был отрисован смайл
    @PREVIOUS_OFFSET
    D=M
    @CLEAR
    D;JNE

    @LOOP
    0;JMP

// обновление текущей позиции смайла
(LEFT)
    @3978
    D=A
    @CURRENT_OFFSET
    M=D
    @THERE_SMILEY
    0;JMP

(UP)
    @2640
    D=A
    @CURRENT_OFFSET
    M=D
    @THERE_SMILEY
    0;JMP

(RIGHT)
    @3989
    D=A
    @CURRENT_OFFSET
    M=D
    @THERE_SMILEY
    0;JMP

(DOWN)
    @5296
    D=A
    @CURRENT_OFFSET
    M=D
    @THERE_SMILEY
    0;JMP

(THERE_SMILEY)
    // проверка текущих координат смайла с прошлыми
    @CURRENT_OFFSET
    D=M
    @PREVIOUS_OFFSET
    D=D-M
    @LOOP
    D;JEQ
    // если координаты разнятся, то стираем смайл
    @CLEAR
    0;JMP

(CLEAR)
    // стирание смайла по предыдущим координатам
    @PREVIOUS_OFFSET
    D=M

    @16384
    A = D+A
    M = 0

    @16416
    A = D+A
    M = 0

    @16448
    A = D+A
    M = 0

    @16544
    A = D+A
    M = 0

    @16576
    A = D+A
    M = 0

    @16608
    A = D+A
    M = 0

    // обнуление предыдущего значения
    @PREVIOUS_OFFSET
    M=0

    // если клавиша не не была нажата, то cuurent__offset = 0 и тогда идем обратно в начальный цикл
    @CURRENT_OFFSET
    D=M
    @LOOP
    D;JEQ
    // иначе рисуем новый смайл
    @PAINT
    0;JMP

(PAINT)
    // рисуем смайл со смещением CURRENT_OFFSET
    @CURRENT_OFFSET
    D=M
    @16384
    D = D+A
    @OFFSET
    M=D
    @7224
    D=A
    @OFFSET
    A=M
    M = D

    @CURRENT_OFFSET
    D=M
    @16416
    D = D+A
    @OFFSET
    M=D
    @7224
    D=A
    @OFFSET
    A=M
    M = D

    @CURRENT_OFFSET
    D=M
    @16448
    D = D+A
    @OFFSET
    M=D
    @7224
    D=A
    @OFFSET
    A=M
    M = D

    @CURRENT_OFFSET
    D=M
    @16544
    D = D+A
    @OFFSET
    M=D
    @24582
    D=A
    @OFFSET
    A=M
    M = D

    @CURRENT_OFFSET
    D=M
    @16576
    D = D+A
    @OFFSET
    M=D
    @14364
    D=A
    @OFFSET
    A=M
    M = D

    @CURRENT_OFFSET
    D=M
    @16608
    D = D+A
    @OFFSET
    M=D
    @4080
    D=A
    @OFFSET
    A=M
    M = D

    // обновление PREVIOUS_OFFSET
    @CURRENT_OFFSET
    D=M
    @PREVIOUS_OFFSET
    M=D

    @LOOP
    0;JMP