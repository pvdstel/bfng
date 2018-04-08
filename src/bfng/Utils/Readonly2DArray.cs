using System;
using System.Collections.Generic;
using System.Text;

namespace bfng.Utils
{
    /// <summary>
    /// A readonly two-dimensional array.
    /// </summary>
    /// <typeparam name="T">The type of the array.</typeparam>
    public class Readonly2DArray<T>
    {
        private T[,] _array;

        /// <summary>
        /// Initializes a new instance of the <see cref="Readonly2DArray{T}"/> class.
        /// </summary>
        /// <param name="array">The array to use as a value.</param>
        public Readonly2DArray(T[,] array)
        {
            _array = array ?? throw new ArgumentNullException(nameof(array));
        }

        /// <summary>
        /// Accesses an element from the 2D array. This is read-only.
        /// </summary>
        /// <param name="d1">The index along the first dimension.</param>
        /// <param name="d2">The index along the second dimension.</param>
        /// <returns>The element at position (<paramref name="d1"/>, <paramref name="d2"/>).</returns>
        public T this[int d1, int d2] => _array[d1, d2];

        /// <summary>
        /// Gets the total number of elements in all the dimensions of the array.
        /// </summary>
        public int Length => _array.Length;

        /// <summary>
        /// Gets the length of the array along the specified dimension.
        /// </summary>
        /// <param name="dimension">The dimension to get the length of.</param>
        /// <returns>The length of the array along the specified dimension.</returns>
        public int GetLength(int dimension)
        {
            return _array.GetLength(dimension);
        }

        /// <summary>
        /// Gets a mutable copy of the array by performing a shallow clone.
        /// </summary>
        /// <returns>A shallow clone of the array.</returns>
        public T[,] GetMutable()
        {
            return (T[,])_array.Clone();
        }
    }
}
