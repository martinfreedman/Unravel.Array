# Unravel.Array
Enumerable extensions to rectangular arrays

## Overview ##
Adds IEnumerable<T> enumerations to T[,] regular arrays with optional indexing, grouping and two-dimensional slicing.

Now no need to convert to T[][] jagged arrays to enable Linq. Saving both conversion looping over original data and additional memory storage.
If you need transposed data as well, then can also access this directly from the 2d array.

Flatten the whole or transposed array to a single enumerable of cells, optional 2D indices to each cell, optional add 2D slice.
Enumerate into enumerable rows or enumerable columns, optional 2D indices to each cell, optional row or column keys (grouping), 
optional add 2D slice. 
    
## Status ##

First beta release

## To do ##

1) Add versioning
2) Public API comments
3) Enhanced readme.
4) Final Beta release
5) Full documentation
6) Add Nuget/Release
