using System;
using System.Collections;
using System.Configuration;
using System.IO;
using System.Text;


namespace Sudoku
{
    public class OneStepSudokuSolver
    {
        public SudokuPossibleToSetItem sudokuPossibleToSetItem;
        public int totalNumberOfItemsPossibleToSetUniquely;
        public int numberOfItemsPossibleToSetUniquelyDueToAloneInCell;
        public int numberOfItemsPossibleToSetUniquelyDueToAloneItemValue;
        public int[,] possibleToSetUniquely;

        private int _numberOfCallsToProcess;
        private string _debugDirectory;

        public OneStepSudokuSolver()
        {
            sudokuPossibleToSetItem = new SudokuPossibleToSetItem();
            possibleToSetUniquely = new int[81, 3];
            _numberOfCallsToProcess = 0;
            _debugDirectory = ConfigurationManager.AppSettings["DebugDirectory"] + "\\OneStepSudokuSolver";

            if (!Directory.Exists(_debugDirectory))
            {
                Directory.CreateDirectory(_debugDirectory);
            }
            else
            {
                string[] files = Directory.GetFiles(_debugDirectory);

                foreach (string file in files)
                {
                    File.Delete(file);
                }
            }
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

        public void Process(SudokuBoard sudokuBoard, out bool errorFound)
        {
            int i, j, k, squareIndex, numberOfNumbersPossibleToSet;
            string[,] debug = new string[9, 9];
            string rowColumn, fileNameFullPath;
            StringBuilder sb = new StringBuilder();
            ArrayList tmpArrayList = new ArrayList();
            int[] tmpIntArray = new int[9];

            _numberOfCallsToProcess++;

            errorFound = false;

            sudokuPossibleToSetItem.Process(sudokuBoard);

            for (i = 0; i < 9; i++)
            {
                for (j = 0; j < 9; j++)
                {
                    rowColumn = string.Format("[{0},{1}]: ", (i + 1).ToString(), (j + 1).ToString());

                    squareIndex = (3 * (i / 3)) + (j / 3);

                    if (!sudokuBoard.NumberIsSet(i, j))
                    {
                        if (sudokuPossibleToSetItem.numberOfPossibleItemsRows[i, j] == 0)
                        {
                            debug[i, j] = rowColumn + "ERROR!! Not possible to set any item in cell that does not cause conflict!";
                        }
                        else if (sudokuPossibleToSetItem.numberOfPossibleItemsRows[i, j] == 1)
                        {
                            numberOfItemsPossibleToSetUniquelyDueToAloneInCell++;
                            possibleToSetUniquely[totalNumberOfItemsPossibleToSetUniquely, 0] = i;
                            possibleToSetUniquely[totalNumberOfItemsPossibleToSetUniquely, 1] = j;
                            possibleToSetUniquely[totalNumberOfItemsPossibleToSetUniquely, 2] = sudokuPossibleToSetItem.rows[i, j, 0];
                            totalNumberOfItemsPossibleToSetUniquely++;

                            debug[i, j] = rowColumn + string.Format("CAN SET ITEM {0}. The item is alone in cell.", sudokuPossibleToSetItem.rows[i, j, 0].ToString());
                        }
                        else
                        {
                            numberOfNumbersPossibleToSet = 0;
                            sb.Clear();
                            tmpArrayList.Clear();

                            for (k = 0; k < sudokuPossibleToSetItem.numberOfPossibleItemsRows[i, j]; k++)
                            {
                                if (IsItemAloneInRow(i, sudokuPossibleToSetItem.rows[i, j, k]))
                                {
                                    sb.Append(string.Format(", item {0} alone possible in row", sudokuPossibleToSetItem.rows[i, j, k].ToString()));
                                }

                                if (IsItemAloneInColumn(j, sudokuPossibleToSetItem.rows[i, j, k]))
                                {
                                    if (tmpArrayList.IndexOf(sudokuPossibleToSetItem.rows[i, j, k]) == -1)
                                    {
                                        tmpArrayList.Add(sudokuPossibleToSetItem.rows[i, j, k]);
                                        numberOfNumbersPossibleToSet++;
                                    }

                                    sb.Append(string.Format(", number {0} alone possible in column", sudokuPossibleToSetItem.rows[i, j, k].ToString()));
                                }

                                if (IsItemAloneInSquare(squareIndex, sudokuPossibleToSetItem.rows[i, j, k]))
                                {
                                    if (tmpArrayList.IndexOf(sudokuPossibleToSetItem.rows[i, j, k]) == -1)
                                    {
                                        tmpArrayList.Add(sudokuPossibleToSetItem.rows[i, j, k]);
                                        numberOfNumbersPossibleToSet++;
                                    }

                                    sb.Append(string.Format(", item {0} alone possible in square", sudokuPossibleToSetItem.rows[i, j, k].ToString()));
                                }
                            }

                            FillTempIntArray(sudokuPossibleToSetItem.rows, i, j, sudokuPossibleToSetItem.numberOfPossibleItemsRows[i, j], tmpIntArray);

                            if (numberOfNumbersPossibleToSet == 0)
                            {
                                debug[i, j] = rowColumn + "Can not set any number. Numbers not causing conflict: " + Utility.ReturnString(tmpIntArray, sudokuPossibleToSetItem.numberOfPossibleItemsRows[i, j]);
                            }
                            else if (numberOfNumbersPossibleToSet == 1)
                            {
                                numberOfItemsPossibleToSetUniquelyDueToAloneItemValue++;
                                possibleToSetUniquely[totalNumberOfItemsPossibleToSetUniquely, 0] = i;
                                possibleToSetUniquely[totalNumberOfItemsPossibleToSetUniquely, 1] = j;
                                possibleToSetUniquely[totalNumberOfItemsPossibleToSetUniquely, 2] = (int)tmpArrayList[0];
                                totalNumberOfItemsPossibleToSetUniquely++;

                                debug[i, j] = rowColumn + "CAN SET ITEM " + tmpArrayList[0].ToString() + ". Items not causing conflict: " + Utility.ReturnString(tmpIntArray, sudokuPossibleToSetItem.numberOfPossibleItemsRows[i, j]) + sb.ToString();
                            }
                            else
                            {
                                errorFound = true;
                                debug[i, j] = rowColumn + "ERROR!! Can set more than one item: " + Utility.ReturnString(tmpArrayList) + ". Items not causing conflict: " + Utility.ReturnString(tmpIntArray, sudokuPossibleToSetItem.numberOfPossibleItemsRows[i, j]) + ", " + sb.ToString();
                            }
                        }
                    }
                    else
                    {
                        debug[i, j] = rowColumn + string.Format("Board cell is already set with item {0}", sudokuBoard.ReturnNumber(i, j).ToString());
                    }
                }
            }

            sb.Clear();

            for (i = 0; i < 9; i++)
            {
                for (j = 0; j < 9; j++)
                {
                    sb.Append(debug[i, j] + "\r\n");
                }

                if (i != 8)
                {
                    sb.Append("\r\n");
                }
            }

            fileNameFullPath = _debugDirectory + "\\" + string.Format("Process{0} TotalPossible {1} AloneCell {2} AloneItemValue {3}.txt",
                _numberOfCallsToProcess.ToString(),
                sudokuPossibleToSetItem.totalNumberOfItemsPossibleToSet.ToString(),
                numberOfItemsPossibleToSetUniquelyDueToAloneInCell.ToString(),
                numberOfItemsPossibleToSetUniquelyDueToAloneItemValue.ToString());

            Utility.CreateNewFile(fileNameFullPath, sb.ToString());
        }
    }
}
