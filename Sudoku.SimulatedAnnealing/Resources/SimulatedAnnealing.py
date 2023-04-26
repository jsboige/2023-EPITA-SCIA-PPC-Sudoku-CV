import random
import numpy as np
from simanneal import Annealer

def print_sudoku(state):
    border = "------+-------+------"
    rows = [state[i:i+9] for i in range(0,81,9)]
    for i,row in enumerate(rows):
        if i % 3 == 0:
            print(border)
        three = [row[i:i+3] for i in range(0,9,3)]
        print(" | ".join(
            " ".join(str(x or "_") for x in one)
            for one in three
        ))
    print(border)

def coord(row, col):
    return row*9+col

def block_indices(block_num):
    """return linear array indices corresp to the sq block, row major, 0-indexed.
    block:
       0 1 2     (0,0) (0,3) (0,6)
       3 4 5 --> (3,0) (3,3) (3,6)
       6 7 8     (6,0) (6,3) (6,6)
    """
    firstrow = (block_num // 3) * 3
    firstcol = (block_num % 3) * 3
    indices = [coord(firstrow+i, firstcol+j) for i in range(3) for j in range(3)]
    return indices

def initial_solution(problem):
    """provide sudoku problem, generate an init solution by randomly filling
    each sq block without considering row/col consistency"""
    solution = problem.copy()
    for block in range(9):
        indices = block_indices(block)
        block = problem[indices]
        zeros = [i for i in indices if problem[i] == 0]
        to_fill = [i for i in range(1, 10) if i not in block]
        random.shuffle(to_fill)
        for index, value in zip(zeros, to_fill):
            solution[index] = value
    return solution

class Sudoku_Sq(Annealer):
    def __init__(self, problem):
        self.problem = problem
        state = initial_solution(problem)
        super().__init__(state)
    def move(self):
        """randomly swap two cells in a random square"""
        block = random.randrange(9)
        indices = [i for i in block_indices(block) if self.problem[i] == 0]
        if len(indices) < 2:
            return
        m, n = random.sample(indices, 2)
        self.state[m], self.state[n] = self.state[n], self.state[m]
    def energy(self):
        """calculate the number of violations: assume all rows are OK"""
        column_score = lambda n: -len(set(self.state[coord(i, n)] for i in range(9)))
        row_score = lambda n: -len(set(self.state[coord(n, i)] for i in range(9)))
        score = sum(column_score(n)+row_score(n) for n in range(9))
        if score == -162:
            self.user_exit = True # early quit, we found a solution
        return score

def solveSudoku(instance):
    PROBLEM = np.array([element for tuple in instance for element in tuple])
    sudoku = Sudoku_Sq(PROBLEM)
    sudoku.copy_strategy = "method"
    #print_sudoku(sudoku.state)
    #sudoku.steps = 1000000
    #auto_schedule = sudoku.auto(minutes=1)
    #print(auto_schedule)
    #sudoku.set_schedule(auto_schedule)
    sudoku.Tmax = 0.1
    sudoku.Tmin = 0.05
    sudoku.steps = 1000000
    sudoku.updates = 1000
    state, e = sudoku.anneal()
    #print("\n")
    #print_sudoku(state)
    #print("E=%f (expect -162)" % e)

    state = state.reshape(9, 9)
    res = tuple(tuple(int(x) for x in row) for row in state)
    return res

r = solveSudoku(instance)
