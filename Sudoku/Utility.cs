using System;
using System.Collections;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace Sudoku
{
    public static class Utility
    {
        public static SudokuBoard ReturnSudokuBoard(string sudoku, out ArrayList originalData, out string errorMessage)
        {
            string[] u, v;
            bool noErrorFound;
            int i, j, n;

            errorMessage = null;
            originalData = new ArrayList();

            SudokuBoard sudokuBoard = new SudokuBoard();

            u = sudoku.Split(new string[] { "\r\n" }, StringSplitOptions.None);

            if (u.Length != 9)
            {
                errorMessage = "Not exactly 9 rows";
                return null;
            }

            i = 1;
            noErrorFound = true;

            while (noErrorFound && (i <= 9))
            {
                v = u[i - 1].Split(' ');

                if (v.Length != 9)
                {
                    errorMessage = "Row " + i.ToString() + " does not contain exactly 9 columns one blank separated";
                    noErrorFound = false;
                }
                else
                {
                    j = 1;

                    while (noErrorFound && (j <= 9))
                    {
                        if (!int.TryParse(v[j - 1], out n))
                        {
                            errorMessage = "The value \"" + v[j - 1] + "\" in row " + i.ToString() + " and column " + j.ToString() + " is not a valid integer";
                            noErrorFound = false;
                        }
                        else
                        {
                            if ((n < 0) || (n > 9))
                            {
                                errorMessage = "The integer " + v[j - 1] + " in row " + i.ToString() + " and column " + j.ToString() + " is not in the range 0-9";
                                noErrorFound = false;
                            }
                            else if (n != 0)
                            {
                                try
                                {
                                    sudokuBoard.SetNumber(i - 1, j - 1, n);
                                    originalData.Add(new ThreeTupleOfIntegers(i - 1, j - 1, n));
                                }
                                catch(Exception e)
                                {
                                    errorMessage = e.Message;
                                    noErrorFound = false;
                                }
                            }
                        }

                        j++;
                    }
                }

                i++;
            }

            if (!noErrorFound)
            {
                return null;
            }

            return sudokuBoard;
        }

        public static string ReturnSudokuString(TextBox[,] sudokuCells)
        {
            StringBuilder sb = new StringBuilder();
            string str, item;

            for(int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    str = (j <= 7) ? " " : "\r\n";
                    item = string.IsNullOrEmpty(sudokuCells[i, j].Text) ? "0" : sudokuCells[i, j].Text;
                    sb.Append(string.Format("{0}{1}", item, str));
                }
            }

            return sb.ToString().TrimEnd();
        }

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

            for (int i = 0; i < v.Length; i++)
            {
                File.Delete(v[i]);
            }
        }

        public static void UpdateSudokuCells(TextBox[,] sudokuCells, ArrayList originalData, ArrayList result)
        {
            int i;
            ThreeTupleOfIntegers ttoi;

            for (i = 0; i < originalData.Count; i++)
            {
                ttoi = (ThreeTupleOfIntegers)originalData[i];
                sudokuCells[ttoi.rowIndex, ttoi.columnIndex].BackColor = System.Drawing.Color.LightGray;
            }

            for (i = 0; i < result.Count; i++)
            {
                ttoi = (ThreeTupleOfIntegers)result[i];
                sudokuCells[ttoi.rowIndex, ttoi.columnIndex].Text = ttoi.item.ToString();
            }
        }

        public static void ResetBackcolor(TextBox[,] sudokuCells, ArrayList originalData)
        {
            int i;
            ThreeTupleOfIntegers ttoi;

            for (i = 0; i < originalData.Count; i++)
            {
                ttoi = (ThreeTupleOfIntegers)originalData[i];
                sudokuCells[ttoi.rowIndex, ttoi.columnIndex].BackColor = System.Drawing.Color.White;
            }
        }

        public static void ClearSudokuCells(TextBox[,] sudokuCells)
        {
            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    sudokuCells[i, j].Clear();
                }
            }
        }

        public static void AddIfNotExistsAlready(ArrayList v, int n)
        {
            if (v.IndexOf(n) == -1)
            {
                v.Add(n);
            }
        }

        public static string ReturnCommaSeparatedStringOfIntegers(ArrayList v)
        {
            StringBuilder sb = new StringBuilder();

            for(int i = 0; i < v.Count; i++)
            {
                if (i == 0)
                {
                    sb.Append(v[i].ToString());
                }
                else
                {
                    sb.Append(", " + v[i].ToString());
                }
            }

            return sb.ToString();
        }
    }
}
