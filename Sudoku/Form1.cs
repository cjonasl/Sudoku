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
        private TextBox[,] sudokuCells;

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

            sudokuCells = new TextBox[9, 9];

            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    sudokuCells[i, j] = new TextBox();
                    sudokuCells[i, j].Multiline = true;
                    sudokuCells[i, j].Name = string.Format("Row{0}Column{1}", i + 1, j + 1);
                    sudokuCells[i, j].Size = new Size(CELL_WIDTH, CELL_HEIGHT);
                    x = START_X + (j * CELL_WIDTH) + (j * SPACE_BETWEEN_CELL_X) + (j / 3) * EXTRA_SPACE_BETWEEN_SQUARES_X;
                    y = START_Y + (i * CELL_HEIGHT) + (i * SPACE_BETWEEN_CELL_Y) + (i / 3) * EXTRA_SPACE_BETWEEN_SQUARES_Y;
                    sudokuCells[i, j].Font = new System.Drawing.Font("Courier New", 11F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
                    sudokuCells[i, j].Location = new Point(x, y);
                    this.panel1.Controls.Add(sudokuCells[i, j]);
                }
            }
        }

        private void buttonRun_Click(object sender, EventArgs e)
        {
            string debugDirectory, errorMessage, sudoku = Utility.ReturnSudokuString(this.sudokuCells);
            ArrayList originalData;
            int maxTries;
            SudokuBoard sudokuBoard = Utility.ReturnSudokuBoard(sudoku, out originalData, out errorMessage);

            debugDirectory = ConfigurationManager.AppSettings["DebugDirectory"];
            maxTries = int.Parse(ConfigurationManager.AppSettings["MaxTries"]);

            if (errorMessage == null)
            {
                
            }
            else
            {
                MessageBox.Show("The sudoku board is incorrect! Error message: " + errorMessage, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        private void buttonBrowse_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            string sudoku, errorMessage;
            SudokuBoard sudokuBoard;
            int n;

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                this.textBoxDataFromFile.Text = openFileDialog.FileName;
                sudoku = Utility.ReturnFileContents(openFileDialog.FileName);
                sudokuBoard = Utility.ReturnSudokuBoard(sudoku, out errorMessage);

                if (errorMessage == null)
                {
                    for (int i = 0; i < 9; i++)
                    {
                        for (int j = 0; j < 9; j++)
                        {
                            n = sudokuBoard.ReturnNumber(i, j);
                            sudokuCells[i, j].Text = n == 0 ? "" : n.ToString();
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
    }
}
