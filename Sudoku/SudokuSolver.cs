using System;
using System.Collections;
using System.Text;

namespace Sudoku
{
    public class SudokuSolver
    {
        private OneStepSudokuSolver _oneStepSudokuSolver;
        private Random _random;
        private bool _isDebug;

        public SudokuSolver(Random random, bool isDebug)
        {
            _oneStepSudokuSolver = new OneStepSudokuSolver();
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
        public ArrayList Process(string debugDirectory, SudokuBoard sudokuBoard, out string resultString)
        {
            //----- out parameter in call to _oneStepSudokuSolver.Process -----
            int numberOfItemsitemsSetDueToAloneInCell;
            int numberOfItemsitemsSetDueToAlonePossibleInRowColumnAndOrSquare;
            int totalNumberOfItemsPossibleToSetWithoutCausingConflict;
            int numberOfErrorsNotPossibleToSetAnyItemInCell;
            int numberOfErrorsNotUniqueItemAlonePossible;
            bool simulated;
            string debugString;
            //-----------------------------------------------------------------

            bool stopIteration = false;
            string str, fileNameFullPath;
            ThreeTupleOfIntegers[] setInIteration;
            StringBuilder sb = new StringBuilder();
            ArrayList result = new ArrayList();

            int iteration = 0;

            while (!stopIteration)
            {
                iteration++;
                setInIteration = _oneStepSudokuSolver.Process
                    (sudokuBoard,                 
                     _random, 
                     _isDebug,
                     out numberOfItemsitemsSetDueToAloneInCell,
                     out numberOfItemsitemsSetDueToAlonePossibleInRowColumnAndOrSquare,
                     out totalNumberOfItemsPossibleToSetWithoutCausingConflict,
                     out numberOfErrorsNotPossibleToSetAnyItemInCell,
                     out numberOfErrorsNotUniqueItemAlonePossible,
                     out simulated,
                     out debugString);

                fileNameFullPath = (iteration < 10) ? (debugDirectory + "\\OneStepSudokuSolver0" + iteration.ToString() + ".txt") : (debugDirectory + "\\OneStepSudokuSolver" + iteration.ToString() + ".txt");
                Utility.CreateNewFile(fileNameFullPath, debugString);

                int totalSet = numberOfItemsitemsSetDueToAloneInCell + numberOfItemsitemsSetDueToAlonePossibleInRowColumnAndOrSquare;

                if (!simulated && (setInIteration != null) && (setInIteration.Length != totalSet))
                {
                    throw new Exception("(setInIteration.Length != totalSet) in OneStepSudokuSolver!!!!");
                }

                str = string.Format("({0}, {1}, {2}, {3}, {4}, {5}, {6}, {7}, {8}, {9})",
                    iteration,
                    sudokuBoard.NumberOfBoardEntriesSet,
                    81 - sudokuBoard.NumberOfBoardEntriesSet,
                    totalSet,
                    numberOfItemsitemsSetDueToAloneInCell,
                    numberOfItemsitemsSetDueToAlonePossibleInRowColumnAndOrSquare,
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

                if (simulated)
                {
                    if (totalSet != 0)
                    {
                        throw new Exception("(totalSet != 0) in OneStepSudokuSolver!!!!");
                    }

                    result.Add(setInIteration[0]);
                }

                sb.Append(str + "\r\n");

                if (sudokuBoard.SudokuIsSolved || (totalNumberOfItemsPossibleToSetWithoutCausingConflict == 0))
                {
                    stopIteration = true;
                }
            }

            resultString = sb.ToString().TrimEnd();

            return result;
        }
    }
}
