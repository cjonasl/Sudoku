using System;
using System.Collections;
using System.Text;

namespace Sudoku
{
    public class SudokuSolver
    {
        private OneStepSudokuSolver _oneStepSudokuSolver;
        private SudokuPossibleToSetItem _sudokuPossibleToSetItem;
        private SudokuBoard _sudokuBoard;
        private Random _random;
        private bool _isDebug;

        public SudokuSolver(Random random, SudokuBoard sudokuBoard, bool isDebug)
        {
            _sudokuPossibleToSetItem = new SudokuPossibleToSetItem();
            _oneStepSudokuSolver = new OneStepSudokuSolver(sudokuBoard, _sudokuPossibleToSetItem);
            _sudokuBoard = sudokuBoard;
            _random = random;
            _isDebug = isDebug;
        }

        /*
         Position 1: Iteration numner
         Position 2: Numner of item(s) set in sudoku board
         Position 3: Number of item(s) not set in sudoku board
         Position 4: Total number of item(s) set in iteration
         Position 5: Total number of item(s) set by alone in cell
         Position 6: Total number of item(s) set by alone possible (in row, column and/or square)
         Position 7: Total number of items possible to set without causing conflict
         Position 8: Total number of errors (not possible to set any item in cell)
         Position 9: Simulated one item, true or false
         */
        public ArrayList Process(string debugDirectory, out string resultString)
        {
            //----- out parameter in call to _oneStepSudokuSolver.Process -----
            int numberOfItemsSetDueToAloneInCell;
            int numberOfItemsSetDueToAlonePossibleInRowColumnAndOrSquare;
            int totalNumberOfItemsPossibleToSetWithoutCausingConflict;
            int numberOfErrorsNotPossibleToSetAnyItemInCell;
            int numberOfErrorsNotUniqueItemAlonePossible;
            bool simulated;
            string debugString;
            //-----------------------------------------------------------------

            bool stopIteration = false;
            string str, fileNameFullPath;
            int simulatedItems;
            ThreeTupleOfIntegers[] setInIteration;
            StringBuilder sb = new StringBuilder();
            ArrayList result = new ArrayList();

            int iteration = 0;
            _sudokuPossibleToSetItem.Process(_sudokuBoard);

            while (!stopIteration)
            {
                iteration++;
                setInIteration = _oneStepSudokuSolver.Process
                    (_sudokuBoard,                 
                     _random, 
                     _isDebug,
                     out numberOfItemsSetDueToAloneInCell,
                     out numberOfItemsSetDueToAlonePossibleInRowColumnAndOrSquare,
                     out totalNumberOfItemsPossibleToSetWithoutCausingConflict,
                     out numberOfErrorsNotPossibleToSetAnyItemInCell,
                     out numberOfErrorsNotUniqueItemAlonePossible,
                     out simulated,
                     out debugString);

                fileNameFullPath = (iteration < 10) ? (debugDirectory + "\\OneStepSudokuSolver0" + iteration.ToString() + ".txt") : (debugDirectory + "\\OneStepSudokuSolver" + iteration.ToString() + ".txt");
                Utility.CreateNewFile(fileNameFullPath, debugString);

                simulatedItems = simulated ? 1 : 0;

                int totalSet = numberOfItemsSetDueToAloneInCell + numberOfItemsSetDueToAlonePossibleInRowColumnAndOrSquare + numberOfErrorsNotUniqueItemAlonePossible + simulatedItems;

                if ((setInIteration != null) && (setInIteration.Length != totalSet))
                {
                    throw new Exception("((setInIteration != null) && (setInIteration.Length != totalSet))");
                }

                str = string.Format("({0}, {1}, {2}, {3}, {4}, {5}, {6}, {7}, {8}, {9})",
                    iteration,
                    _sudokuBoard.NumberOfCellsSet,
                    81 - _sudokuBoard.NumberOfCellsSet,
                    totalSet,
                    numberOfItemsSetDueToAloneInCell,
                    numberOfItemsSetDueToAlonePossibleInRowColumnAndOrSquare,
                    totalNumberOfItemsPossibleToSetWithoutCausingConflict,
                    numberOfErrorsNotPossibleToSetAnyItemInCell,
                    numberOfErrorsNotUniqueItemAlonePossible,
                    simulated.ToString().ToLower());

                if (setInIteration != null)
                {
                    for (int i = 0; i < totalSet; i++)
                    {
                        result.Add(setInIteration[i]);
                    }
                }

                sb.Append(str + "\r\n");

                if (_sudokuBoard.SudokuIsSolved || (totalNumberOfItemsPossibleToSetWithoutCausingConflict == 0))
                {
                    stopIteration = true;
                }
            }

            resultString = sb.ToString().TrimEnd();

            return result;
        }
    }
}
