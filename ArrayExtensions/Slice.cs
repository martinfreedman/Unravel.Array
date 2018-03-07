using System;

namespace Unravel.Array
{
    internal static class Slice
    {
        static readonly string[] _axes = new string[] { "row", "col" };
        public const int _row = 0;
        public const int _col = 1;
        public enum MajorAxis { ByRows, ByCols }


        internal static void ThrowIfOutOfRange<T>(T[,] matrix, int axis, int axe)
        {
            if (axe < matrix.GetLowerBound(axis) || axe > matrix.GetUpperBound(axis))
                throw new ArgumentOutOfRangeException(_axes[axe]);
        }

        internal static void ThrowIfOutOfRange<T>(T[,] matrix, int axis, int start, int length)
        {
            if (matrix.GetLength(axis) == 0)
                return;

            if (start < 0 || start > matrix.GetUpperBound(axis))
                throw new ArgumentOutOfRangeException(nameof(start));
            if (length < 1 || start + length > matrix.GetLength(axis))
                throw new ArgumentOutOfRangeException(nameof(length));
        }

        internal static void ThrowIfOutOfRange<T>(T[,] matrix, int axis, int axe, int start, int length)
        {
            ThrowIfOutOfRange(matrix, axis, axe);
            ThrowIfOutOfRange(matrix, axis, start, length);
        }

        internal static ((int, int), (int, int)) SetSliceOrThrow<T>(this T[,] matrix, int rowSkip, int rowTake, int colSkip, int colTake)
        {
            if (rowTake != 0 && colTake != 0)
            {
                ThrowIfOutOfRange(matrix, _col, colSkip, colTake);
                ThrowIfOutOfRange(matrix, _row, rowSkip, rowTake);
            }
            else if (rowTake != 0)
            {
                ThrowIfOutOfRange(matrix, _row, rowSkip, rowTake);
                (colSkip, colTake) = SetSliceColDefault(matrix);
            }
            else if (colTake != 0)
            {
                ThrowIfOutOfRange(matrix, _col, colSkip, colTake);
                (rowSkip, rowTake) = SetSliceRowDefault(matrix);
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
            int skip, take;

            skip = matrix.GetLowerBound(item);
            take = skip + matrix.GetLength(item);

            return (skip, take);
        }
    }
}
