using System;
using System.Collections;
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
                for (j = 0; j < _sudokuPossibleToSetItem.numberOfPossibleItemsRows[rowIndex, i]; j++)
                {
                    if (_sudokuPossibleToSetItem.rows[rowIndex, i, j] == item)
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
                for (j = 0; j < _sudokuPossibleToSetItem.numberOfPossibleItemsColumns[columnIndex, i]; j++)
                {
                    if (_sudokuPossibleToSetItem.columns[columnIndex, i, j] == item)
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
                for (j = 0; j < _sudokuPossibleToSetItem.numberOfPossibleItemsSquares[squareIndex, i]; j++)
                {
                    if (_sudokuPossibleToSetItem.squares[squareIndex, i, j] == item)
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

        public ThreeTupleOfIntegers[] Process(
            SudokuBoard sudokuBoard,
            Random r,
            bool isDebug,
            out int numberOfItemsitemsSetDueToAloneInCell,
            out int numberOfItemsitemsSetDueToAlonePossibleInRowColumnAndOrSquare,
            out int totalNumberOfItemsPossibleToSetWithoutCausingConflict,
            out int numberOfErrorsNotPossibleToSetAnyItemInCell,
            out int numberOfErrorsNotUniqueItemAlonePossible,
            out bool simulated,
            out string debugString)
        {
            int i, j, k, n, numberOfItemsSetInIteration, squareIndex;
            string str = "", commaSeparatedStringOfIntegers;
            StringBuilder sb1, sb2, tmpStringBuilder;
            ArrayList itemsSet = new ArrayList();
            ArrayList itemsPossibleToSetDueToAlonePossibleInRowColumnAndOrSquare = new ArrayList();
            ThreeTupleOfIntegers[] result;
            ThreeTupleOfIntegers item;
            int[] tmpIntArray = new int[9];

            numberOfItemsSetInIteration = 0;
            numberOfItemsitemsSetDueToAloneInCell = 0;
            numberOfItemsitemsSetDueToAlonePossibleInRowColumnAndOrSquare = 0;
            numberOfErrorsNotPossibleToSetAnyItemInCell = 0;
            numberOfErrorsNotUniqueItemAlonePossible = 0;
            simulated = false;

            _sudokuPossibleToSetItem.Process(sudokuBoard);
            totalNumberOfItemsPossibleToSetWithoutCausingConflict = _sudokuPossibleToSetItem.totalNumberOfItemsPossibleToSetWithoutCausingConflict;

            if (totalNumberOfItemsPossibleToSetWithoutCausingConflict == 0)
            {
                debugString = "Not possible to set anything!!";
                return null;
            }

            sb1 = new StringBuilder();
            sb2 = new StringBuilder("Result: The following ####REPLACE_NUMBER_OF_ITEMS##### item(s) are possible to set, {row, column, item}:\r\n\r\n");
            tmpStringBuilder = new StringBuilder();

            sb1.Append("\r\n------------------------Find cells to set------------------------\r\n\r\n");

            for (i = 0; i < 9; i++)
            {
                for (j = 0; j < 9; j++)
                {
                    item = null;
                    sb1.Append(string.Format("[{0}, {1}]: ", i + 1, j + 1));
                    squareIndex = (3 * (i / 3)) + (j / 3);

                    if (sudokuBoard.ItemIsSet(i, j))
                    {
                        sb1.Append(string.Format("Cell already set with item {0}.\r\n", sudokuBoard.ReturnItem(i, j).ToString()));
                    }
                    else
                    {
                        if (_sudokuPossibleToSetItem.numberOfPossibleItemsRows[i, j] == 0)
                        {
                            sb1.Append("ERROR!! Not possible to set any item in cell that does not cause conflict!\r\n");
                            numberOfErrorsNotPossibleToSetAnyItemInCell++;
                        }
                        else if (_sudokuPossibleToSetItem.numberOfPossibleItemsRows[i, j] == 1)
                        {
                            numberOfItemsitemsSetDueToAloneInCell++;
                            item = new ThreeTupleOfIntegers(i, j, _sudokuPossibleToSetItem.rows[i, j, 0]);
                            itemsSet.Add(item);
                            numberOfItemsSetInIteration++;

                            sb1.Append(string.Format("CAN SET ITEM {0}. The item is alone in cell.\r\n", _sudokuPossibleToSetItem.rows[i, j, 0].ToString()));
                            sb2.Append("{" + string.Format("{0}, {1}, {2}", i + 1, j + 1, _sudokuPossibleToSetItem.rows[i, j, 0]) + "}\r\n");
                        }
                        else
                        {
                            tmpStringBuilder.Clear();
                            itemsPossibleToSetDueToAlonePossibleInRowColumnAndOrSquare.Clear();
                            
                            for (k = 0; k < _sudokuPossibleToSetItem.numberOfPossibleItemsRows[i, j]; k++)
                            {
                                n = _sudokuPossibleToSetItem.rows[i, j, k];

                                if (IsItemAloneInRow(i, n))
                                {
                                    Utility.AddIfNotExistsAlready(itemsPossibleToSetDueToAlonePossibleInRowColumnAndOrSquare, _sudokuPossibleToSetItem.rows[i, j, k]);
                                    tmpStringBuilder.Append(string.Format("Item {0} alone possible in row. ", n.ToString()));
                                }

                                if (IsItemAloneInColumn(j, n))
                                {
                                    Utility.AddIfNotExistsAlready(itemsPossibleToSetDueToAlonePossibleInRowColumnAndOrSquare, _sudokuPossibleToSetItem.rows[i, j, k]);
                                    str = string.IsNullOrEmpty(tmpStringBuilder.ToString()) ? "" : ", ";
                                    tmpStringBuilder.Append(string.Format("Item {0} alone possible in column. ", n.ToString()));
                                }

                                if (IsItemAloneInSquare(squareIndex, n))
                                {
                                    Utility.AddIfNotExistsAlready(itemsPossibleToSetDueToAlonePossibleInRowColumnAndOrSquare, _sudokuPossibleToSetItem.rows[i, j, k]);
                                    tmpStringBuilder.Append(string.Format("Item {0} alone possible in squre. ", n.ToString()));
                                }
                            }

                            if (itemsPossibleToSetDueToAlonePossibleInRowColumnAndOrSquare.Count == 0)
                            {
                                FillTempIntArray(_sudokuPossibleToSetItem.rows, i, j, _sudokuPossibleToSetItem.numberOfPossibleItemsRows[i, j], tmpIntArray);
                                sb1.Append(string.Format("Unable to set item. Item(s) not causing conflict: {0}.\r\n", Utility.ReturnString(tmpIntArray, _sudokuPossibleToSetItem.numberOfPossibleItemsRows[i, j])));
                            }
                            else
                            {
                                n = (int)itemsPossibleToSetDueToAlonePossibleInRowColumnAndOrSquare[0];

                                if (itemsPossibleToSetDueToAlonePossibleInRowColumnAndOrSquare.Count == 1)
                                    numberOfItemsitemsSetDueToAlonePossibleInRowColumnAndOrSquare++;
                                else
                                    numberOfErrorsNotUniqueItemAlonePossible++;

                                item = new ThreeTupleOfIntegers(i, j, n);
                                itemsSet.Add(item);

                                numberOfItemsSetInIteration++;

                                sb2.Append("{" + string.Format("{0}, {1}, {2}", i + 1, j + 1, n) + "}\r\n");

                                if (itemsPossibleToSetDueToAlonePossibleInRowColumnAndOrSquare.Count == 1)
                                    sb1.Append(string.Format("CAN SET ITEM {0}. The item is alone possible (in row, column and/or cell). {1} \r\n", n.ToString(), tmpStringBuilder.ToString()));
                                else
                                {
                                    commaSeparatedStringOfIntegers = Utility.ReturnCommaSeparatedStringOfIntegers(itemsPossibleToSetDueToAlonePossibleInRowColumnAndOrSquare);
                                    sb1.Append(string.Format("ERROR!! Not unique alone possible. The following items are possible to set with that rule: {0}. Anyway, set item {1}. {2}\r\n", commaSeparatedStringOfIntegers, n.ToString(), tmpStringBuilder.ToString()));
                                }
                            }
                        }
                    }

                    if (item != null)
                    {
                        sudokuBoard.Set(item);
                        _sudokuPossibleToSetItem.Process(sudokuBoard);
                    }
                }

                sb1.Append("\r\n");
            }

            if (numberOfItemsSetInIteration == 0)
            {
                simulated = true;
                result = new ThreeTupleOfIntegers[1];
                SudokuSimulateItem sudokuSimulateItem = new SudokuSimulateItem();
                result[0] = sudokuSimulateItem.ReturnItem(r, _sudokuPossibleToSetItem, out str);
                sudokuBoard.Set(result[0]);
            }
            else
            {
                result = new ThreeTupleOfIntegers[numberOfItemsSetInIteration];

                for(i = 0; i < numberOfItemsSetInIteration; i++)
                {
                    result[i] = (ThreeTupleOfIntegers)itemsSet[i];
                }

                if (isDebug)
                {
                    SudokuSimulateItem sudokuSimulateItem = new SudokuSimulateItem();
                    ThreeTupleOfIntegers tmpThreeTupleOfIntegers = sudokuSimulateItem.ReturnItem(r, _sudokuPossibleToSetItem, out str);
                }
            }

            if (simulated)
            {
                debugString = _sudokuPossibleToSetItem.ToString() + sb1.ToString() + "\r\n\r\n----------------------------------- Simulate one item -----------------------------------\r\n\r\n" + str + "\r\n\r\n---------- Sudoku board after iteration ----------\r\n\r\n" + sudokuBoard.SudokuBoardString;
            }
            else
            {
                if (!isDebug)
                    debugString = _sudokuPossibleToSetItem.ToString() + sb1.ToString() + sb2.ToString().Replace("####REPLACE_NUMBER_OF_ITEMS#####", numberOfItemsSetInIteration.ToString()).TrimEnd() + "\r\n\r\n---------- Sudoku board after iteration ----------\r\n\r\n" + sudokuBoard.SudokuBoardString;
                else
                {
                    debugString = _sudokuPossibleToSetItem.ToString() + sb1.ToString() + sb2.ToString().Replace("####REPLACE_NUMBER_OF_ITEMS#####", numberOfItemsSetInIteration.ToString()).TrimEnd() + "\r\n\r\n---------- Sudoku board after iteration ----------\r\n\r\n" + sudokuBoard.SudokuBoardString + "\r\n\r\n----------------------------------- DEBUG Simulate one item -----------------------------------\r\n\r\n" + str;
                }
            }

            return result;
        }
    }
}
