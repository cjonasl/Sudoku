using System;
using System.IO;
using System.Text;
using System.Configuration;
using System.Windows.Forms;
using System.Collections;
using System.Drawing;

namespace Sudoku
{
    public partial class Form1 : Form
    {
        private string dir = "C:\\git_cjonasl\\Sudoku";

        public Form1()
        {
            InitializeComponent();
            CreateSudokuBoardInTheForm();

            string str = "Position 1: Iteration numner\r\nPosition 2: Numner of item(s) set in sudoku board\r\nPosition 3: Number of item(s) not set in sudoku board\r\n";
            str += "Position 4: Total number of item(s) set in iteration\r\nPosition 5: Total number of item(s) set by alone in cell\r\n";
            str += "Position 6: Total number of item(s) set by alone possible              (in row, column and/or square)\r\n";
            str += "Position 7: Total number of items possible to set without              violation\r\nPosition 8: Simulated one item, true or false (if true                 then \"Total number of item(s) set in ite-                  ration\" must be 0)";
            this.textBoxInfo.Text = str;
            this.textBoxInfo.ReadOnly = true;

            /*
            int mx = -7, my = 0, mw = 1380, mh = 735, tx = 15, ty = 110, tw = 1340, th = 565; //Default values

            if ((Screen.PrimaryScreen.WorkingArea.Width == 1366) && (Screen.PrimaryScreen.WorkingArea.Height == 728))
            {
                mx = -7;
                my = 0;
                mw = 1380;
                mh = 735;
                tx = 15;
                ty = 110;
                tw = 1340;
                th = 565;
            }

            this.Location = new Point(mx, my);
            this.Size = new Size(mw, mh);
            this.textBox1.Location = new Point(tx, ty);
            this.textBox1.Size = new Size(tw, th); */
        }

        private void CreateSudokuBoardInTheForm()
        {
            const int START_X = 14;
            const int START_Y = 10;
            const int CELL_WIDTH = 30;
            const int CELL_HEIGHT = 25;
            const int SPACE_BETWEEN_CELL_X = 10;
            const int SPACE_BETWEEN_CELL_Y = 10;
            const int EXTRA_SPACE_BETWEEN_SQUARES_X = 7;
            const int EXTRA_SPACE_BETWEEN_SQUARES_Y = 7;
            int x, y;

            TextBox[,] textBoxes = new TextBox[9, 9];

            for(int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    textBoxes[i, j] = new TextBox();
                    textBoxes[i, j].Multiline = true;
                    textBoxes[i, j].Name = string.Format("Row{0}Column{1}", i + 1, j + 1);
                    textBoxes[i, j].Size = new Size(CELL_WIDTH, CELL_HEIGHT);
                    x = START_X + (j * CELL_WIDTH) + (j * SPACE_BETWEEN_CELL_X) + (j / 3) * EXTRA_SPACE_BETWEEN_SQUARES_X;
                    y = START_Y + (i * CELL_HEIGHT) + (i * SPACE_BETWEEN_CELL_Y) + (i / 3) * EXTRA_SPACE_BETWEEN_SQUARES_Y;
                    textBoxes[i, j].Font = new System.Drawing.Font("Courier New", 11F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
                    textBoxes[i, j].Location = new Point(x, y);
                    this.panel1.Controls.Add(textBoxes[i, j]);
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
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
