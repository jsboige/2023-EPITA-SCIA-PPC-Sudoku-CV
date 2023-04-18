package fr.epita;

import java.util.ArrayList;

import org.chocosolver.solver.Model;
import org.chocosolver.solver.Solution;
import org.chocosolver.solver.search.strategy.Search;
import org.chocosolver.solver.variables.IntVar;
import org.chocosolver.util.tools.ArrayUtils;

import static org.chocosolver.solver.search.strategy.Search.minDomLBSearch;
import static org.chocosolver.util.tools.ArrayUtils.append;


public class SudokuSolver extends Object {

    protected Model model;
    private final int n = 9;
    IntVar[][] rows, cols, carres;
    int[][] grid;

    public SudokuSolver(int[][] grid) {
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

    public void solve() {
        this.buildModel();
        this.configureSearch();
        Solution solution = model.getSolver().findSolution();
        model.getSolver().solve();

        for (int i = 0; i < n; i++) {
            for (int j = 0; j < n; j++) {
                this.grid[i][j] = solution.getIntVal(rows[i][j]);
            }
        }
    }

}