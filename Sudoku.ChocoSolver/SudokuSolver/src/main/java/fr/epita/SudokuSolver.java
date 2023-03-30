package fr.epita;

import org.chocosolver.solver.Model;
import org.chocosolver.solver.Solution;
import org.chocosolver.solver.variables.IntVar;

import java.util.Arrays;
import java.util.Optional;

import static org.chocosolver.solver.search.strategy.Search.minDomLBSearch;
import static org.chocosolver.util.tools.ArrayUtils.append;

public class SudokuSolver {
    protected Model model;
    private final int n = 9;
    IntVar[][] rows, cols, carres;
    Integer[][] grid;

    public SudokuSolver(Integer[][] grid) {
        this.grid = grid;
    }

    private void buildModel() {
        model = new Model();

        rows = new IntVar[n][n];
        cols = new IntVar[n][n];
        carres = new IntVar[n][n];
        for (int i = 0; i < n; i++) {
            for (int j = 0; j < n; j++) {
                if (grid[i][j] > 0) {
                    rows[i][j] = model.intVar(grid[i][j]);
                } else {
                    rows[i][j] = model.intVar("c_" + i + "_" + j, 1, n, false);
                }
                cols[j][i] = rows[i][j];
            }
        }

        for (int i = 0; i < 3; i++) {
            for (int j = 0; j < 3; j++) {
                for (int k = 0; k < 3; k++) {
                    carres[j + k * 3][i] = rows[k * 3][i + j * 3];
                    carres[j + k * 3][i + 3] = rows[1 + k * 3][i + j * 3];
                    carres[j + k * 3][i + 6] = rows[2 + k * 3][i + j * 3];
                }
            }
        }

        for (int i = 0; i < n; i++) {
            model.allDifferent(rows[i], "AC").post();
            model.allDifferent(cols[i], "AC").post();
            model.allDifferent(carres[i], "AC").post();
        }
    }

    private void configureSearch() {
        model.getSolver().setSearch(minDomLBSearch(append(rows)));
    }

    public Integer[][] solve() {
        this.buildModel();
        this.configureSearch();
        Solution solution = model.getSolver().findSolution();
        model.getSolver().solve();

        Integer[][] result = new Integer[n][n];
        for (int i = 0; i < n; i++) {
            for (int j = 0; j < n; j++) {
                result[i][j] = solution.getIntVal(rows[i][j]);
            }
            System.out.println();
        }

        return result;
    }

    public static void main(String[] args) {
        Integer[][] grid =  new Integer[][]{
                {0, 0, 0, 1, 0, 5, 4, 0, 0},
                {0, 6, 0, 2, 0, 8, 0, 0, 7},
                {0, 5, 2, 0, 0, 0, 1, 0, 0},
                {0, 1, 5, 6, 0, 2, 0, 0, 0},
                {2, 0, 0, 0, 0, 7, 5, 1, 0},
                {0, 7, 8, 4, 0, 0, 0, 3, 2},
                {0, 0, 3, 0, 1, 4, 7, 0, 6},
                {0, 0, 0, 0, 0, 0, 0, 0, 0},
                {6, 0, 0, 5, 0, 0, 0, 8, 0}
        };

        SudokuSolver sudokuSolver = new SudokuSolver(grid);
        Integer[][] result = sudokuSolver.solve();
        System.out.println(Arrays.deepToString(result));
    }
}
