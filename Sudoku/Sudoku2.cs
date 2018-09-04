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
        public string sudokuString;

        public Result(string errorMessage, bool solvedAlready, bool partiallySolved, bool nothingSolved, bool solved, string sudokuString)
        {
            this.errorMessage = errorMessage;
            this.solvedAlready = solvedAlready;
            this.partiallySolved = partiallySolved;
            this.nothingSolved = nothingSolved;
            this.solved = solved;
            this.sudokuString = sudokuString;
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

        public override string ToString()
        {
            return string.Format("[{0}, {1}]", x.ToString(), y.ToString());
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
        private int[][] _numberOfItemsPossibleToSetRow;
        private int[][] _numberOfItemsPossibleToSetColumn;
        private int[][] _numberOfItemsPossibleToSetSquare;
        private int _totalNumberOfItemsPossibleToSet;
        private ArrayList _cellsRemainToSet, _dataSetBeforeSimulationHasBeenDone;
        private bool _simulationHasBeenDone;
        private TwoTupleOfIntegers[][] _squareIndex;

        public SudokuBoard()
        {
            int i, j, r, c;

            _cells = new Cell[9][];
            _numberOfItemsPossibleToSetRow = new int[9][];
            _numberOfItemsPossibleToSetColumn = new int[9][];
            _numberOfItemsPossibleToSetSquare = new int[9][];
            _squareIndex = new TwoTupleOfIntegers[9][];

            for (i = 0; i < 9; i++)
            {
                r = i / 3;
                c = i % 3;
                _squareIndex[i] = new TwoTupleOfIntegers[] { new TwoTupleOfIntegers(3 * r, 3 * c), new TwoTupleOfIntegers(3 * r, 3 * c + 1), new TwoTupleOfIntegers(3 * r, 3 * c + 2), new TwoTupleOfIntegers(3 * r + 1, 3 * c), new TwoTupleOfIntegers(3 * r + 1, 3 * c + 1), new TwoTupleOfIntegers(3 * r + 1, 3 * c + 2), new TwoTupleOfIntegers(3 * r + 2, 3 * c), new TwoTupleOfIntegers(3 * r + 2, 3 * c + 1), new TwoTupleOfIntegers(3 * r + 2, 3 * c + 2) };
                _cells[i] = new Cell[9];
                _numberOfItemsPossibleToSetRow[i] = new int[9];
                _numberOfItemsPossibleToSetColumn[i] = new int[9];
                _numberOfItemsPossibleToSetSquare[i] = new int[9];
            }

            for (i = 0; i < 9; i++)
            {
                for (j = 0; j < 9; j++)
                {
                    _cells[i][j] = new Cell(new TwoTupleOfIntegers(0, 0), 0);
                }
            }

            _cellsRemainToSet = new ArrayList();
            _dataSetBeforeSimulationHasBeenDone = new ArrayList();
            _simulationHasBeenDone = false;

            //DebugPrintSquareIndex();
        }

        private void DebugPrintSquareIndex()
        {
            StringBuilder sb = new StringBuilder();

            for (int i = 1; i <= 9; i++)
            {
                sb.Append(string.Format("squareIndex[{0}]: {1}\r\n", i.ToString(), ReturnArrayOfTwoTuplesAsString(_squareIndex[i - 1])));
            }

            Sudoku.Utility.CreateNewFile("C:\\tmp\\Sudoku\\Sudoku2SquareIndex_" + Guid.NewGuid().ToString() + ".txt", sb.ToString());
        }

        private string ReturnArrayOfTwoTuplesAsString(TwoTupleOfIntegers[] v)
        {
            StringBuilder sb = new StringBuilder("{");

            for (int i = 0; i < v.Length; i++)
            {
                if (i == 0)
                {
                    sb.Append(v[i]).ToString();
                }
                else
                {
                    sb.Append(", " + v[i]).ToString();
                }
            }

            sb.Append("}");

            return sb.ToString();
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

        private void UpdateStructure(int rowIndex, int columnIndex, int squareIndex, int itemValue)
        {
            int i, j, rowIdx, colIdx, sqIdx;

            for(i = 0; i < 9; i++)
            {
                if (_cells[rowIndex][i].itemValue == 0)
                {
                    for(j = 0; j < _cells[rowIndex][i].possibleItemValuesToSet.Count; j++)
                    {
                        if (_cells[rowIndex][i].possibleItemValuesToSet.IndexOf(itemValue) >= 0)
                        {
                            _cells[rowIndex][i].possibleItemValuesToSet.Remove(itemValue);

                            sqIdx = (3 * (rowIndex / 3)) + (i / 3);

                            if (_numberOfItemsPossibleToSetRow[rowIndex][itemValue - 1] == 0)
                            {
                                throw new Exception("(_numberOfItemsPossibleToSetRow[rowIndex][itemValue - 1] == 0)");
                            }

                            if (_numberOfItemsPossibleToSetColumn[i][itemValue - 1] == 0)
                            {
                                throw new Exception("(_numberOfItemsPossibleToSetColumn[i][itemValue - 1] == 0)");
                            }

                            if (_numberOfItemsPossibleToSetSquare[sqIdx][itemValue - 1] == 0)
                            {
                                throw new Exception("(_numberOfItemsPossibleToSetSquare[sqIdx][itemValue - 1] == 0)");
                            }

                            _numberOfItemsPossibleToSetRow[rowIndex][itemValue - 1]--;
                            _numberOfItemsPossibleToSetColumn[i][itemValue - 1]--;
                            _numberOfItemsPossibleToSetSquare[sqIdx][itemValue - 1]--;
                            _totalNumberOfItemsPossibleToSet -= 3;
                        }
                    }
                }
            }

            for (i = 0; i < 9; i++)
            {
                if ((i != rowIndex) && (_cells[i][columnIndex].itemValue == 0))
                {
                    for (j = 0; j < _cells[i][columnIndex].possibleItemValuesToSet.Count; j++)
                    {
                        if (_cells[i][columnIndex].possibleItemValuesToSet.IndexOf(itemValue) >= 0)
                        {
                            _cells[i][columnIndex].possibleItemValuesToSet.Remove(itemValue);

                            sqIdx = (3 * (i / 3)) + (columnIndex / 3);

                            if (_numberOfItemsPossibleToSetRow[i][itemValue - 1] == 0)
                            {
                                throw new Exception("(_numberOfItemsPossibleToSetRow[i][itemValue - 1] == 0)");
                            }

                            if (_numberOfItemsPossibleToSetColumn[columnIndex][itemValue - 1] == 0)
                            {
                                throw new Exception("(_numberOfItemsPossibleToSetColumn[columnIndex][itemValue - 1] == 0)");
                            }

                            if (_numberOfItemsPossibleToSetSquare[sqIdx][itemValue - 1] == 0)
                            {
                                throw new Exception("(_numberOfItemsPossibleToSetSquare[sqIdx][itemValue - 1] == 0)");
                            }

                            _numberOfItemsPossibleToSetRow[i][itemValue - 1]--;
                            _numberOfItemsPossibleToSetColumn[columnIndex][itemValue - 1]--;
                            _numberOfItemsPossibleToSetSquare[sqIdx][itemValue - 1]--;
                            _totalNumberOfItemsPossibleToSet -= 3;
                        }
                    }
                }
            }

            for (i = 0; i < 9; i++)
            {
                rowIdx = _squareIndex[squareIndex][i].x;
                colIdx = _squareIndex[squareIndex][i].y;

                if ((rowIdx != rowIndex) && (colIdx != columnIndex) && (_cells[rowIdx][colIdx].itemValue == 0))
                {
                    for (j = 0; j < _cells[rowIdx][colIdx].possibleItemValuesToSet.Count; j++)
                    {
                        if (_cells[rowIdx][colIdx].possibleItemValuesToSet.IndexOf(itemValue) >= 0)
                        {
                            _cells[rowIdx][colIdx].possibleItemValuesToSet.Remove(itemValue);

                            sqIdx = (3 * (rowIdx / 3)) + (colIdx / 3);

                            if (_numberOfItemsPossibleToSetRow[rowIdx][itemValue - 1] == 0)
                            {
                                throw new Exception("(_numberOfItemsPossibleToSetRow[rowIdx][itemValue - 1] == 0)");
                            }

                            if (_numberOfItemsPossibleToSetColumn[colIdx][itemValue - 1] == 0)
                            {
                                throw new Exception("(_numberOfItemsPossibleToSetColumn[colIdx][itemValue - 1] == 0)");
                            }

                            if (_numberOfItemsPossibleToSetSquare[sqIdx][itemValue - 1] == 0)
                            {
                                throw new Exception("(_numberOfItemsPossibleToSetSquare[sqIdx][itemValue - 1] == 0)");
                            }

                            _numberOfItemsPossibleToSetRow[rowIdx][itemValue - 1]--;
                            _numberOfItemsPossibleToSetColumn[colIdx][itemValue - 1]--;
                            _numberOfItemsPossibleToSetSquare[sqIdx][itemValue - 1]--;
                            _totalNumberOfItemsPossibleToSet -= 3;
                        }
                    }
                }
            }
        }

        private void Init()
        {
            int i, j, rowIndex, columnIndex, squareIndex, itemValue;

            for (rowIndex = 0; rowIndex < 9; rowIndex++)
            {
                for (columnIndex = 0; columnIndex < 9; columnIndex++)
                {
                    squareIndex = (3 * (rowIndex / 3)) + (columnIndex / 3);

                    _cells[rowIndex][columnIndex].itemValue = 0;
                    _cells[rowIndex][columnIndex].possibleItemValuesToSet.Clear();

                    for (itemValue = 1; itemValue <= 9; itemValue++)
                    {
                        _cells[rowIndex][columnIndex].possibleItemValuesToSet.Add(itemValue);
                    }
                }
            }

            for (i = 0; i < 9; i++)
            {
                for (j = 0; j < 9; j++)
                {
                    _numberOfItemsPossibleToSetRow[i][j] = 9;
                    _numberOfItemsPossibleToSetColumn[i][j] = 9;
                    _numberOfItemsPossibleToSetSquare[i][j] = 9;
                }
            }

            _totalNumberOfItemsPossibleToSet = (81 * 9);
        }

        private string SetCell(int rowIndex, int columnIndex, int itemValue)
        {
            int squareIndex = (3 * (rowIndex / 3)) + (columnIndex / 3);

            if (RowHasItemValueInAnyCell(rowIndex, itemValue))
            {
                return string.Format("Number {0} appears more than once in row {1}!", itemValue, rowIndex + 1);
            }

            if (ColumnHasItemValueInAnyCell(columnIndex, itemValue))
            {
                return string.Format("Number {0} appears more than once in column {1}!", itemValue, columnIndex + 1);
            }

            if (SquareHasItemValueInAnyCell(squareIndex, itemValue))
            {
                return string.Format("Number {0} appears more than once in square {1}!", itemValue, squareIndex + 1);
            }

            _cells[rowIndex][columnIndex].itemValue = itemValue;

            UpdateStructure(rowIndex, columnIndex, squareIndex, itemValue);

            return null;
        }

        private string SetData(ArrayList data)
        {
            int i;
            string returnValue = null;

            i = 0;
            while ((i < data.Count) && (returnValue == null))
            {
                returnValue = SetCell(((Cell)data[i]).position.x, ((Cell)data[i]).position.y, ((Cell)data[i]).itemValue);
                i++;
            }

            //DebugPrintStructure();

            return returnValue;
        }

        private void FillCellsRemainToSet()
        {
            _cellsRemainToSet.Clear();

            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    if (_cells[i][j].itemValue == 0)
                    {
                        _cellsRemainToSet.Add(new TwoTupleOfIntegers(i, j));
                    }
                }
            }
        }

        private ArrayList ReturnData(ArrayList originalData)
        {
            ArrayList data = new ArrayList();
            Sudoku.ThreeTupleOfIntegers threeTupleOfIntegers;
            TwoTupleOfIntegers position;
            int itemValue;

            for (int i = 0; i < originalData.Count; i++)
            {
                threeTupleOfIntegers = (Sudoku.ThreeTupleOfIntegers)originalData[i];
                position = new TwoTupleOfIntegers(threeTupleOfIntegers.rowIndex, threeTupleOfIntegers.columnIndex);
                itemValue = threeTupleOfIntegers.item;
                data.Add(new Cell(position, itemValue));
            }

            return data;
        }

        private string ReturnSudokuString()
        {
            StringBuilder sb = new StringBuilder();
            string str;

            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    str = (j <= 7) ? " " : "\r\n";
                    sb.Append(string.Format("{0}{1}", _cells[i][j].itemValue.ToString(), str));
                }
            }

            return sb.ToString().TrimEnd();
        }

        private void DebugPrintStructure()
        {
            int i, j;
            StringBuilder sb = new StringBuilder();

            for (i = 1; i <= 9; i++)
            {
                for (j = 1; j <= 9; j++)
                {
                    sb.Append(string.Format("[{0}, {1}]: {2} {3}\r\n", i.ToString(), j.ToString(), _cells[i - 1][j - 1].itemValue.ToString(), Sudoku.Utility.ReturnString(_cells[i - 1][j - 1].possibleItemValuesToSet)));
                }

                sb.Append("\r\n");
            }

            for (i = 1; i <= 9; i++)
            {
                sb.Append(string.Format("Possible to set row[{0}]: {1}\r\n", i.ToString(), Sudoku.Utility.ReturnString(_possibleToSetRow[i - 1])));
            }

            sb.Append("\r\n");

            for (i = 1; i <= 9; i++)
            {
                sb.Append(string.Format("Possible to set column[{0}]: {1}\r\n", i.ToString(), Sudoku.Utility.ReturnString(_possibleToSetColumn[i - 1])));
            }

            sb.Append("\r\n");

            for (i = 1; i <= 9; i++)
            {
                sb.Append(string.Format("Possible to set square[{0}]: {1}\r\n", i.ToString(), Sudoku.Utility.ReturnString(_possibleToSetSquare[i - 1])));
            }

            Sudoku.Utility.CreateNewFile("C:\\tmp\\Sudoku\\Sudoku2Structure_" + Guid.NewGuid().ToString() + ".txt", sb.ToString());
        }

        private int LoopThroughListWithCellsThatRemainToSet()
        {
            int i, j, itemsSet, itemValue = 0, rowIndex, columnIndex, squareIndex;
            bool findItemToSet;

            i = 0;
            itemsSet = 0;

            while (i < _cellsRemainToSet.Count)
            {
                findItemToSet = false;
                rowIndex = ((TwoTupleOfIntegers)_cellsRemainToSet[i]).x;
                columnIndex = ((TwoTupleOfIntegers)_cellsRemainToSet[i]).y;
                squareIndex = (3 * (rowIndex / 3)) + (columnIndex / 3);

                if (_cells[rowIndex][columnIndex].possibleItemValuesToSet.Count == 1)
                {
                    findItemToSet = true;
                    itemValue = (int)_cells[rowIndex][columnIndex].possibleItemValuesToSet[0];
                }
                else
                {
                    j = 0;
                    while ((j < _cells[rowIndex][columnIndex].possibleItemValuesToSet.Count) && (!findItemToSet))
                    {
                        itemValue = (int)_cells[rowIndex][columnIndex].possibleItemValuesToSet[j];

                        if ((_numberOfItemsPossibleToSetRow[rowIndex][itemValue - 1] == 1) || (_numberOfItemsPossibleToSetColumn[columnIndex][itemValue - 1] == 1) || (_numberOfItemsPossibleToSetSquare[squareIndex][itemValue - 1] == 1))
                        {
                            findItemToSet = true;
                        }
                        else
                        {
                            j++;
                        }
                    }
                }

                if (findItemToSet)
                {
                    SetCell(rowIndex, columnIndex, itemValue);

                    if (!_simulationHasBeenDone)
                    {
                        _dataSetBeforeSimulationHasBeenDone.Add(new Cell(new TwoTupleOfIntegers(rowIndex, columnIndex), itemValue));
                    }

                    itemsSet++;

                    _cellsRemainToSet.RemoveAt(i);
                }
                else
                {
                    i++;
                }
            }

            return itemsSet;
        }

        public Result Process(ArrayList originalData, int maxNumberOfTries)
        {
            ArrayList data, dataSetBeforeSimulationHasBeenDone, cellsRemainToSet;
            int i, j, n, randomNumber, rowIndex, columnIndex, squareIndex, minNumberPossibleToSetItems, itemsSet, numberOfTries = 0, maxNumberOfItemsSetInATry = 0;
            int[] numberPossibleToSetItems = new int[8];
            bool atLeastOneItemSet, simulationHasBeenDone, findItemToSet;
            string errorMessage, sudokuString = null;
            Cell cell = new Cell(new TwoTupleOfIntegers(0, 0), 0);
            Random random = new Random((int)(DateTime.Now.Ticks % (long)int.MaxValue));

            itemsSet = 0;
            simulationHasBeenDone = false;
            dataSetBeforeSimulationHasBeenDone = new ArrayList();

            data = ReturnData(originalData); //data is an ArrayList with cells

            Init();
            errorMessage = SetData(data);

            if (errorMessage != null)
                return new Result(errorMessage, false, false, true, false, null);

            if (data.Count == 81)
                return new Result(null, true, false, true, true, null);

            while (((data.Count + itemsSet) < 81) && (numberOfTries < maxNumberOfTries))
            {
                numberOfTries++;

                if (numberOfTries > 1)
                {
                    Init();
                    SetData(data);
                    itemsSet = 0;

                    for (i = 0; i < dataSetBeforeSimulationHasBeenDone.Count; i++)
                    {
                        SetCell((Cell)dataSetBeforeSimulationHasBeenDone[i]);
                    }
                }

                atLeastOneItemSet = true;

                FillCellsRemainToSet();

                while (((data.Count + itemsSet) < 81) && atLeastOneItemSet)
                {
                    atLeastOneItemSet = false;
                    cellsRemainToSet = ReturnCellsRemainToSet();



                    if (!atLeastOneItemSet)
                    {
                        for (i = 0; i < 8; i++)
                        {
                            numberPossibleToSetItems[i] = 0;
                        }

                        minNumberPossibleToSetItems = int.MaxValue;

                        for (i = 0; i < cellsRemainToSet.Count; i++)
                        {
                            rowIndex = ((TwoTupleOfIntegers)cellsRemainToSet[i]).x;
                            columnIndex = ((TwoTupleOfIntegers)cellsRemainToSet[i]).y;

                            if (_cells[rowIndex][columnIndex].possibleItemValuesToSet.Count == 1)
                            {
                                throw new Exception("(_cells[rowIndex][columnIndex].possibleItemValuesToSet.Count == 1)");
                            }
                            else if (_cells[rowIndex][columnIndex].possibleItemValuesToSet.Count > 1)
                            {
                                if (minNumberPossibleToSetItems > _cells[rowIndex][columnIndex].possibleItemValuesToSet.Count)
                                {
                                    minNumberPossibleToSetItems = _cells[rowIndex][columnIndex].possibleItemValuesToSet.Count;
                                }

                                numberPossibleToSetItems[_cells[rowIndex][columnIndex].possibleItemValuesToSet.Count - 2]++;
                            }
                        }

                        if (minNumberPossibleToSetItems < int.MaxValue)
                        {
                            n = 0;
                            i = 0;
                            randomNumber = 1 + random.Next(numberPossibleToSetItems[minNumberPossibleToSetItems - 2]);

                            while (n < randomNumber)
                            {
                                rowIndex = ((TwoTupleOfIntegers)cellsRemainToSet[i]).x;
                                columnIndex = ((TwoTupleOfIntegers)cellsRemainToSet[i]).y;
                                cell.position.x = rowIndex;
                                cell.position.y = columnIndex;

                                if (_cells[rowIndex][columnIndex].possibleItemValuesToSet.Count == minNumberPossibleToSetItems)
                                {
                                    n++;
                                }

                                if (n == randomNumber)
                                {
                                    cell.itemValue = (int)_cells[rowIndex][columnIndex].possibleItemValuesToSet[random.Next(minNumberPossibleToSetItems)];
                                    SetCell(cell);
                                    atLeastOneItemSet = true;
                                    simulationHasBeenDone = true;
                                    itemsSet++;
                                }

                                i++;
                            }
                        }
                    }
                }

                if (itemsSet > maxNumberOfItemsSetInATry)
                {
                    maxNumberOfItemsSetInATry = itemsSet;
                    sudokuString = ReturnSudokuString();
                }

                if ((numberOfTries == 1) && (itemsSet == 0))
                {
                    return new Result(null, false, false, true, false, null);
                }
            }

            if ((data.Count + itemsSet) < 81)
            {
                return new Result(null, false, true, false, false, sudokuString);
            }
            else
            {
                return new Result(null, false, false, false, true, sudokuString);
            }
        }
    }
}
