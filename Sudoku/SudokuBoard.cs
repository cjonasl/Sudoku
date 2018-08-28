using System;
using System.Collections;
using System.Text;

namespace Sudoku
{
    public class SudokuBoard
    {
        private int[,] _rows, _columns, _squares;
        private int _numberOfCellsSet;

        public SudokuBoard()
        {
            _rows = new int[9, 9];
            _columns = new int[9, 9];
            _squares = new int[9, 9];
            Init();
        }

        private void Init()
        {
            _numberOfCellsSet = 0;

            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    _rows[i, j] = 0;
                    _columns[i, j] = 0;
                    _squares[i, j] = 0;
                }
            }
        }

        public int NumberOfCellsSet { get { return _numberOfCellsSet; } }

        private bool RowHasItemInAnyCell(int rowIndex, int item)
        {
            bool returnValue = false;
            int i = 0;

            if ((rowIndex < 0) || (rowIndex > 8))
            {
                throw new Exception("Error in method RowHasItemInAnyCell! Row index must be an integer between 0 and 8.");
            }

            if ((item < 1) || (item > 9))
            {
                throw new Exception("Error in method RowHasItemInAnyCell! Item to check must be an integer between 1 and 9.");
            }

            while ((!returnValue) && (i < 9))
            {
                if (_rows[rowIndex, i] == item)
                {
                    returnValue = true;
                }
                else
                {
                    i++;
                }
            }

            return returnValue;
        }

        private bool ColumnHasItemInAnyCell(int columnIndex, int item)
        {
            bool returnValue = false;
            int i = 0;

            if ((columnIndex < 0) || (columnIndex > 8))
            {
                throw new Exception("Error in method ColumnHasItemInAnyCell! Column index must be an integer between 0 and 8.");
            }

            if ((item < 1) || (item > 9))
            {
                throw new Exception("Error in method ColumnHasItemInAnyCell! Item to check must be an integer between 1 and 9.");
            }

            while ((!returnValue) && (i < 9))
            {
                if (_columns[columnIndex, i] == item)
                {
                    returnValue = true;
                }
                else
                {
                    i++;
                }
            }

            return returnValue;
        }

        private bool SquareHasItemInAnyCell(int squareIndex, int item)
        {
            bool returnValue = false;
            int i = 0;

            if ((squareIndex < 0) || (squareIndex > 8))
            {
                throw new Exception("Error in method SquareHasItemInAnyCell! Square index must be an integer between 0 and 8.");
            }

            if ((item < 1) || (item > 9))
            {
                throw new Exception("Error in method SquareHasItemInAnyCell! Item to check must be an integer between 1 and 9.");
            }

            while ((!returnValue) && (i < 9))
            {
                if (_squares[squareIndex, i] == item)
                {
                    returnValue = true;
                }
                else
                {
                    i++;
                }
            }

            return returnValue;
        }

        public bool CanSetItem(int rowIndex, int columnIndex, int item)
        {
            int squareIndex;

            squareIndex = (3 * (rowIndex / 3)) + (columnIndex / 3);

            return !RowHasItemInAnyCell(rowIndex, item) && !ColumnHasItemInAnyCell(columnIndex, item) && !SquareHasItemInAnyCell(squareIndex, item);
        }

        public void SetItem(int rowIndex, int columnIndex, int item)
        {
            int squareIndex, squareSequenceIndex;

            if (_rows[rowIndex, columnIndex] != 0)
            {
                throw new Exception("Error in method SetItem! An item is already set in row " + (rowIndex + 1).ToString() + " column " + (columnIndex + 1).ToString());
            }

            if ((rowIndex < 0) || (rowIndex > 8))
            {
                throw new Exception("Error in method SetItem! Row index must be an integer between 0 and 8.");
            }

            if ((columnIndex < 0) || (columnIndex > 8))
            {
                throw new Exception("Error in method SetItem! Column index must be an integer between 0 and 8.");
            }

            if ((item < 1) || (item > 9))
            {
                throw new Exception("Error in method SetItem! Item to set must be an integer between 1 and 9.");
            }

            if (RowHasItemInAnyCell(rowIndex, item))
            {
                throw new Exception(string.Format("Item {0} appears more than once in row {1}!", item, rowIndex + 1));
            }

            if (ColumnHasItemInAnyCell(columnIndex, item))
            {
                throw new Exception(string.Format("Item {0} appears more than once in column {1}!", item, columnIndex + 1));
            }

            squareIndex = (3 * (rowIndex / 3)) + (columnIndex / 3);

            if (SquareHasItemInAnyCell(squareIndex, item))
            {
                throw new Exception(string.Format("Item {0} appears more than once in square {1}!", item, squareIndex + 1));
            }

            _rows[rowIndex, columnIndex] = item;
            _columns[columnIndex, rowIndex] = item;

            squareSequenceIndex = (3 * (rowIndex % 3)) + (columnIndex % 3);
            _squares[squareIndex, squareSequenceIndex] = item;

            _numberOfCellsSet++;
        }

        public int ReturnItem(int rowIndex, int columnIndex)
        {
            return _rows[rowIndex, columnIndex];
        }

        public void Set(ThreeTupleOfIntegers[] threeTupleOfIntegersCollection)
        {
            for(int i = 0; i < threeTupleOfIntegersCollection.Length; i++)
            {
                this.SetItem(threeTupleOfIntegersCollection[i].rowIndex, threeTupleOfIntegersCollection[i].columnIndex, threeTupleOfIntegersCollection[i].item);
            }
        }

        public void Set(ThreeTupleOfIntegers items)
        {
            this.SetItem(items.rowIndex, items.columnIndex, items.item);
        }

        public void Reset(ArrayList originalData)
        {
            Init();

            ThreeTupleOfIntegers[] items = new ThreeTupleOfIntegers[originalData.Count];

            for(int i = 0; i < originalData.Count; i++)
            {
                items[i] = (ThreeTupleOfIntegers)originalData[i];
            }

            Set(items);
        }

        public bool ItemIsSet(int rowIndex, int columnIndex)
        {
            return _rows[rowIndex, columnIndex] != 0;
        }

        public bool SudokuIsSolved
        {
            get { return _numberOfCellsSet == 81; }
        }

        public string SudokuBoardString
        {
            get
            {
                StringBuilder sb = new StringBuilder();
                int i, j;

                for (i = 0; i < 9; i++)
                {
                    for (j = 0; j < 9; j++)
                    {
                        if (j == 8)
                        {
                            sb.Append(string.Format("{0}\r\n", _rows[i, j].ToString()));
                        }
                        else
                        {
                            sb.Append(string.Format("{0} ", _rows[i, j].ToString()));
                        }
                    }
                }

                return sb.ToString().TrimEnd();
            }
        }
    }
}
