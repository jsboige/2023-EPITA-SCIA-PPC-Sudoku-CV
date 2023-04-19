package fr.epita;

import org.chocosolver.solver.Model;
import org.chocosolver.solver.Solution;
import org.chocosolver.solver.search.strategy.Search;
import org.chocosolver.solver.variables.IntVar;

import static org.chocosolver.util.tools.ArrayUtils.append;


public class SudokuSolver {

    private final int n = 9;
    protected Model model;
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

    public void solve(int method) {
        this.buildModel();
        switch (method){
            case 0:
                model.getSolver().setSearch(Search.inputOrderLBSearch(append(rows)));
                break;
            case 1:
                model.getSolver().setSearch(Search.domOverWDegSearch(append(rows)));
                break;
            case 2:
                model.getSolver().setSearch(Search.minDomLBSearch(append(rows)));
                break;
            case 3:
                model.getSolver().setSearch(Search.randomSearch(append(rows), 42));
                break;
            case 4:
                model.getSolver().setSearch(Search.conflictHistorySearch(append(rows)));
                break;
            case 5:
                model.getSolver().setSearch(Search.activityBasedSearch(append(rows)));
                break;
            case 6:
                model.getSolver().setSearch(Search.failureRateBasedSearch(append(rows)));
                break;
            default:
                model.getSolver().setSearch(Search.defaultSearch(model));
                break;
        }
        Solution solution = model.getSolver().findSolution();
        model.getSolver().solve();

        for (int i = 0; i < n; i++) {
            for (int j = 0; j < n; j++) {
                this.grid[i][j] = solution.getIntVal(rows[i][j]);
            }
        }
    }
}