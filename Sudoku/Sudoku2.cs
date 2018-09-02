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
        private ArrayList[] _possibleToSetRow;
        private ArrayList[] _possibleToSetColumn;
        private ArrayList[] _possibleToSetSquare;
        private ArrayList _cellsRemainToSet;
        private TwoTupleOfIntegers[][] _squareIndex;

        public SudokuBoard()
        {
            int i, j, r, c;

            _possibleToSetRow = new ArrayList[9];
            _possibleToSetColumn = new ArrayList[9];
            _possibleToSetSquare = new ArrayList[9];
            _squareIndex = new TwoTupleOfIntegers[9][];

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
                        _squareIndex[j] = new TwoTupleOfIntegers[9];
                        r = j / 3;
                        c = j % 3;
                        _squareIndex[j] = new TwoTupleOfIntegers[] { new TwoTupleOfIntegers(3 * r, 3 * c), new TwoTupleOfIntegers(3 * r, 3 * c + 1), new TwoTupleOfIntegers(3 * r, 3 * c + 2), new TwoTupleOfIntegers(3 * r + 1, 3 * c), new TwoTupleOfIntegers(3 * r + 1, 3 * c + 1), new TwoTupleOfIntegers(3 * r + 1, 3 * c + 2), new TwoTupleOfIntegers(3 * r + 2, 3 * c), new TwoTupleOfIntegers(3 * r + 2, 3 * c + 1), new TwoTupleOfIntegers(3 * r + 2, 3 * c + 2) };
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

            DebugPrintSquareIndex();
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

        private bool CanSetItemValueInCell(Cell cell)
        {
            int rowIndex = cell.position.x;
            int columnIndex = cell.position.y;
            int squareIndex = (3 * (rowIndex / 3)) + (columnIndex / 3);
            int itemValue = cell.itemValue;

            return !RowHasItemValueInAnyCell(rowIndex, itemValue) && !ColumnHasItemValueInAnyCell(columnIndex, itemValue) && !SquareHasItemValueInAnyCell(squareIndex, itemValue);
        }

        private ArrayList ReturnListWithCellsToUpdate(int rowIndex, int columnIndex, int squareIndex)
        {
            ArrayList tmpArrayList, cellsToUpdate;
            int i, r, c;
            string str;

            tmpArrayList = new ArrayList();
            cellsToUpdate = new ArrayList();

            for (i = 0; i <= 8; i++)
            {
                str = string.Format("[{0}][{1}]", i.ToString(), columnIndex.ToString());
                tmpArrayList.Add(str);

                if (_cells[i][columnIndex].itemValue == 0)
                {
                    cellsToUpdate.Add(new TwoTupleOfIntegers(i, columnIndex));
                }
            }

            for (i = 0; i <= 8; i++)
            {
                str = string.Format("[{0}][{1}]", rowIndex.ToString(), i.ToString());
                tmpArrayList.Add(str);

                if (_cells[rowIndex][i].itemValue == 0)
                {
                    cellsToUpdate.Add(new TwoTupleOfIntegers(rowIndex, i));
                }
            }

            for (i = 0; i <= 8; i++)
            {
                r = _squareIndex[squareIndex][i].x;
                c = _squareIndex[squareIndex][i].y;

                str = string.Format("[{0}][{1}]", r.ToString(), c.ToString());

                if ((_cells[r][c].itemValue == 0) && (tmpArrayList.IndexOf(str) == -1))
                {
                    cellsToUpdate.Add(new TwoTupleOfIntegers(r, c));
                }
            }

            //-----------------Debug -------------------
            StringBuilder sb = new StringBuilder(string.Format("[rowIndex, columnIndex, squareIndex] = [{0}, {1}, {2}]\r\n", rowIndex.ToString(), columnIndex.ToString(), squareIndex.ToString()));
            TwoTupleOfIntegers[] arrayWithTwoTupleOfIntegers = new TwoTupleOfIntegers[cellsToUpdate.Count];
            for (i = 0; i < cellsToUpdate.Count; i++)
            {
                arrayWithTwoTupleOfIntegers[i] = (TwoTupleOfIntegers)cellsToUpdate[i];
            }
            sb.Append(ReturnArrayOfTwoTuplesAsString(arrayWithTwoTupleOfIntegers));
            Sudoku.Utility.CreateNewFile("C:\\tmp\\Sudoku\\Sudoku2CellsToUpdate_" + Guid.NewGuid().ToString() + ".txt", sb.ToString());
            //---------------------------------------------------------------------------

            return cellsToUpdate;
        }

        private void UpdateStructure(int rowIndex, int columnIndex, int squareIndex, int itemValue)
        {
            int i;
            ArrayList cellsToUpdate;

            //---------------- Debug -----------------------------
            for (i = 0; i < _cells[rowIndex][columnIndex].possibleItemValuesToSet.Count; i++)
            {
                if (_possibleToSetRow[rowIndex].IndexOf(_cells[rowIndex][columnIndex].possibleItemValuesToSet[i]) == -1)
                {
                    throw new Exception("(_possibleToSetRow[rowIndex].IndexOf(_cells[rowIndex][columnIndex].possibleItemValuesToSet[i]) == -1)");
                }

                if (_possibleToSetColumn[columnIndex].IndexOf(_cells[rowIndex][columnIndex].possibleItemValuesToSet[i]) == -1)
                {
                    throw new Exception("(_possibleToSetColumn[columnIndex].IndexOf(_cells[rowIndex][columnIndex].possibleItemValuesToSet[i]) == -1)");
                }


                if (_possibleToSetSquare[squareIndex].IndexOf(_cells[rowIndex][columnIndex].possibleItemValuesToSet[i]) == -1)
                {
                    throw new Exception("(_possibleToSetSquare[squareIndex].IndexOf(_cells[rowIndex][columnIndex].possibleItemValuesToSet[i]) == -1)");
                }
            }
            //-------------------------------------------------

            for (i = 0; i < _cells[rowIndex][columnIndex].possibleItemValuesToSet.Count; i++)
            {
                _possibleToSetRow[rowIndex].Remove(_cells[rowIndex][columnIndex].possibleItemValuesToSet[i]);
                _possibleToSetColumn[columnIndex].Remove(_cells[rowIndex][columnIndex].possibleItemValuesToSet[i]);
                _possibleToSetSquare[squareIndex].Remove(_cells[rowIndex][columnIndex].possibleItemValuesToSet[i]);
            }

            cellsToUpdate = ReturnListWithCellsToUpdate(rowIndex, columnIndex, squareIndex);

            for (i = 0; i < cellsToUpdate.Count; i++)
            {
                _cells[((TwoTupleOfIntegers)cellsToUpdate[i]).x][((TwoTupleOfIntegers)cellsToUpdate[i]).y].possibleItemValuesToSet.Remove(itemValue);
            }

            _cells[rowIndex][columnIndex].possibleItemValuesToSet.Clear();
        }

        private string SetCell(Cell cell)
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
            int i, j;
            string returnValue = null;

            for (i = 0; i < 9; i++)
            {
                for (j = 0; j < 9; j++)
                {
                    _cells[i][j].itemValue = 0;
                }
            }

            ResetStructure();

            i = 0;
            while ((i < data.Count) && (returnValue == null))
            {
                returnValue = SetCell((Cell)data[i]);
                i++;
            }

            DebugPrintStructure();

            return returnValue;
        }

        private ArrayList ReturnCellsRemainToSet()
        {
            ArrayList cellsRemainToSet = new ArrayList();

            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    if (_cells[i][j].itemValue == 0)
                    {
                        cellsRemainToSet.Add(new TwoTupleOfIntegers(i, j));
                    }
                }
            }

            return cellsRemainToSet;
        }

        private void ResetStructure()
        {
            int i, rowIndex, columnIndex, squareIndex, itemValue;

            for (i = 0; i < 9; i++)
            {
                _possibleToSetRow[i].Clear();
                _possibleToSetColumn[i].Clear();
                _possibleToSetSquare[i].Clear();
            }

            for (rowIndex = 0; rowIndex < 9; rowIndex++)
            {
                for (columnIndex = 0; columnIndex < 9; columnIndex++)
                {
                    _cells[rowIndex][columnIndex].possibleItemValuesToSet.Clear();
                    squareIndex = (3 * (rowIndex / 3)) + (columnIndex / 3);

                    for (itemValue = 1; itemValue <= 9; itemValue++)
                    {
                        _cells[rowIndex][columnIndex].possibleItemValuesToSet.Add(itemValue);
                        _possibleToSetRow[rowIndex].Add(itemValue);
                        _possibleToSetColumn[columnIndex].Add(itemValue);
                        _possibleToSetSquare[squareIndex].Add(itemValue);
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

        private int ReturnNumberOfOccurenciesOfItemValue(ArrayList v, int itemValue)
        {
            int i, numberOfOccurencies = 0;

            for (i = 0; i < v.Count; i++)
            {
                if ((int)v[i] == itemValue)
                {
                    numberOfOccurencies++;
                }
            }

            if (numberOfOccurencies == 0)
            {
                throw new Exception("(numberOfOccurencies == 0) in ReturnNumberOfOccurenciesOfItemValue!");
            }

            return numberOfOccurencies;
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
                    SetData(data);
                    itemsSet = 0;

                    for (i = 0; i < dataSetBeforeSimulationHasBeenDone.Count; i++)
                    {
                        SetCell((Cell)dataSetBeforeSimulationHasBeenDone[i]);
                    }
                }

                atLeastOneItemSet = true;

                while (((data.Count + itemsSet) < 81) && atLeastOneItemSet)
                {
                    atLeastOneItemSet = false;
                    cellsRemainToSet = ReturnCellsRemainToSet();

                    for (i = 0; i < cellsRemainToSet.Count; i++)
                    {
                        rowIndex = ((TwoTupleOfIntegers)cellsRemainToSet[i]).x;
                        columnIndex = ((TwoTupleOfIntegers)cellsRemainToSet[i]).y;
                        squareIndex = (3 * (rowIndex / 3)) + (columnIndex / 3);

                        if ((rowIndex == 5) && (columnIndex == 2))
                        {
                            int yyy = 0;
                        }

                        cell.position.x = rowIndex;
                        cell.position.y = columnIndex;

                        if (_cells[rowIndex][columnIndex].possibleItemValuesToSet.Count == 1)
                        {
                            cell.itemValue = (int)_cells[rowIndex][columnIndex].possibleItemValuesToSet[0];
                            SetCell(cell);
                            atLeastOneItemSet = true;
                            itemsSet++;

                            if (!simulationHasBeenDone)
                            {
                                dataSetBeforeSimulationHasBeenDone.Add(new Cell(new TwoTupleOfIntegers(rowIndex, columnIndex), cell.itemValue));
                            }
                        }
                        else
                        {
                            findItemToSet = false;
                            j = 0;
                            while ((j < _cells[rowIndex][columnIndex].possibleItemValuesToSet.Count) && (!findItemToSet))
                            {
                                if (ReturnNumberOfOccurenciesOfItemValue(_possibleToSetRow[rowIndex], (int)_cells[rowIndex][columnIndex].possibleItemValuesToSet[j]) == 1)
                                {
                                    cell.itemValue = (int)_cells[rowIndex][columnIndex].possibleItemValuesToSet[j];
                                    SetCell(cell);
                                    atLeastOneItemSet = true;
                                    findItemToSet = true;
                                    itemsSet++;

                                    if (!simulationHasBeenDone)
                                    {
                                        dataSetBeforeSimulationHasBeenDone.Add(new Cell(new TwoTupleOfIntegers(rowIndex, columnIndex), cell.itemValue));
                                    }
                                }
                                else if (ReturnNumberOfOccurenciesOfItemValue(_possibleToSetColumn[columnIndex], (int)_cells[rowIndex][columnIndex].possibleItemValuesToSet[j]) == 1)
                                {
                                    cell.itemValue = (int)_cells[rowIndex][columnIndex].possibleItemValuesToSet[j];
                                    SetCell(cell);
                                    atLeastOneItemSet = true;
                                    findItemToSet = true;
                                    itemsSet++;

                                    if (!simulationHasBeenDone)
                                    {
                                        dataSetBeforeSimulationHasBeenDone.Add(new Cell(new TwoTupleOfIntegers(rowIndex, columnIndex), cell.itemValue));
                                    }
                                }
                                else if (ReturnNumberOfOccurenciesOfItemValue(_possibleToSetSquare[squareIndex], (int)_cells[rowIndex][columnIndex].possibleItemValuesToSet[j]) == 1)
                                {
                                    cell.itemValue = (int)_cells[rowIndex][columnIndex].possibleItemValuesToSet[j];
                                    SetCell(cell);
                                    atLeastOneItemSet = true;
                                    findItemToSet = true;
                                    itemsSet++;

                                    if (!simulationHasBeenDone)
                                    {
                                        dataSetBeforeSimulationHasBeenDone.Add(new Cell(new TwoTupleOfIntegers(rowIndex, columnIndex), cell.itemValue));
                                    }
                                }
                                else
                                {
                                    j++;
                                }
                            }
                        }
                    }

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
