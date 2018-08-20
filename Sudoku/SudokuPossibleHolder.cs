using System;
using System.Text;

namespace Sudoku
{
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

            for (int i = 0; i < numberOfPossibleItems; i++)
            {
                _possibleItems[i] = possibleItems[i];
            }
        }

        private string PossibleItemsString()
        {
            StringBuilder sb = new StringBuilder("{");

            for (int i = 0; i < _numberOfPossibleItems; i++)
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
}
