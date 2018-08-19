using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sudoku
{
    public class SudokuBoard
    {
        private int[,] _rows, _columns, _squares;
        private int _numberOfBoardEntriesSet;

        public SudokuBoard()
        {
            _rows = new int[9, 9];
            _columns = new int[9, 9];
            _squares = new int[9, 9];
            _numberOfBoardEntriesSet = 0;

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

        private bool RowHasNumberInAnyEntry(int rowIndex, int number)
        {
            bool returnValue = false;
            int i = 0;

            if ((rowIndex < 0) || (rowIndex > 8))
            {
                throw new Exception("Error in method RowHasNumberInAnyEntry! Row index must be an integer between 0 and 8.");
            }

            if ((number < 1) || (number > 9))
            {
                throw new Exception("Error in method RowHasNumberInAnyEntry! Number to check must be an integer between 1 and 9.");
            }

            while ((!returnValue) && (i < 9))
            {
                if (_rows[rowIndex, i] == number)
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

        private bool ColumnHasNumberInAnyEntry(int columnIndex, int number)
        {
            bool returnValue = false;
            int i = 0;

            if ((columnIndex < 0) || (columnIndex > 8))
            {
                throw new Exception("Error in method ColumnHasNumberInAnyEntry! Column index must be an integer between 0 and 8.");
            }

            if ((number < 1) || (number > 9))
            {
                throw new Exception("Error in method ColumnHasNumberInAnyEntry! Number to check must be an integer between 1 and 9.");
            }

            while ((!returnValue) && (i < 9))
            {
                if (_columns[columnIndex, i] == number)
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

        private bool SquareHasNumberInAnyEntry(int squareIndex, int number)
        {
            bool returnValue = false;
            int i = 0;

            if ((squareIndex < 0) || (squareIndex > 8))
            {
                throw new Exception("Error in method SquareHasNumberInAnyEntry! Square index must be an integer between 0 and 8.");
            }

            if ((number < 1) || (number > 9))
            {
                throw new Exception("Error in method SquareHasNumberInAnyEntry! Number to check must be an integer between 1 and 9.");
            }

            while ((!returnValue) && (i < 9))
            {
                if (_squares[squareIndex, i] == number)
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

        public bool CanSetNumber(int rowIndex, int columnIndex, int number)
        {
            int squareIndex;

            squareIndex = (3 * (rowIndex / 3)) + (columnIndex / 3);

            return !RowHasNumberInAnyEntry(rowIndex, number) && !ColumnHasNumberInAnyEntry(columnIndex, number) && !SquareHasNumberInAnyEntry(squareIndex, number);
        }

        public void SetNumber(int rowIndex, int columnIndex, int number)
        {
            int squareIndex, squareSequenceIndex;

            if (_rows[rowIndex, columnIndex] != 0)
            {
                throw new Exception("Error in method SetNumber! A number is already set in row " + (rowIndex + 1).ToString() + " column " + (columnIndex + 1).ToString());
            }

            if ((rowIndex < 0) || (rowIndex > 8))
            {
                throw new Exception("Error in method SetNumber! Row index must be an integer between 0 and 8.");
            }

            if ((columnIndex < 0) || (columnIndex > 8))
            {
                throw new Exception("Error in method SetNumber! Column index must be an integer between 0 and 8.");
            }

            if ((number < 1) || (number > 9))
            {
                throw new Exception("Error in method SetNumber! Number to set must be an integer between 1 and 9.");
            }

            if (RowHasNumberInAnyEntry(rowIndex, number))
            {
                throw new Exception(string.Format("Number {0} appears more than once in row {1}!", number, rowIndex + 1));
            }

            if (ColumnHasNumberInAnyEntry(columnIndex, number))
            {
                throw new Exception(string.Format("Number {0} appears more than once in column {1}!", number, columnIndex + 1));
            }

            squareIndex = (3 * (rowIndex / 3)) + (columnIndex / 3);

            if (SquareHasNumberInAnyEntry(squareIndex, number))
            {
                throw new Exception(string.Format("Number {0} appears more than once in square {1}!", number, squareIndex + 1));
            }

            _rows[rowIndex, columnIndex] = number;
            _columns[columnIndex, rowIndex] = number;

            squareSequenceIndex = (3 * (rowIndex % 3)) + (columnIndex % 3);
            _squares[squareIndex, squareSequenceIndex] = number;

            _numberOfBoardEntriesSet++;
        }

        public int ReturnNumber(int rowIndex, int columnIndex)
        {
            return _rows[rowIndex, columnIndex];
        }

        public bool NumberIsSet(int rowIndex, int columnIndex)
        {
            return _rows[rowIndex, columnIndex] != 0;
        }

        public bool SudokuIsSolved
        {
            get { return _numberOfBoardEntriesSet == 81; }
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
