using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sudoku2
{
    public class DebugSudoku2
    {
        private Cell[][] _cells;
        private int[][] _numberOfItemsPossibleToSetRow;
        private int[][] _numberOfItemsPossibleToSetColumn;
        private int[][] _numberOfItemsPossibleToSetSquare;
        private TwoTupleOfIntegers[][] _squareIndex;

        public DebugSudoku2()
        {
            int i, j;

            _cells = new Cell[9][];
            _numberOfItemsPossibleToSetRow = new int[9][];
            _numberOfItemsPossibleToSetColumn = new int[9][];
            _numberOfItemsPossibleToSetSquare = new int[9][];
            _squareIndex = new TwoTupleOfIntegers[9][];

            for (i = 0; i < 9; i++)
            {
                _squareIndex[i] = new TwoTupleOfIntegers[9];
                _cells[i] = new Cell[9];
                _numberOfItemsPossibleToSetRow[i] = new int[9];
                _numberOfItemsPossibleToSetColumn[i] = new int[9];
                _numberOfItemsPossibleToSetSquare[i] = new int[9];
            }

            for (i = 0; i < 9; i++)
            {
                for (j = 0; j < 9; j++)
                {
                    _squareIndex[i][j] = new TwoTupleOfIntegers((3 * (i / 3)) + (j / 3), (3 * (i % 3)) + (j % 3));
                }
            }

            for (i = 0; i < 9; i++)
            {
                for (j = 0; j < 9; j++)
                {
                    _cells[i][j] = new Cell(new TwoTupleOfIntegers(i, j), 0, false);
                }
            }
        }

        private bool RowHasItemValueInAnyCell(int FixedRowIndex, int itemValue)
        {
            bool returnValue = false;
            int variableColumnIndex = 0;

            while ((!returnValue) && (variableColumnIndex < 9))
            {
                if (_cells[FixedRowIndex][variableColumnIndex].itemValue == itemValue)
                {
                    returnValue = true;
                }
                else
                {
                    variableColumnIndex++;
                }
            }

            return returnValue;
        }

        private bool ColumnHasItemValueInAnyCell(int fixedColumnIndex, int itemValue)
        {
            bool returnValue = false;
            int variableRowIndex = 0;

            while ((!returnValue) && (variableRowIndex < 9))
            {
                if (_cells[variableRowIndex][fixedColumnIndex].itemValue == itemValue)
                {
                    returnValue = true;
                }
                else
                {
                    variableRowIndex++;
                }
            }

            return returnValue;
        }

        private bool SquareHasItemValueInAnyCell(int fixedSquareIndex, int itemValue)
        {
            bool returnValue = false;
            int variableRowIndex, variableColumnIndex, variableSquareSequenceIndex = 0;

            while ((!returnValue) && (variableSquareSequenceIndex < 9))
            {
                variableRowIndex = _squareIndex[fixedSquareIndex][variableSquareSequenceIndex].x;
                variableColumnIndex = _squareIndex[fixedSquareIndex][variableSquareSequenceIndex].y;

                if (_cells[variableRowIndex][variableColumnIndex].itemValue == itemValue)
                {
                    returnValue = true;
                }
                else
                {
                    variableSquareSequenceIndex++;
                }
            }

            return returnValue;
        }

        private bool CanSetItemValueInCell(int rowIndex, int columnIndex, int itemValue)
        {
            int squareIndex = (3 * (rowIndex / 3)) + (columnIndex / 3);
            return !RowHasItemValueInAnyCell(rowIndex, itemValue) && !ColumnHasItemValueInAnyCell(columnIndex, itemValue) && !SquareHasItemValueInAnyCell(squareIndex, itemValue);
        }

        private string DebugReturnNumberOfItemsPossibleToSetAsString()
        {
            int i;
            StringBuilder sb = new StringBuilder();

            for (i = 0; i < 9; i++)
            {
                sb.Append(Sudoku.Utility.ReturnString(_numberOfItemsPossibleToSetRow[i], 9, '[', ']'));
            }

            for (i = 0; i < 9; i++)
            {
                sb.Append(Sudoku.Utility.ReturnString(_numberOfItemsPossibleToSetColumn[i], 9, '[', ']'));
            }

            for (i = 0; i < 9; i++)
            {
                sb.Append(Sudoku.Utility.ReturnString(_numberOfItemsPossibleToSetSquare[i], 9, '[', ']'));
            }

            return sb.ToString().TrimEnd();
        }

        public string ReturnNumberOfItemsPossibleToSetAsString(Cell[][] cells)
        {
            int i, rowIndex, columnIndex, squareIndex, itemValue;

            for(rowIndex = 0; rowIndex < 9; rowIndex++)
            {
                for (columnIndex = 0; columnIndex < 9; columnIndex++)
                {
                    _cells[rowIndex][columnIndex].itemValue = cells[rowIndex][columnIndex].itemValue;
                }
            }

            for (i = 0; i < 9; i++)
            {
                for (itemValue = 1; itemValue <= 9; itemValue++)
                {
                    _numberOfItemsPossibleToSetRow[i][itemValue - 1] = 0;
                    _numberOfItemsPossibleToSetColumn[i][itemValue - 1] = 0;
                    _numberOfItemsPossibleToSetSquare[i][itemValue - 1] = 0;
                }
            }

            for (rowIndex = 0; rowIndex < 9; rowIndex++)
            {
                for (columnIndex = 0; columnIndex < 9; columnIndex++)
                {
                    squareIndex = (3 * (rowIndex / 3)) + (columnIndex / 3);

                    if (_cells[rowIndex][columnIndex].itemValue == 0)
                    {
                        for (itemValue = 1; itemValue <= 9; itemValue++)
                        {
                            if (CanSetItemValueInCell(rowIndex, columnIndex, itemValue))
                            {
                                _numberOfItemsPossibleToSetRow[rowIndex][itemValue - 1]++;
                                _numberOfItemsPossibleToSetColumn[columnIndex][itemValue - 1]++;
                                _numberOfItemsPossibleToSetSquare[squareIndex][itemValue - 1]++;
                            }
                        }
                    }
                }
            }

            return DebugReturnNumberOfItemsPossibleToSetAsString();
        }
    }
}
