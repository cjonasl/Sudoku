using System;
using System.Collections;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Sudoku
{
    public class SolveSudoku
    {
        private string _debugDirectory;
        private int _maxNumberOfTries;
        private SudokuBoard _sudokuBoard;
        private ArrayList _originalData;
        private bool _isDebug;

        public SolveSudoku(string debugDirectory, int maxNumberOfTries, SudokuBoard sudokuBoard, ArrayList originalData, bool isDebug)
        {
            _debugDirectory = debugDirectory;
            _maxNumberOfTries = maxNumberOfTries;
            _sudokuBoard = sudokuBoard;
            _originalData = originalData;
            _isDebug = isDebug;
        } 

        public ArrayList Process(out ArrayList resultStrings)
        {
            Random random = new Random((int)(DateTime.Now.Ticks % (long)int.MaxValue));
            string ResultStr, iterationDebugDirectory, debugDirectory;
            int numberOfTries = 0;
            ArrayList result = new ArrayList();

            resultStrings = new ArrayList();

            debugDirectory = _debugDirectory + "\\Solve_Sudoku_" + DateTime.Now.ToString("yyyy-MM-dd HH.mm.ss.fff");

            Directory.CreateDirectory(debugDirectory);

            SudokuSolver sudokuSolver = new SudokuSolver(random, _isDebug);

            while ((numberOfTries < _maxNumberOfTries) && !_sudokuBoard.SudokuIsSolved)
            {
                numberOfTries++;
                iterationDebugDirectory = debugDirectory + "\\Try" + numberOfTries.ToString();
                Directory.CreateDirectory(iterationDebugDirectory);
                result = sudokuSolver.Process(iterationDebugDirectory, _sudokuBoard, out ResultStr);
                resultStrings.Add(ResultStr);

                if (_sudokuBoard.SudokuIsSolved)
                {
                    if ((_originalData.Count + result.Count) != 81)
                    {
                        throw new Exception("((_originalData.Count + result.Count) != 81) in method SolveSudoku.Process");
                    }

                    Utility.CreateNewFile(iterationDebugDirectory + "\\Sudoku_solved.txt", _sudokuBoard.SudokuBoardString);
                }
                else if (numberOfTries < _maxNumberOfTries)
                {
                    _sudokuBoard.Reset(_originalData);
                }
            }

            return result;
        }
    }
}
