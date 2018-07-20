using System;
using System.IO;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

    public class SudokuIterator
    {
        public int[,,] _rows, _columns, _squares;
        public int[,] _numberOfPossibleNumbersRows, _numberOfPossibleNumbersColumns, _numberOfPossibleNumbersSquares;
        public int _numberOfNewNumbersPossibleToSetInSudokuBoard;

        public SudokuIterator()
        {
            _rows = new int[9, 9, 9];
            _columns = new int[9, 9, 9];
            _squares = new int[9, 9, 9];
            _numberOfPossibleNumbersRows = new int[9, 9];
            _numberOfPossibleNumbersColumns = new int[9, 9];
            _numberOfPossibleNumbersSquares = new int[9, 9];
        }

        private void Init()
        {
            _numberOfNewNumbersPossibleToSetInSudokuBoard = 0;

            for(int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    _numberOfPossibleNumbersRows[i, j] = -1;
                    _numberOfPossibleNumbersColumns[i, j] = -1;
                    _numberOfPossibleNumbersSquares[i, j] = -1;

                }
            }
        }

        private bool IsNumberAloneInRow(int rowIndex, int number)
        {
            int i, j, numberOfOccurenciesOfNumber = 0;

            for(i = 0; i < 9; i++)
            {
                for (j = 0; j < _numberOfPossibleNumbersRows[rowIndex, i]; j++)
                {
                    if (_rows[rowIndex, i, j] == number)
                    {
                        numberOfOccurenciesOfNumber++;
                    }
                }
            }

            if (numberOfOccurenciesOfNumber == 0)
            {
                throw new Exception("(numberOfOccurenciesOfNumber == 0) in method IsNumberAloneInRow");
            }

            return numberOfOccurenciesOfNumber == 1;
        }

        private bool IsNumberAloneInColumn(int columnIndex, int number)
        {
            int i, j, numberOfOccurenciesOfNumber = 0;

            for (i = 0; i < 9; i++)
            {
                for (j = 0; j < _numberOfPossibleNumbersColumns[columnIndex, i]; j++)
                {
                    if (_columns[columnIndex, i, j] == number)
                    {
                        numberOfOccurenciesOfNumber++;
                    }
                }
            }

            if (numberOfOccurenciesOfNumber == 0)
            {
                throw new Exception("(numberOfOccurenciesOfNumber == 0) in method IsNumberAloneInColumn");
            }

            return numberOfOccurenciesOfNumber == 1;
        }

        private bool IsNumberAloneInSquare(int squareIndex, int number)
        {
            int i, j, numberOfOccurenciesOfNumber = 0;

            for (i = 0; i < 9; i++)
            {
                for (j = 0; j < _numberOfPossibleNumbersSquares[squareIndex, i]; j++)
                {
                    if (_squares[squareIndex, i, j] == number)
                    {
                        numberOfOccurenciesOfNumber++;
                    }
                }
            }

            if (numberOfOccurenciesOfNumber == 0)
            {
                throw new Exception("(numberOfOccurenciesOfNumber == 0) in method IsNumberAloneInSquare");
            }

            return numberOfOccurenciesOfNumber == 1;
        }

        private void FillTempIntArray(int[,,] v, int i, int j, int n, int[] tmpIntArray)
        {
            for(int h = 0; h < n; h++)
            {
                tmpIntArray[h] = v[i, j, h];
            }
        }

        public int[,] ReturnNewNumbersPossibleToSetInSudokuBoard(SudokuBoard sudokuBoard, out int numberOfNewNumbersPossibleToSetInSudokuBoard, out string debugString)
        {
            int i, j, k, n, squareIndex, squareSequenceIndex, numberOfNumbersPossibleToSet;
            int[,] possibleToSetInSudokuBoard;
            int[] tmpIntArray;
            string[,] debug;
            StringBuilder sb;
            ArrayList tmpArrayList;
            string rowColumn;
            bool errorFound = false;

            possibleToSetInSudokuBoard = new int[9, 9];
            debug = new string[9, 9];
            tmpIntArray = new int[9];
            numberOfNewNumbersPossibleToSetInSudokuBoard = 0;
            debugString = "";

            sb = new StringBuilder();
            tmpArrayList = new ArrayList();

            for (i = 0; i < 9; i++)
            {
                for (j = 0; j < 9; j++)
                {
                    possibleToSetInSudokuBoard[i, j] = 0;
                }
            }

            Init();

            for (i = 0; i < 9; i++)
            {
                for (j = 0; j < 9; j++)
                {
                    squareIndex = (3 * (i / 3)) + (j / 3);
                    squareSequenceIndex = (3 * (i % 3)) + (j % 3);

                    if (!sudokuBoard.NumberIsSet(i, j))
                    {
                        _numberOfPossibleNumbersRows[i, j] = 0;
                        _numberOfPossibleNumbersColumns[j, i] = 0;
                        _numberOfPossibleNumbersSquares[squareIndex, squareSequenceIndex] = 0;

                        for (k = 1; k <= 9; k++)
                        {
                            if (sudokuBoard.CanSetNumber(i, j, k))
                            {
                                _rows[i, j, _numberOfPossibleNumbersRows[i, j]] = k;
                                _numberOfPossibleNumbersRows[i, j]++;

                                _columns[j, i, _numberOfPossibleNumbersColumns[j, i]] = k;
                                _numberOfPossibleNumbersColumns[j, i]++;

                                _squares[squareIndex, squareSequenceIndex, _numberOfPossibleNumbersSquares[squareIndex, squareSequenceIndex]] = k;
                                _numberOfPossibleNumbersSquares[squareIndex, squareSequenceIndex]++;
                            }
                        }
                    }
                }
            }

            for (i = 0; i < 9; i++)
            {
                for (j = 0; j < 9; j++)
                {
                    rowColumn = string.Format("[{0},{1}]: ", (i + 1).ToString(), (j + 1).ToString());

                    squareIndex = (3 * (i / 3)) + (j / 3);

                    if (!sudokuBoard.NumberIsSet(i, j))
                    {
                        if (_numberOfPossibleNumbersRows[i, j] == 0)
                        {
                            errorFound = true;
                            debug[i, j] = rowColumn + "ERROR!! Not possible to put any number in board square that does not cause conflict";
                        }
                        else if (_numberOfPossibleNumbersRows[i, j] == 1)
                        {
                            debug[i, j] = rowColumn + string.Format("CAN SET NUMBER {0}. Number {0} alone possible in board square", _rows[i, j, 0].ToString());
                            possibleToSetInSudokuBoard[i, j] = _rows[i, j, 0];
                            numberOfNewNumbersPossibleToSetInSudokuBoard++;
                        }
                        else
                        {
                            numberOfNumbersPossibleToSet = 0;
                            sb.Clear();
                            tmpArrayList.Clear();

                            for (k = 0; k < _numberOfPossibleNumbersRows[i, j]; k++)
                            {
                                if (IsNumberAloneInRow(i, _rows[i, j, k]))
                                {
                                    if (tmpArrayList.IndexOf(_rows[i, j, k]) == -1)
                                    {
                                        tmpArrayList.Add(_rows[i, j, k]);
                                        numberOfNumbersPossibleToSet++;
                                    }

                                    sb.Append(string.Format(", number {0} alone possible in row", _rows[i, j, k].ToString()));
                                }

                                if (IsNumberAloneInColumn(j, _rows[i, j, k]))
                                {
                                    if (tmpArrayList.IndexOf(_rows[i, j, k]) == -1)
                                    {
                                        tmpArrayList.Add(_rows[i, j, k]);
                                        numberOfNumbersPossibleToSet++;
                                    }

                                    sb.Append(string.Format(", number {0} alone possible in column", _rows[i, j, k].ToString()));
                                }

                                if (IsNumberAloneInSquare(squareIndex, _rows[i, j, k]))
                                {
                                    if (tmpArrayList.IndexOf(_rows[i, j, k]) == -1)
                                    {
                                        tmpArrayList.Add(_rows[i, j, k]);
                                        numberOfNumbersPossibleToSet++;
                                    }

                                    sb.Append(string.Format(", number {0} alone possible in square", _rows[i, j, k].ToString()));
                                }
                            }

                            FillTempIntArray(_rows, i, j, _numberOfPossibleNumbersRows[i, j], tmpIntArray);

                            if (numberOfNumbersPossibleToSet == 0)
                            {
                                debug[i, j] = rowColumn + "Can not set any number. Numbers not causing conflict: " + Utility.ReturnString(tmpIntArray, _numberOfPossibleNumbersRows[i, j]);
                            }
                            else if (numberOfNumbersPossibleToSet == 1)
                            {
                                possibleToSetInSudokuBoard[i, j] = (int)tmpArrayList[0];
                                numberOfNewNumbersPossibleToSetInSudokuBoard++;
                                debug[i, j] = rowColumn + "CAN SET NUMBER " + tmpArrayList[0].ToString() + ". Number(s) not causing conflict: " + Utility.ReturnString(tmpIntArray, _numberOfPossibleNumbersRows[i, j])  + sb.ToString();
                            }
                            else
                            {
                                errorFound = true;
                                debug[i, j] = rowColumn + "ERROR!! Can set more than one number: " + Utility.ReturnString(tmpArrayList) + ". Number(s) not causing conflict: " + Utility.ReturnString(tmpIntArray, _numberOfPossibleNumbersRows[i, j]) + ", " + sb.ToString();
                            }
                        }
                    }
                    else
                    {
                        debug[i, j] = rowColumn + string.Format("Board square is already set with number {0}", sudokuBoard.ReturnNumber(i, j).ToString());
                    }
                }
            }

            if (errorFound)
                numberOfNewNumbersPossibleToSetInSudokuBoard = 0;

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

            debugString = sb.ToString();

            return possibleToSetInSudokuBoard;
        }
    }
}
