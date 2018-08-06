using System;
using System.IO;
using System.Text;
using System.Configuration;
using System.Windows.Forms;
using System.Collections;

namespace Sudoku
{
    public partial class Form1 : Form
    {
        private string dir = "C:\\git_cjonasl\\Sudoku";

        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string debugString;
            string fileContents, errorMessage;
            int i, j, iteration, numberOfNewNumbersPossibleToSetInSudokuBoard = 1;
            bool sudokuSolved = false;

            int[,] v;
            SudokuBoard sudokuBoard;
            SudokuIterator sudokuIterator;

            fileContents = Utility.ReturnFileContents(dir + "\\SudokuBoardInput.txt");

            if (!Utility.DataIsCorrect(fileContents, out errorMessage))
            {
                MessageBox.Show("Data in file is incorrect! Error message:\r\n" + errorMessage, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                v = Utility.ReturnSudokuData(Utility.ReturnFileContents(dir + "\\SudokuBoardInput.txt"));

                sudokuBoard = new SudokuBoard();

                try
                {
                    for (i = 0; i < 9; i++)
                    {
                        for (j = 0; j < 9; j++)
                        {
                            if (v[i, j] != 0)
                            {
                                sudokuBoard.SetNumber(i, j, v[i, j]);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("An error happened when setting data to sudoku board! Error message:\r\n" + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

                //string str = sudokuBoard.ReturnSudoku();
                //Utility.CreateNewFile("C:\\git_cjonasl\\Sudoku\\sudokuBoard.ReturnSudoku.txt", str);

                Utility.InitDirectory(dir + "\\debug");

                if (File.Exists(dir + "\\Sudoku result.txt"))
                {
                    File.Delete(dir + "\\Sudoku result.txt");
                }

                if (File.Exists(dir + "\\Error occured.txt"))
                {
                    File.Delete(dir + "\\Error occured.txt");
                }

                sudokuIterator = new SudokuIterator();

                iteration = 1;

                while ((numberOfNewNumbersPossibleToSetInSudokuBoard != 0) && (!sudokuSolved))
                {
                    v = sudokuIterator.ReturnNewNumbersPossibleToSetInSudokuBoard(sudokuBoard, out numberOfNewNumbersPossibleToSetInSudokuBoard, out debugString);

                    if (numberOfNewNumbersPossibleToSetInSudokuBoard != 0)
                    {
                        try
                        {
                            for (i = 0; i < 9; i++)
                            {
                                for (j = 0; j < 9; j++)
                                {
                                    if (v[i, j] != 0)
                                    {
                                        sudokuBoard.SetNumber(i, j, v[i, j]);
                                    }
                                }
                            }

                            Utility.CreateNewFile(dir + "\\debug\\Iteration" + iteration.ToString() + ".txt", debugString);
                            sudokuSolved = sudokuBoard.SudokuIsSolved;
                            iteration++;
                        }
                        catch(Exception ex)
                        {
                            Utility.CreateNewFile(dir + "\\Error occured.txt", string.Format("An error occured when calling sudokuBoard.SetNumber in iterration {0}. Error message: {1}", iteration.ToString(), ex.Message));
                        }
                    }
                    else
                    {
                        Utility.CreateNewFile(dir + "\\Error occured.txt", string.Format("An error occured when calling sudokuBoard.SetNumber in iterration {0}.", iteration.ToString()));
                    }
                }

                if (sudokuSolved)
                {
                    Utility.CreateNewFile(dir + "\\Sudoku result.txt", sudokuBoard.SudokuBoardString);
                }
            }
        }
    }

    public static class Utility
    {
        public static void CreateNewFile(string fileNameFullPath, string fileContent)
        {
            FileStream fileStream = new FileStream(fileNameFullPath, FileMode.Create, FileAccess.Write);
            StreamWriter streamWriter = new StreamWriter(fileStream, Encoding.UTF8);
            streamWriter.Write(fileContent);
            streamWriter.Flush();
            fileStream.Flush();
            streamWriter.Close();
            fileStream.Close();
        }

        public static string ReturnFileContents(string fileNameFullPath)
        {
            FileStream fileStream = new FileStream(fileNameFullPath, FileMode.Open, FileAccess.Read);
            StreamReader streamReader = new StreamReader(fileStream, Encoding.UTF8);
            string str = streamReader.ReadToEnd();
            streamReader.Close();
            fileStream.Close();

            return str;
        }

        public static bool DataIsCorrect(string sudoku, out string errorMessage)
        {
            string[] u, v;
            bool tmp;
            int i, j, n;

            errorMessage = null;

            u = sudoku.Split(new string[] { "\r\n" }, StringSplitOptions.None);

            if (u.Length != 9)
            {
                errorMessage = "Not exactly 9 rows";
                return false;
            }

            i = 1;
            tmp = true;

            while (tmp && (i <= 9))
            {
                v = u[i - 1].Split(' ');

                if (v.Length != 9)
                {
                    errorMessage = "Row " + i.ToString() + " does not contain exactly 9 columns one blank separated";
                    tmp = false;
                }
                else
                {
                    j = 1;

                    while (tmp && (j <= 9))
                    {
                        if (!int.TryParse(v[j - 1], out n))
                        {
                            errorMessage = "The value \"" + v[j - 1] + "\" in row " + i.ToString() + " and column " + j.ToString() + " is not a valid integer";
                            tmp = false;
                        }
                        else
                        {
                            if ((n < 0) || (n > 9))
                            {
                                errorMessage = "The integer " + v[j - 1] + " in row " + i.ToString() + " and column " + j.ToString() + " is not in the range 0-9";
                                tmp = false;
                            }
                        }

                        j++;
                    }
                }

                i++;
            }

            if (!tmp)
            {
                return false;
            }

            return true;
        }

        public static int[,] ReturnSudokuData(string sudoku)
        {
            int[,] sudokuData = new int[9, 9];
            string[] u, v;
            int i, j;

            u = sudoku.Split(new string[] { "\r\n" }, StringSplitOptions.None);

            for (i = 0; i < 9; i++)
            {
                v = u[i].Split(' ');

                for (j = 0; j < 9; j++)
                {
                    sudokuData[i, j] = int.Parse(v[j]);
                }
            }

            return sudokuData;
        }

        public static string ReturnString(int[] v, int n)
        {
            StringBuilder sb = new StringBuilder("{");

            for (int i = 0; i < n; i++)
            {
                if (i == 0)
                {
                    sb.Append(v[0].ToString());
                }
                else if (i == (n - 1))
                {
                    sb.Append(", " + v[n - 1].ToString() + "}");
                }
                else
                {
                    sb.Append(", " + v[i].ToString());
                }
            }

            return sb.ToString();
        }

        public static string ReturnString(ArrayList v)
        {
            StringBuilder sb = new StringBuilder("{");

            for (int i = 0; i < v.Count; i++)
            {
                if (i == 0)
                {
                    sb.Append(v[0].ToString());
                }
                else if (i == (v.Count - 1))
                {
                    sb.Append(", " + v[v.Count - 1].ToString() + "}");
                }
                else
                {
                    sb.Append(", " + v[i].ToString());
                }
            }

            return sb.ToString();
        }

        public static void InitDirectory(string directoryNameFullPath)
        {
            if (!Directory.Exists(directoryNameFullPath))
            {
                Directory.CreateDirectory(directoryNameFullPath);
                return;
            }

            string[] v = Directory.GetFiles(directoryNameFullPath);

            for(int i = 0; i < v.Length; i++)
            {
                File.Delete(v[i]);
            }
        }
    }

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
                throw new Exception("Error in method SetNumber! There is alreday a number " + number.ToString() + " in row " + (rowIndex + 1).ToString());
            }

            if (ColumnHasNumberInAnyEntry(columnIndex, number))
            {
                throw new Exception("Error in method SetNumber! There is alreday a number " + number.ToString() + " in column " + (columnIndex + 1).ToString());
            }

            squareIndex = (3 * (rowIndex / 3)) + (columnIndex / 3);

            if (SquareHasNumberInAnyEntry(squareIndex, number))
            {
                throw new Exception("Error in method SetNumber! There is alreday a number " + number.ToString() + " in squre " + (squareIndex + 1).ToString());
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

    /// <summary>
    /// Item is one of 1, 2, 3, 4, 5, 6, 7, 8, 9
    /// </summary>
    public class SudokuPossibleToSetItem
    {
        public int[,,] rows, columns, squares;
        public int[,] numberOfPossibleItemsRows, numberOfPossibleItemsColumns, numberOfPossibleItemsSquares;
        public int totalNumberOfItemsPossibleToSet;

        public SudokuPossibleToSetItem()
        {
            rows = new int[9, 9, 9];
            columns = new int[9, 9, 9];
            squares = new int[9, 9, 9];
            numberOfPossibleItemsRows = new int[9, 9];
            numberOfPossibleItemsColumns = new int[9, 9];
            numberOfPossibleItemsSquares = new int[9, 9];
        }

        public void Process(SudokuBoard sudokuBoard)
        {
            int i, j, k, squareIndex, squareSequenceIndex;

            totalNumberOfItemsPossibleToSet = 0;

            for (i = 0; i < 9; i++)
            {
                for (j = 0; j < 9; j++)
                {
                    numberOfPossibleItemsRows[i, j] = -1;
                    numberOfPossibleItemsColumns[i, j] = -1;
                    numberOfPossibleItemsSquares[i, j] = -1;
                }
            }

            for (i = 0; i < 9; i++)
            {
                for (j = 0; j < 9; j++)
                {
                    squareIndex = (3 * (i / 3)) + (j / 3);
                    squareSequenceIndex = (3 * (i % 3)) + (j % 3);

                    if (!sudokuBoard.NumberIsSet(i, j))
                    {
                        numberOfPossibleItemsRows[i, j] = 0;
                        numberOfPossibleItemsColumns[j, i] = 0;
                        numberOfPossibleItemsSquares[squareIndex, squareSequenceIndex] = 0;

                        for (k = 1; k <= 9; k++)
                        {
                            if (sudokuBoard.CanSetNumber(i, j, k))
                            {
                                rows[i, j, numberOfPossibleItemsRows[i, j]] = k;
                                numberOfPossibleItemsRows[i, j]++;

                                columns[j, i, numberOfPossibleItemsColumns[j, i]] = k;
                                numberOfPossibleItemsColumns[j, i]++;

                                squares[squareIndex, squareSequenceIndex, numberOfPossibleItemsSquares[squareIndex, squareSequenceIndex]] = k;
                                numberOfPossibleItemsSquares[squareIndex, squareSequenceIndex]++;

                                totalNumberOfItemsPossibleToSet++;
                            }
                        }
                    }
                }
            }
        }
    }

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

                foreach(string file in files)
                {
                    File.Delete(file);
                }
            }
        }

        private bool IsItemAloneInRow(int rowIndex, int item)
        {
            int i, j, numberOfOccurenciesOfItem = 0;

            for(i = 0; i < 9; i++)
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
            for(int h = 0; h < n; h++)
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

                                debug[i, j] = rowColumn + "CAN SET ITEM " + tmpArrayList[0].ToString() + ". Items not causing conflict: " + Utility.ReturnString(tmpIntArray, sudokuPossibleToSetItem.numberOfPossibleItemsRows[i, j])  + sb.ToString();
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

        public class SudokuPossibleHolder
        {
            private int _row, _column, _numberOfPossibleItems;
            private int[] _possibleItems;

            public SudokuPossibleHolder(int row, int column, int numberOfPossibleItems, int[] possibleItems)
            {
                _row = row;
                _column = column;
                _numberOfPossibleItems = numberOfPossibleItems;

                _possibleItems = new int[numberOfPossibleItems];

                for(int i = 0; i < numberOfPossibleItems; i++)
                {
                    _possibleItems[i] = possibleItems[i];
                }
            }

            private string PossibleItemsString()
            {
                StringBuilder sb = new StringBuilder("{");

                for(int i = 0; i < _numberOfPossibleItems; i++)
                {
                    if (i > 0)
                    {
                        sb.Append(", " + _possibleItems[i].ToString());
                    }
                    else
                    {
                        sb.Append(_possibleItems[0].ToString());
                    }
                }

                sb.Append("}");

                return sb.ToString();
            }

            public override string ToString()
            {
                return string.Format("[{0}, {1}, {2}]", _row.ToString(), _column.ToString(), PossibleItemsString());
            }

            public int ReturnItem(Random r)
            {
                int n = r.Next(_numberOfPossibleItems);
                return _possibleItems[n];
            }
        }

        public class CollectionSudokuPossibleHolder
        {
            private int _numberOfPossibleItems;
            private ArrayList _sudokuPossibleHolders;

            public CollectionSudokuPossibleHolder(int numberOfPossibleItems)
            {
                _numberOfPossibleItems = numberOfPossibleItems;
                _sudokuPossibleHolders = new ArrayList();
            }

            public void Add(int row, int column, int[] possibleItems)
            {
                _sudokuPossibleHolders.Add(new SudokuPossibleHolder(row, column, _numberOfPossibleItems, possibleItems));
            }

            public override string ToString()
            {
                StringBuilder sb = new StringBuilder(string.Format("{0}:", _numberOfPossibleItems.ToString()));

                for(int i = 0; i < _sudokuPossibleHolders.Count; i++)
                {
                    sb.Append("  " + ((SudokuPossibleHolder)_sudokuPossibleHolders[i]).ToString());
                }

                return sb.ToString().TrimEnd();
            }

            public int ReturnItem(Random r)
            {
                int n = r.Next(_sudokuPossibleHolders.Count);
                return ((SudokuPossibleHolder)_sudokuPossibleHolders[n]).ReturnItem(r);
            }

            public void Reset()
            {
                _sudokuPossibleHolders.Clear();
            }
        }

        public class SudokuSimulateItem
        {
            private CollectionSudokuPossibleHolder[] _collectionSudokuPossibleHolder;
            private string _debugDirectory;
            private int _numberOfCallsToReturnItem;

            public SudokuSimulateItem()
            {
                _collectionSudokuPossibleHolder = new CollectionSudokuPossibleHolder[8];
                _debugDirectory = ConfigurationManager.AppSettings["DebugDirectory"] + "\\SudokuSimulateItem";
                _numberOfCallsToReturnItem = 0;

                for (int i = 0; i < 8; i++)
                {
                    _collectionSudokuPossibleHolder[i] = new CollectionSudokuPossibleHolder(2 + i);
                }

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

            private int[] ReturnIntArray(int[,,] u, int i, int j)
            {
                int[] v = new int[9];

                for(int k = 0; k < 9; k++)
                {
                    v[k] = u[i, j, k];
                }

                return v;
            }

            public int ReturnItem(Random r, SudokuPossibleToSetItem sudokuPossibleToSetItem)
            {
                int item, minNumberOfPossibleItemsToSet = 9;
                string fileNameFullPath;
                StringBuilder sb;

                _numberOfCallsToReturnItem++;

                for (int i = 0; i < 8; i++)
                {
                    _collectionSudokuPossibleHolder[i].Reset();
                }

                for (int i = 0; i < 9; i++)
                {
                    for (int j = 0; j < 9; j++)
                    {
                        if (sudokuPossibleToSetItem.numberOfPossibleItemsRows[i, j] != -1)
                        {
                            if (sudokuPossibleToSetItem.numberOfPossibleItemsRows[i, j] < minNumberOfPossibleItemsToSet)
                            {
                                minNumberOfPossibleItemsToSet = sudokuPossibleToSetItem.numberOfPossibleItemsRows[i, j];
                            }

                            _collectionSudokuPossibleHolder[sudokuPossibleToSetItem.numberOfPossibleItemsRows[i, j] - 2].Add(i + 1, j + 1, ReturnIntArray(sudokuPossibleToSetItem.rows, i, j));
                        }
                    }
                }

                item = _collectionSudokuPossibleHolder[minNumberOfPossibleItemsToSet - 2].ReturnItem(r);

                sb = new StringBuilder(string.Format("Simulate item {0}\r\n\r\n", item.ToString()));

                for (int i = 0; i < 8; i++)
                {
                    sb.Append(_collectionSudokuPossibleHolder[i].ToString() + "\r\n");
                }

                fileNameFullPath = _debugDirectory + "\\" + string.Format("ReturnItem{0}.txt", _numberOfCallsToReturnItem.ToString());
                Utility.CreateNewFile(fileNameFullPath, sb.ToString().TrimEnd());

                return item;
            }
        }
    }
}
