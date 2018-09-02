using System;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sudoku2
{
    public class Result
    {
        public string errorMessage;
        public bool solvedAlready;
        public bool partiallySolved;
        public bool nothingSolved;
        public bool solved;
        public ArrayList addionalCellsSet;

        public Result(string errorMessage, bool solvedAlready, bool partiallySolved, bool nothingSolved, bool solved, ArrayList addionalCellsSet)
        {
            this.errorMessage = errorMessage;
            this.solvedAlready = solvedAlready;
            this.partiallySolved = partiallySolved;
            this.nothingSolved = nothingSolved;
            this.solved = solved;
            this.addionalCellsSet = addionalCellsSet;
        }
    }

    public struct TwoTupleOfIntegers
    {
        public int x, y;

        public TwoTupleOfIntegers(int x, int y)
        {
            this.x = x;
            this.y = y;
        }
    }

    public class Cell
    {
        public TwoTupleOfIntegers position; //rowIndex, columnIndex
        public int itemValue; //1-9, or 0 if not set
        public ArrayList possibleItemValuesToSet;

        public Cell(TwoTupleOfIntegers position, int itemValue)
        {
            this.position = position;
            this.itemValue = itemValue;
            possibleItemValuesToSet = new ArrayList();
        }
    }

    public class SudokuBoard
    {
        private Cell[][] _cells;
        private ArrayList[] _possibleToSetRow;
        private ArrayList[] _possibleToSetColumn;
        private ArrayList[] _possibleToSetSquare;
        private ArrayList _cellsRemainToSet;
        private TwoTupleOfIntegers[][] _squareIndex;

        public SudokuBoard()
        {
            int i, j, r, c;

            for (i = 0; i < 9; i++)
            {
                _cells = new Cell[9][];
                _possibleToSetRow[i] = new ArrayList();
                _possibleToSetColumn[i] = new ArrayList();
                _possibleToSetSquare[i] = new ArrayList();

                for (j = 0; j < 9; j++)
                {
                    if (i == 0)
                    {
                        _squareIndex = new TwoTupleOfIntegers[j][];
                        r = j / 3;
                        c = j % 3;
                        _squareIndex[j] = new TwoTupleOfIntegers[] { new TwoTupleOfIntegers(3 * r, c), new TwoTupleOfIntegers(3 * r, c + 1), new TwoTupleOfIntegers(3 * r, c + 2), new TwoTupleOfIntegers(3 * r + 1, c), new TwoTupleOfIntegers(3 * r + 1, c + 1), new TwoTupleOfIntegers(3 * r + 1, c + 2), new TwoTupleOfIntegers(3 * r + 2, c), new TwoTupleOfIntegers(3 * r + 2, c + 1), new TwoTupleOfIntegers(3 * r + 2, c + 2) };
                    }

                    _cells[j] = new Cell[9];                
                }
            }

            for (i = 0; i < 9; i++)
            {

                for (j = 0; j < 9; j++)
                {
                    _cells[i][j] = new Cell(new TwoTupleOfIntegers(0, 0), 0);
                }
            }

            _cellsRemainToSet = new ArrayList();
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

        private bool CanSetItemValueInCell(Cell cell)
        {
            int rowIndex = cell.position.x;
            int columnIndex = cell.position.y;
            int squareIndex = (3 * (rowIndex / 3)) + (columnIndex / 3);
            int itemValue = cell.itemValue;

            return !RowHasItemValueInAnyCell(rowIndex, itemValue) && !ColumnHasItemValueInAnyCell(columnIndex, itemValue) && !SquareHasItemValueInAnyCell(squareIndex, itemValue);
        }

        public string SetCell(Cell cell)
        {
            int rowIndex = cell.position.x;
            int columnIndex = cell.position.y;
            int squareIndex = (3 * (rowIndex / 3)) + (columnIndex / 3);
            int itemValue = cell.itemValue;

            if (RowHasItemValueInAnyCell(rowIndex, itemValue))
            {
                return string.Format("Number {0} appears more than once in row {1}!", itemValue, rowIndex + 1);
            }

            if (ColumnHasItemValueInAnyCell(columnIndex, itemValue))
            {
                return string.Format("Number {0} appears more than once in column {1}!", itemValue, columnIndex + 1);
            }

            if (SquareHasItemValueInAnyCell(columnIndex, itemValue))
            {
                return string.Format("Number {0} appears more than once in square {1}!", itemValue, squareIndex + 1);
            }

            _cells[rowIndex][columnIndex].itemValue = itemValue;

            return null;
        }

        private string SetData(ArrayList data)
        {
            int i, j;
            string returnValue = null;

            for(i = 0; i < 9; i++)
            {
                for (j = 0; j < 9; j++)
                {
                    _cells[i][j].itemValue = 0;
                }
            }

            i = 0;

            while ((i < data.Count) && (returnValue == null))
            {
                returnValue = SetCell((Cell)data[i]);
                i++;
            }

            return returnValue;
        }

        private ArrayList ReturnCellsRemainToSet()
        {
            ArrayList cellsRemainToSet = new ArrayList();

            for(int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    if (_cells[i][j].itemValue == 0)
                    {
                        cellsRemainToSet.Add(_cells[i][j]);
                    }
                }
            }

            return cellsRemainToSet;
        }

        private void SetStructure()
        {
            int i, rowIndex, columnIndex, squareIndex, itemValue;
            Cell tmpCell = new Cell(new TwoTupleOfIntegers(0, 0), 0);

            for(i = 0; i < 9; i++)
            {
                _possibleToSetRow[i].Clear();
                _possibleToSetColumn[i].Clear();
                _possibleToSetSquare[i].Clear();
            }

            for (rowIndex = 0; rowIndex < 9; rowIndex++)
            {
                for (columnIndex = 0; columnIndex < 9; columnIndex++)
                {
                    if (_cells[rowIndex][columnIndex].itemValue == 0)
                    {
                        _cells[rowIndex][columnIndex].possibleItemValuesToSet.Clear();
                    }

                    tmpCell.position.x = rowIndex;
                    tmpCell.position.y = columnIndex;

                    squareIndex = (3 * (rowIndex / 3)) + (columnIndex / 3);

                    for (itemValue = 1; itemValue <= 9; itemValue++)
                    {
                        tmpCell.itemValue = itemValue;

                        if (CanSetItemValueInCell(tmpCell))
                        {
                            _cells[rowIndex][columnIndex].possibleItemValuesToSet.Add(itemValue);
                            _possibleToSetRow[rowIndex].Add(itemValue);
                            _possibleToSetColumn[columnIndex].Add(itemValue);
                            _possibleToSetSquare[squareIndex].Add(itemValue);
                        }
                    }
                }
            }
        }

        private ArrayList ReturnData(ArrayList originalData)
        {
            ArrayList data = new ArrayList();
            Cell tmpCell;
            TwoTupleOfIntegers position;
            int itemValue;

            for (int i = 0; i < originalData.Count; i++)
            {
                tmpCell = (Cell)originalData[i];
                position = new TwoTupleOfIntegers(tmpCell.position.x, tmpCell.position.y);
                itemValue = tmpCell.itemValue;
                data.Add(new Cell(position, itemValue));
            }

            return data;
        }

        public Result Process(ArrayList originalData, int maxNumberOfTries)
        {
            ArrayList data, setData, tmpSetData, cellsRemainToSet;
            int i, numberOfTries = 0;
            bool atLeastOneItemSet;
            string errorMessage;

            data = ReturnData(originalData);

            errorMessage = SetData(data);

            if (errorMessage != null)
                return new Result(errorMessage, false, false, true, false, null);

           if (data.Count == 81)
                return new Result(null, true, false, true, true, null);

            tmpSetData = new ArrayList();

            while (((data.Count + tmpSetData.Count) < 81) && (numberOfTries < maxNumberOfTries))
            {
                numberOfTries++;

                if (numberOfTries > 1)
                {
                    SetData(data);
                }

                cellsRemainToSet = ReturnCellsRemainToSet();
                SetStructure();
                atLeastOneItemSet = true;

                while (((data.Count + tmpSetData.Count) < 81) && atLeastOneItemSet)
                {
                    atLeastOneItemSet = false;

                    for(i = 0; i < cellsRemainToSet.Count; i++)
                    {
                        
                    }
                }
            }
        }
    }


    public class Sudoku2
    {

    }
}
