using System;
using System.Collections;
using System.Text;

namespace Sudoku
{
    public class SudokuSolver
    {
        private OneStepSudokuSolver _oneStepSudokuSolver;
        private string _debugDirectory;

        public SudokuSolver(string debugDirector)
        {
            _debugDirectory = debugDirector;
            _oneStepSudokuSolver = new OneStepSudokuSolver();
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
        public ArrayList Process(SudokuBoard sudokuBoard, Random r, out string resultString)
        {
            int iteration, i, totalNumberOfItemsPossibleToSetUniquely, numberOfItemsPossibleToSetUniquelyDueToAloneInCell, numberOfItemsPossibleToSetUniquelyDueToAlonePossibleInRowColumnAndOrSquare, totalNumberOfItemsPossibleToSetWithoutCausingConflict, NumberOfCellsNotPossibleToSetAnyItemInCellThatDoesNotCauseConflict;
            bool simulated, stopIteration = false;
            string str, debugString, fileNameFullPath;
            ThreeTupleOfIntegers[] possibleToSetUniqueInIteration;
            StringBuilder sb = new StringBuilder();
            ArrayList result = new ArrayList();

            iteration = 0;

            while (!stopIteration)
            {
                iteration++;
                possibleToSetUniqueInIteration = _oneStepSudokuSolver.Process(sudokuBoard, r, out numberOfItemsPossibleToSetUniquelyDueToAloneInCell, out numberOfItemsPossibleToSetUniquelyDueToAlonePossibleInRowColumnAndOrSquare, out totalNumberOfItemsPossibleToSetWithoutCausingConflict, out NumberOfCellsNotPossibleToSetAnyItemInCellThatDoesNotCauseConflict, out simulated, out debugString);
                fileNameFullPath = (iteration < 10) ? (_debugDirectory + "\\OneStepSudokuSolver0" + iteration.ToString() + ".txt") : (_debugDirectory + "\\OneStepSudokuSolver" + iteration.ToString() + ".txt");
                Utility.CreateNewFile(fileNameFullPath, debugString);

                if (possibleToSetUniqueInIteration != null)
                {
                    sudokuBoard.Set(possibleToSetUniqueInIteration);
                }

                totalNumberOfItemsPossibleToSetUniquely = numberOfItemsPossibleToSetUniquelyDueToAloneInCell + numberOfItemsPossibleToSetUniquelyDueToAlonePossibleInRowColumnAndOrSquare;

                str = string.Format("({0}, {1}, {2}, {3}, {4}, {5}, {6}, {7}, {8})",
                    iteration,
                    sudokuBoard.NumberOfBoardEntriesSet,
                    81 - sudokuBoard.NumberOfBoardEntriesSet,
                    totalNumberOfItemsPossibleToSetUniquely,
                    numberOfItemsPossibleToSetUniquelyDueToAloneInCell,
                    numberOfItemsPossibleToSetUniquelyDueToAlonePossibleInRowColumnAndOrSquare,
                    totalNumberOfItemsPossibleToSetWithoutCausingConflict,
                    NumberOfCellsNotPossibleToSetAnyItemInCellThatDoesNotCauseConflict,
                    simulated);

                for(i = 0; i < totalNumberOfItemsPossibleToSetUniquely; i++)
                {
                    result.Add(possibleToSetUniqueInIteration[i]);
                }

                if (simulated)
                {
                    if (totalNumberOfItemsPossibleToSetUniquely != 0)
                    {
                        throw new Exception("(totalNumberOfItemsUniqelySet != 0) in OneStepSudokuSolver!!!!");
                    }

                    result.Add(possibleToSetUniqueInIteration[0]);
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
