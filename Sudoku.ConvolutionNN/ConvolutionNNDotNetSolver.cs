using Python.Runtime;
using Sudoku.Shared;
using System.Runtime.InteropServices;
using System;
using System.IO;
using System.Reflection;
using Microsoft.VisualBasic;

namespace Sudoku.ConvolutionNN;

public class  ConvolutionNNDotNetSolver : PythonSolverBase
{
    public override Shared.SudokuGrid Solve(Shared.SudokuGrid s)
    {
        using (PyModule scope = Py.CreateScope())
        {
            // convert the Cells array object to a PyObject
            PyObject pyCells = s.Cells.ToPython();

            // create a Python variable "instance"
            scope.Set("instance", pyCells);

            try
            {
                string modelPath = Path.Combine(GetResourcesPath(), "sudoku.model");
                Console.WriteLine($"model Path:\n{modelPath}");
                scope.Set("model_path", modelPath);
            }
            catch (ApplicationException exception) 
            {
                Console.WriteLine(exception.Message);
            }

            // run the Python script
            string code = Resources.CNN_py;
            scope.Exec(code);

            //Retrieve solved Sudoku variable
            var result = scope.Get("r");

            //Convert back to C# object
            var managedResult = result.As<int[][]>();
            //var convertesdResult = managedResult.Select(objList => objList.Select(o => (int)o).ToArray()).ToArray();
            return new Shared.SudokuGrid() { Cells = managedResult };
        }
    }

    protected override void InitializePythonComponents()
    {
        //declare your pip packages here
        InstallPipModule("numpy");
        InstallPipModule("pandas");
        InstallPipModule("scikit-learn");
        if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            InstallPipModule("tensorflow-macos");
        else
            InstallPipModule("tensorflow");
        base.InitializePythonComponents();
    }
    
    protected string GetResourcesPath()
    {
        //var currentDirectory = new DirectoryInfo(Environment.CurrentDirectory);
        //var regex = new System.Text.RegularExpressions.Regex("(.*)(2023-EPITA-SCIA-PPC-Sudoku-CV)(.*)");
        //var match = regex.Match(currentDirectory.ToString());
        //if (match.Success)
        //{
        //    var path = match.Groups[1].Value;
        //    return Path.Combine(path, "2023-EPITA-SCIA-PPC-Sudoku-CV", "Sudoku.ConvolutionNN", "Resources");
        //}

        //throw new ApplicationException("Couldn't find resources directory");
        return Path.Combine(Environment.CurrentDirectory, @".\Resources\");
    }
}