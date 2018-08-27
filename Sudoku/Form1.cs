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
        private TextBox[,] _sudokuCells;
        private ArrayList _resultStrings;
        private ArrayList _originalData;
        public Form1()
        {
            InitializeComponent();
            CreateSudokuBoardInTheForm();

            string str = "Position 1: Iteration numner\r\nPosition 2: Numner of item(s) set in sudoku board\r\nPosition 3: Number of item(s) not set in sudoku board\r\n";
            str += "Position 4: Total number of item(s) set in iteration\r\nPosition 5: Total number of item(s) set by alone in cell\r\n";
            str += "Position 6: Total number of item(s) set by alone possible              (in row, column and/or square)\r\n";
            str += "Position 7: Total number of items possible to set without              causing conflict\r\nPosition 8: ";
            str += "Error not possible to set any item in cell\r\nPosition 9: Error not unique item alone possible\r\nPosition 10: Simulated one item (true or false)";
            this.textBoxInfo.Text = str;
            this.textBoxInfo.ReadOnly = true;
            this.buttonNew.Enabled = false;
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

            _sudokuCells = new TextBox[9, 9];

            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    _sudokuCells[i, j] = new TextBox();
                    _sudokuCells[i, j].Multiline = true;
                    _sudokuCells[i, j].Name = string.Format("Row{0}Column{1}", i + 1, j + 1);
                    _sudokuCells[i, j].Size = new Size(CELL_WIDTH, CELL_HEIGHT);
                    x = START_X + (j * CELL_WIDTH) + (j * SPACE_BETWEEN_CELL_X) + (j / 3) * EXTRA_SPACE_BETWEEN_SQUARES_X;
                    y = START_Y + (i * CELL_HEIGHT) + (i * SPACE_BETWEEN_CELL_Y) + (i / 3) * EXTRA_SPACE_BETWEEN_SQUARES_Y;
                    _sudokuCells[i, j].Font = new System.Drawing.Font("Courier New", 11F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
                    _sudokuCells[i, j].Location = new Point(x, y);
                    this.panel1.Controls.Add(_sudokuCells[i, j]);
                }
            }
        }

        private void buttonRun_Click(object sender, EventArgs e)
        {
            string debugDirectory, errorMessage, sudoku = Utility.ReturnSudokuString(_sudokuCells);
            int i;
            string maxTriesString;
            int maxNumberOfTries;
            SudokuBoard sudokuBoard;

            debugDirectory = ConfigurationManager.AppSettings["DebugDirectory"];
            if (debugDirectory == null)
            {
                MessageBox.Show("The key DebugDirectory is not given in App.config!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (!Directory.Exists(debugDirectory))
            {
                MessageBox.Show("The given DebugDirectory \"" + debugDirectory + "\" does not exist!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            maxTriesString = ConfigurationManager.AppSettings["MaxNumberOfTries"];
            if (maxTriesString == null)
            {
                MessageBox.Show("The key MaxNumberOfTries is not given in App.config!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (!int.TryParse(maxTriesString, out maxNumberOfTries))
            {
                MessageBox.Show("The value \"" + maxTriesString + "\" to key MaxNumberOfTries is not a valid integer!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if ((maxNumberOfTries < 1) || (maxNumberOfTries > 100))
            {
                MessageBox.Show("The value \"" + maxTriesString + "\" to key MaxNumberOfTries must be between 1 and 100!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            sudokuBoard = Utility.ReturnSudokuBoard(sudoku, out _originalData, out errorMessage);

            if (errorMessage == null)
            {
                SolveSudoku solveSudoku = new SolveSudoku(debugDirectory, maxNumberOfTries, sudokuBoard, _originalData, true);          
                ArrayList result = solveSudoku.Process(out _resultStrings);

                for(i = 1; i <= _resultStrings.Count; i++)
                {
                    this.listBox1.Items.Add("Try" + i.ToString());
                }

                this.buttonNew.Enabled = true;
                this.buttonRun.Enabled = false;

                Utility.UpdateSudokuCells(_sudokuCells, _originalData, result);

                if (sudokuBoard.SudokuIsSolved)
                {
                    MessageBox.Show("Sudoku solved!", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show("Unable to solve sudoku!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

                this.listBox1.SelectedIndexChanged += new System.EventHandler(this.listBox1_SelectedIndexChanged);
                this.listBox1.SelectedIndex = 0;
            }
            else
            {
                MessageBox.Show("The sudoku board is incorrect! Error message: " + errorMessage, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            this.buttonNew.Focus();
        }

        private void buttonBrowse_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            string sudoku, errorMessage;
            SudokuBoard sudokuBoard;
            ArrayList originalData;
            int n;

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                this.textBoxDataFromFile.Text = openFileDialog.FileName;
                sudoku = Utility.ReturnFileContents(openFileDialog.FileName);
                sudokuBoard = Utility.ReturnSudokuBoard(sudoku, out originalData, out errorMessage);

                if (errorMessage == null)
                {
                    for (int i = 0; i < 9; i++)
                    {
                        for (int j = 0; j < 9; j++)
                        {
                            n = sudokuBoard.ReturnItem(i, j);
                            _sudokuCells[i, j].Text = n == 0 ? "" : n.ToString();
                        }
                    }
                }
                else
                {
                    this.textBoxDataFromFile.Clear();
                    MessageBox.Show("The file is incorrect! Error message: " + errorMessage, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                this.textBoxDataFromFile.Clear();
            }
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.textBox1.Text = (string)_resultStrings[listBox1.SelectedIndex];
        }

        private void buttonNew_Click(object sender, EventArgs e)
        {
            this.listBox1.SelectedIndexChanged -= new System.EventHandler(this.listBox1_SelectedIndexChanged);
            this.listBox1.Items.Clear();
            Utility.ResetBackcolor(_sudokuCells, _originalData);
            this.textBoxDataFromFile.Clear();
            Utility.ClearSudokuCells(_sudokuCells);
            this.textBox1.Clear();
            this.buttonNew.Enabled = false;
            this.buttonRun.Enabled = true;
        }
    }
}
