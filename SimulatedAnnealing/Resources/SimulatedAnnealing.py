def find_empty_location(board):
        for row in range(9):
            for col in range(9):
                if board[row][col] == 0:
                    return row, col
        return (-1, -1)


def is_valid(puzzle, row, col, num):
        # check row
        for i in range(9):
            if puzzle[row][i] == num:
                return False
        # check column
        for i in range(9):
            if puzzle[i][col] == num:
                return False
        # check square
        square_row = (row // 3) * 3
        square_col = (col // 3) * 3
        for i in range(3):
            for j in range(3):
                if puzzle[square_row + i][square_col + j] == num:
                    return False
        return True


def solveSudoku(board):
    row, col = find_empty_location(board)
    
    if row == -1 and col == -1:
        return True
    
    for num in range(1, 10):
        if is_valid(board, row, col, num):
            board[row][col] = num
            

            if solveSudoku(board):
                return True
            

            board[row][col] = 0
            

    return False



if(solveSudoku(instance)):

	r=instance
else:
	print ("Aucune solution trouv")

