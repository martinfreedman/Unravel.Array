using System;

namespace Unravel.Array
{
    internal static class Slice
    {
        public const int _row = 0;
        public const int _col = 1;

        internal static (int,int) ThrowIfOutOfRange<T>(T[,] matrix, int axis, int skip, int take)
        {
            if (matrix.GetLength(axis) == 0)
                return (0,0);

            if (skip < 0 || skip > matrix.GetUpperBound(axis))
                throw new ArgumentOutOfRangeException(nameof(skip));
            if (take < 1 || take + skip > matrix.GetLength(axis)) 
                throw new ArgumentOutOfRangeException(nameof(take));

            return (skip, take + skip);
        }

        internal static ((int, int), (int, int)) SetSliceOrThrow<T>(this T[,] matrix, int rowSkip, int rowTake, int colSkip, int colTake)
        {
            if (rowTake != 0 && colTake != 0)
            {
                (rowSkip, rowTake) = ThrowIfOutOfRange(matrix, _row, rowSkip, rowTake);
                (colSkip, colTake) = ThrowIfOutOfRange(matrix, _col, colSkip, colTake);
            }
            else if (rowTake != 0)
            {
                (rowSkip, rowTake) = ThrowIfOutOfRange(matrix, _row, rowSkip, rowTake);
                (colSkip, colTake) = SetSliceColDefault(matrix);
            }
            else if (colTake != 0)
            {
                (rowSkip, rowTake) = SetSliceRowDefault(matrix);
                (colSkip, colTake) = ThrowIfOutOfRange(matrix, _col, colSkip, colTake);
            }
            else
            {
                ((rowSkip, rowTake), (colSkip, colTake)) = SetSliceDefault(matrix);
            }

            return ((rowSkip, rowTake), (colSkip, colTake));
        }

        internal static ((int, int), (int, int)) SetSliceDefault<T>(this T[,] m) => (m.SetSliceRowDefault(), m.SetSliceColDefault());
        internal static (int, int) SetSliceRowDefault<T>(this T[,] m) => m.SetSliceItemDefault(_row);
        internal static (int, int) SetSliceColDefault<T>(this T[,] m) => m.SetSliceItemDefault(_col);

        internal static (int, int) SetSliceItemDefault<T>(this T[,] matrix, int item)
        {
            int start, length;

            start = matrix.GetLowerBound(item);
            length = matrix.GetLength(item);

            return (start, length);
        }
    }
}
