#region License and Terms
// Unravel.Array - Enumerable extensions to regular two dimensional arrays
// Copyright (c) 2018 Martin Freedman. All rights reserved.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
#endregion

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
