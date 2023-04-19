from pycsp3 import *


# clues = data  # if not 0, clues[i][j] is a value imposed at row i and col j
clues = instance

# x[i][j] is the value at row i and col j
grid = VarArray(size=[9, 9], dom=range(1, 10))

satisfy(
   # imposing distinct values on each row and each column
   AllDifferent(grid, matrix=True),

   # imposing distinct values on each block  tag(blocks)
   [AllDifferent(grid[i:i + 3, j:j + 3]) for i in [0, 3, 6] for j in [0, 3, 6]],

   # imposing clues  tag(clues)
   [grid[i][j] == clues[i][j] for i in range(9) for j in range(9)
     if clues and clues[i][j] > 0]
)

r = []

if solve() is SAT:
    r = list(values(grid))
else:
    print('No solution was found')
