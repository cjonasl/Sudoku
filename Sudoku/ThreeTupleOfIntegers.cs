using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sudoku
{
    public class ThreeTupleOfIntegers
    {
        public int rowIndex, columnIndex, item;

        public ThreeTupleOfIntegers(int rIdx, int cIdx, int i)
        {
            rowIndex = rIdx;
            columnIndex = cIdx;
            item = i;
        }

        public override string ToString()
        {
            return string.Format("({0}, {1}, {2}", rowIndex + 1, columnIndex + 1, item);
        }
    }
}
