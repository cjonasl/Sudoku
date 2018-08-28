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
        private int _numberOfCellsSetInOriginalData;

        public SolveSudoku(string debugDirectory, int maxNumberOfTries, SudokuBoard sudokuBoard, ArrayList originalData, bool isDebug)
        {
            _debugDirectory = debugDirectory;
            _maxNumberOfTries = maxNumberOfTries;
            _sudokuBoard = sudokuBoard;
            _originalData = originalData;
            _isDebug = isDebug;
            _numberOfCellsSetInOriginalData = sudokuBoard.NumberOfCellsSet;
        }

        //indexShowFirst = The index where most cells were set
        public void Process(out ArrayList resultStrings, out ArrayList sudokuBoardStrings, out ArrayList listBoxEntries, out int indexShowFirst)
        {
            Random random = new Random((int)(DateTime.Now.Ticks % (long)int.MaxValue));
            string ResultStr, iterationDebugDirectory, debugDirectory;
            int numberOfTries = 0, maxCellsSetInATry = -1, totalNumberOfCellsSet, numberOfCellsSetInTry;
            ArrayList result = new ArrayList();

            resultStrings = new ArrayList();
            sudokuBoardStrings = new ArrayList();
            listBoxEntries = new ArrayList();
            indexShowFirst = -1;

            debugDirectory = _debugDirectory + "\\Solve_Sudoku_" + DateTime.Now.ToString("yyyy-MM-dd HH.mm.ss.fff");

            Directory.CreateDirectory(debugDirectory);

            SudokuSolver sudokuSolver = new SudokuSolver(random, _sudokuBoard, _isDebug);

            while ((numberOfTries < _maxNumberOfTries) && !_sudokuBoard.SudokuIsSolved)
            {
                numberOfTries++;
                iterationDebugDirectory = debugDirectory + "\\Try" + numberOfTries.ToString();
                Directory.CreateDirectory(iterationDebugDirectory);
                result = sudokuSolver.Process(iterationDebugDirectory, out ResultStr);
                resultStrings.Add(ResultStr);
                sudokuBoardStrings.Add(_sudokuBoard.SudokuBoardString);
                totalNumberOfCellsSet = _sudokuBoard.NumberOfCellsSet;
                numberOfCellsSetInTry = result.Count;
                listBoxEntries.Add(string.Format("Try{0} ({1}, {2}, {3})", numberOfTries, totalNumberOfCellsSet, _numberOfCellsSetInOriginalData, numberOfCellsSetInTry));

                if ((_originalData.Count + result.Count) != totalNumberOfCellsSet)
                {
                    throw new Exception("if ((_originalData.Count + result.Count) != totalNumberOfCellsSet)");
                }

                if (numberOfTries == 1)
                {
                    maxCellsSetInATry = totalNumberOfCellsSet;
                    indexShowFirst = 0;
                }
                else if ((numberOfTries > 1) && (totalNumberOfCellsSet > maxCellsSetInATry))
                {
                    maxCellsSetInATry = totalNumberOfCellsSet;
                    indexShowFirst = numberOfTries - 1;
                }

                if (_sudokuBoard.SudokuIsSolved)
                {
                    if ((_originalData.Count + result.Count) != 81)
                    {
                        throw new Exception("((_originalData.Count + result.Count) != 81) in method SolveSudoku.Process");
                    }
                }
                else if (numberOfTries < _maxNumberOfTries)
                {
                    _sudokuBoard.Reset(_originalData);
                }
            }
        }
    }
}
