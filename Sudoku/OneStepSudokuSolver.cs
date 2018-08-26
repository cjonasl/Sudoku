using System;
using System.Collections;
using System.Configuration;
using System.IO;
using System.Text;


namespace Sudoku
{
    public class OneStepSudokuSolver
    {
        private SudokuPossibleToSetItem _sudokuPossibleToSetItem;

        public OneStepSudokuSolver()
        {
            _sudokuPossibleToSetItem = new SudokuPossibleToSetItem();
        }

        private bool IsItemAloneInRow(int rowIndex, int item)
        {
            int i, j, numberOfOccurenciesOfItem = 0;

            for (i = 0; i < 9; i++)
            {
                for (j = 0; j < sudokuPossibleToSetItem.numberOfPossibleItemsRows[rowIndex, i]; j++)
                {
                    if (sudokuPossibleToSetItem.rows[rowIndex, i, j] == item)
                    {
                        numberOfOccurenciesOfItem++;
                    }
                }
            }

            if (numberOfOccurenciesOfItem == 0)
            {
                throw new Exception("(numberOfOccurenciesOfItem == 0) in method IsItemAloneInRow");
            }

            return (numberOfOccurenciesOfItem == 1);
        }

        private bool IsItemAloneInColumn(int columnIndex, int item)
        {
            int i, j, numberOfOccurenciesOfItem = 0;

            for (i = 0; i < 9; i++)
            {
                for (j = 0; j < sudokuPossibleToSetItem.numberOfPossibleItemsColumns[columnIndex, i]; j++)
                {
                    if (sudokuPossibleToSetItem.columns[columnIndex, i, j] == item)
                    {
                        numberOfOccurenciesOfItem++;
                    }
                }
            }

            if (numberOfOccurenciesOfItem == 0)
            {
                throw new Exception("(numberOfOccurenciesOfItem == 0) in method IsItemAloneInColumn");
            }

            return (numberOfOccurenciesOfItem == 1);
        }

        private bool IsItemAloneInSquare(int squareIndex, int item)
        {
            int i, j, numberOfOccurenciesOfItem = 0;

            for (i = 0; i < 9; i++)
            {
                for (j = 0; j < sudokuPossibleToSetItem.numberOfPossibleItemsSquares[squareIndex, i]; j++)
                {
                    if (sudokuPossibleToSetItem.squares[squareIndex, i, j] == item)
                    {
                        numberOfOccurenciesOfItem++;
                    }
                }
            }

            if (numberOfOccurenciesOfItem == 0)
            {
                throw new Exception("(numberOfOccurenciesOfItem == 0) in method IsItemAloneInSquare");
            }

            return (numberOfOccurenciesOfItem == 1);
        }

        private void FillTempIntArray(int[,,] v, int i, int j, int n, int[] tmpIntArray)
        {
            for (int h = 0; h < n; h++)
            {
                tmpIntArray[h] = v[i, j, h];
            }
        }

