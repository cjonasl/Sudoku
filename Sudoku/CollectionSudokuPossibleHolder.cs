using System;
using System.Collections;
using System.Text;

namespace Sudoku
{
    public class CollectionSudokuPossibleHolder
    {
        private int _numberOfPossibleItems;
        private ArrayList _sudokuPossibleHolders;

        public CollectionSudokuPossibleHolder(int numberOfPossibleItems)
        {
            _numberOfPossibleItems = numberOfPossibleItems;
            _sudokuPossibleHolders = new ArrayList();
        }

        public void Add(int rowIndex, int columnIndex, int[] possibleItems)
        {
            _sudokuPossibleHolders.Add(new SudokuPossibleHolder(rowIndex, columnIndex, _numberOfPossibleItems, possibleItems));
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder(string.Format("{0}: ", _numberOfPossibleItems.ToString()));

            for (int i = 0; i < _sudokuPossibleHolders.Count; i++)
            {
                if (i > 0)
                {
                    sb.Append("   " + ((SudokuPossibleHolder)_sudokuPossibleHolders[i]).ToString());
                }
                else
                {
                    sb.Append(((SudokuPossibleHolder)_sudokuPossibleHolders[i]).ToString());
                }
            }

            return sb.ToString();
        }

        public ThreeTupleOfIntegers ReturnItem(Random r)
        {
            int n = r.Next(_sudokuPossibleHolders.Count);
            return ((SudokuPossibleHolder)_sudokuPossibleHolders[n]).ReturnItem(r);
        }

    }
}
