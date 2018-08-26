using System;
using System.Text;

namespace Sudoku
{
    public class SudokuPossibleHolder
    {
        private int _rowIndex, _columnIndex, _numberOfPossibleItems;
        private int[] _possibleItems;

        public SudokuPossibleHolder(int rowIndex, int columnIndex, int numberOfPossibleItems, int[] possibleItems)
        {
            _rowIndex = rowIndex;
            _columnIndex = columnIndex;
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
            return string.Format("[{0}, {1}, {2}]", (_rowIndex + 1).ToString(), (_columnIndex + 1).ToString(), PossibleItemsString());
        }

        public ThreeTupleOfIntegers ReturnItem(Random r)
        {
            int n = r.Next(_numberOfPossibleItems);
            return new ThreeTupleOfIntegers(_rowIndex, _columnIndex, _possibleItems[n]);
        }
    }
}