        public ThreeTupleOfIntegers[] Process(SudokuBoard sudokuBoard, Random r, out int numberOfItemsPossibleToSetUniquelyDueToAloneInCell, out int numberOfItemsPossibleToSetUniquelyDueToAlonePossibleInRowColumnAndOrSquare, out int totalNumberOfItemsPossibleToSetWithoutCausingConflict, out int NumberOfCellsNotPossibleToSetAnyItemInCellThatDoesNotCauseConflict, out bool simulated, out string debugString)
        {
            int i, j, k, n, totalNumberOfItemsPossibleToSetUniquely, squareIndex;
            string str;
            StringBuilder sb1, sb2, tmpStringBuilder;
            ArrayList possibleToSetUniquely = new ArrayList();
            ThreeTupleOfIntegers[] result;
            int[] tmpIntArray = new int[9];

            totalNumberOfItemsPossibleToSetUniquely = 0;
            numberOfItemsPossibleToSetUniquelyDueToAloneInCell = 0;
            numberOfItemsPossibleToSetUniquelyDueToAlonePossibleInRowColumnAndOrSquare = 0;
            NumberOfCellsNotPossibleToSetAnyItemInCellThatDoesNotCauseConflict = 0;
            simulated = false;

            _sudokuPossibleToSetItem.Process(sudokuBoard);
            totalNumberOfItemsPossibleToSetWithoutCausingConflict = _sudokuPossibleToSetItem.totalNumberOfItemsPossibleToSetWithoutCausingConflict;

            if (totalNumberOfItemsPossibleToSetWithoutCausingConflict == 0)
            {
                debugString = "Not possible to set anything!!";
                return null;
            }

            sb1 = new StringBuilder();
            sb2 = new StringBuilder("Result: The following ####REPLACE_NUMBER_OF_ITEMS##### item(s) are possible to set, [row, column, item] = {");
            tmpStringBuilder = new StringBuilder();

            sb1.Append(_sudokuPossibleToSetItem.ToString());
            sb1.Append("\r\n------------------------Find cells to set\r\n\r\n------------------------");

            for (i = 0; i < 9; i++)
            {
                for (j = 0; j < 9; j++)
                {
                    sb1.Append(string.Format("[{0}, {1}]: ", i + 1, j + 1));

                    squareIndex = (3 * (i / 3)) + (j / 3);

                    if (sudokuBoard.ItemIsSet(i, j))
                    {
                        sb1.Append("Item already set (is " + sudokuBoard.ReturnItem(i, j).ToString() + ")\r\n");
                    }
                    else
                    {
                        if (_sudokuPossibleToSetItem.numberOfPossibleItemsRows[i, j] == 0)
                        {
                            sb1.Append("ERROR!! Not possible to set any item in cell that does not cause conflict!");
                            NumberOfCellsNotPossibleToSetAnyItemInCellThatDoesNotCauseConflict++;
                        }
                        else if (_sudokuPossibleToSetItem.numberOfPossibleItemsRows[i, j] == 1)
                        {
                            numberOfItemsPossibleToSetUniquelyDueToAloneInCell++;
                            possibleToSetUniquely.Add(new ThreeTupleOfIntegers(i, j, _sudokuPossibleToSetItem.rows[i, j, 0]));
                            totalNumberOfItemsPossibleToSetUniquely++;

                            sb1.Append(string.Format("CAN SET ITEM {0}. The item is alone in cell.", _sudokuPossibleToSetItem.rows[i, j, 0].ToString()));

                            if (totalNumberOfItemsPossibleToSetUniquely == 1)
                            {
                                sb2.Append(string.Format("[{0}, {1}, {2}]", i + 1, j + 1, _sudokuPossibleToSetItem.rows[i, j, 0]));
                            }
                            else
                            {
                                sb2.Append(string.Format(", [{0}, {1}, {2}]", i + 1, j + 1, _sudokuPossibleToSetItem.rows[i, j, 0]));
                            }
                        }
                        else
                        {
                            n = 0;
                            tmpStringBuilder.Clear();

                            for (k = 0; k < _sudokuPossibleToSetItem.numberOfPossibleItemsRows[i, j]; k++)
                            {
                                if (IsItemAloneInRow(i, _sudokuPossibleToSetItem.rows[i, j, k]))
                                {
                                    n = _sudokuPossibleToSetItem.rows[i, j, k];
                                    str = string.IsNullOrEmpty(tmpStringBuilder.ToString()) ? "" : ", ";
                                    tmpStringBuilder.Append(string.Format("{0}item {1} alone possible in row", str, n.ToString()));
                                }

                                if (IsItemAloneInColumn(j, _sudokuPossibleToSetItem.rows[i, j, k]))
                                {
                                    if (n != 0)
                                    {
                                        throw new Exception("Severe error in OneStepSudokuSolver.Process!!");
                                    }

                                    n = _sudokuPossibleToSetItem.rows[i, j, k];
                                    str = string.IsNullOrEmpty(tmpStringBuilder.ToString()) ? "" : ", ";
                                    tmpStringBuilder.Append(string.Format("{0}item {1} alone possible in column", str, n.ToString()));
                                }

                                if (IsItemAloneInSquare(squareIndex, _sudokuPossibleToSetItem.rows[i, j, k]))
                                {
                                    if (n != 0)
                                    {
                                        throw new Exception("Severe error in OneStepSudokuSolver.Process!!");
                                    }

                                    n = _sudokuPossibleToSetItem.rows[i, j, k];
                                    str = string.IsNullOrEmpty(tmpStringBuilder.ToString()) ? "" : ", ";
                                    tmpStringBuilder.Append(string.Format("{0}item {1} alone possible in squre", str, n.ToString()));
                                }

                                if (n != 0)
                                {
                                    numberOfItemsPossibleToSetUniquelyDueToAlonePossibleInRowColumnAndOrSquare++;
                                    possibleToSetUniquely.Add(new ThreeTupleOfIntegers(i, j, n));
                                    totalNumberOfItemsPossibleToSetUniquely++;

                                    if (totalNumberOfItemsPossibleToSetUniquely == 1)
                                    {
                                        sb2.Append(string.Format("[{0}, {1}, {2}]", i + 1, j + 1, n));
                                    }
                                    else
                                    {
                                        sb2.Append(string.Format(", [{0}, {1}, {2}]", i + 1, j + 1, n));
                                    }

                                    sb1.Append(string.Format("CAN SET ITEM {0}. The item is alone possible (in row, column and/or cell), {1}.", n.ToString(), tmpStringBuilder.ToString()));
                                }
                                else
                                {
                                    FillTempIntArray(_sudokuPossibleToSetItem.rows, i, j, _sudokuPossibleToSetItem.numberOfPossibleItemsRows[i, j], tmpIntArray);
                                    sb1.Append("Can not set any number. Item(s) not causing conflict: " + Utility.ReturnString(tmpIntArray, _sudokuPossibleToSetItem.numberOfPossibleItemsRows[i, j]));
                                }
                            }
                        }
                    }
                }
            }

            if (totalNumberOfItemsPossibleToSetUniquely == 0)
            {
                simulated = true;
                result = new ThreeTupleOfIntegers[1];
                SudokuSimulateItem sudokuSimulateItem = new SudokuSimulateItem();
                result[0] = sudokuSimulateItem.ReturnItem(r, _sudokuPossibleToSetItem, out str);
                debugString = _sudokuPossibleToSetItem.ToString() + sb1.ToString() + "\r\n\r\nResult: Simulate item (row, column, item) = " + result[0].ToString();
            }
            else
            {
                result = new ThreeTupleOfIntegers[totalNumberOfItemsPossibleToSetUniquely];

                for(i = 0; i < totalNumberOfItemsPossibleToSetUniquely; i++)
                {
                    result[i] = (ThreeTupleOfIntegers)possibleToSetUniquely[i];
                }

                debugString = _sudokuPossibleToSetItem.ToString() + sb1.ToString() + "\r\n\r\n" + sb2.ToString().Replace("####REPLACE_NUMBER_OF_ITEMS#####", totalNumberOfItemsPossibleToSetUniquely.ToString()) + "}";
            }

            return result;
        }
    }
}
