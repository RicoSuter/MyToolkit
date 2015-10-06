//-----------------------------------------------------------------------
// <copyright file="MatrixMath.cs" company="MyToolkit">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/MyToolkit/MyToolkit/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System;
using System.Linq;

namespace MyToolkit.Mathematics
{
    public static class MatrixMath
    {
        public static void Copy(this Matrix source, Matrix target)
        {
            if (source.Rows != target.Rows)
                throw new ArgumentException("row count does not match");
            if (source.Columns != target.Columns)
                throw new ArgumentException("column count does not match");

            for (var row = 0; row < source.Rows; row++)
            {
                for (var col = 0; col < source.Columns; col++)
                    target[row, col] = source[row, col];
            }
        }

        public static Matrix DeleteColumn(this Matrix matrix, int columnToDelete)
        {
            if (columnToDelete >= matrix.Columns)
                throw new ArgumentException("column does not exist");

            var newMatrix = new double[matrix.Rows, matrix.Columns - 1];
            for (var row = 0; row < matrix.Rows; row++)
            {
                var targetCol = 0;
                for (var col = 0; col < matrix.Columns; col++)
                {
                    if (col != columnToDelete)
                    {
                        newMatrix[row, targetCol] = matrix[row, col];
                        targetCol++;
                    }
                }
            }

            return new Matrix(newMatrix, false);
        }

        public static Matrix DeleteRow(this Matrix matrix, int rowToDelete)
        {
            if (rowToDelete >= matrix.Rows)
                throw new ArgumentException("row does not exist");

            var targetRow = 0;
            var newMatrix = new double[matrix.Rows - 1, matrix.Columns];
            for (var row = 0; row < matrix.Rows; row++)
            {
                if (row != rowToDelete)
                {
                    for (var col = 0; col < matrix.Columns; col++)
                        newMatrix[targetRow, col] = matrix[row, col];
                    targetRow++;
                }
            }

            return new Matrix(newMatrix, false);
        }

        public static Matrix Divide(this Matrix matrix, double divisor)
        {
            var result = new double[matrix.Rows, matrix.Columns];
            for (var row = 0; row < matrix.Rows; row++)
            {
                for (var col = 0; col < matrix.Columns; col++)
                    result[row, col] = matrix[row, col] / divisor;
            }
            return new Matrix(result, false);
        }

        public static double DotProduct(this Matrix a, Matrix b)
        {
            if (!a.IsVector || !b.IsVector)
                throw new ArgumentException("two vectors expected");

            //var length = a.Data.GetLength(0);
            //if ((length != a.Data.GetLength(1) && a.Data.GetLength(1) != 1) || (b.Data.GetLength(0) != a.Data.GetLength(1) && a.Data.GetLength(0) != 1))
            //	throw new ArgumentException();
    
            //if (length != b.Data.GetLength(1))
            //	throw new ArgumentException("vectors don't have same length");

            //var result = 0.0;
            //for (var i = 0; i < length; i++)
            //	result += a.Data[i, 0] * b.Data[0, i];
            //return result;

            var aArray = a.ToPackedArray();
            var bArray = b.ToPackedArray();

            if (aArray.Length != bArray.Length)
                throw new ArgumentException("vectors don't have same length");

            var result = 0.0;
            for (var i = 0; i < aArray.Length; i++)
                result += aArray[i] * bArray[i];
            return result;
        }

        public static Matrix Multiply(this Matrix a, double factor)
        {
            var result = new double[a.Rows, a.Columns];
            for (var row = 0; row < a.Rows; row++)
            {
                for (var col = 0; col < a.Columns; col++)
                    result[row, col] = a[row, col] * factor;
            }
            return new Matrix(result, false);
        }

        public static Matrix Multiply(this Matrix a, Matrix b)
        {
            if (a.Columns != b.Rows)
                throw new ArgumentException("a.Columns != b.Rows");

            var result = new double[a.Rows, b.Columns];
            for (var row = 0; row < a.Rows; row++)
            {
                for (var column = 0; column < b.Columns; column++)
                {
                    var value = 0.0;
                    for (var i = 0; i < a.Columns; i++)
                        value += a[row, i] * b[i, column];
                    result[row, column] = value;
                }
            }

            return new Matrix(result, false);
        }

        public static Matrix Add(this Matrix a, Matrix b)
        {
            if (a.Rows != b.Rows)
                throw new ArgumentException("row count does not match");
            if (a.Columns != b.Columns)
                throw new ArgumentException("column count does not match");

            var result = new double[a.Rows, a.Columns];
            for (var resultRow = 0; resultRow < a.Rows; resultRow++)
            {
                for (var resultCol = 0; resultCol < a.Columns; resultCol++)
                    result[resultRow, resultCol] = a[resultRow, resultCol] + b[resultRow, resultCol];
            }

            return new Matrix(result, false);
        }

        public static Matrix Subtract(this Matrix a, Matrix b)
        {
            if (a.Rows != b.Rows)
                throw new ArgumentException("row count does not match");
            if (a.Columns != b.Columns)
                throw new ArgumentException("column count does not match");

            var result = new double[a.Rows, a.Columns];
            for (var resultRow = 0; resultRow < a.Rows; resultRow++)
            {
                for (var resultCol = 0; resultCol < a.Columns; resultCol++)
                    result[resultRow, resultCol] = a[resultRow, resultCol] - b[resultRow, resultCol];
            }

            return new Matrix(result, false);
        }

        public static Matrix Transpose(this Matrix input)
        {
            var inverseMatrix = new double[input.Columns, input.Rows];
            for (var r = 0; r < input.Rows; r++)
            {
                for (var c = 0; c < input.Columns; c++)
                    inverseMatrix[c, r] = input[r, c];
            }
            return new Matrix(inverseMatrix, false);
        }

        public static double VectorLength(this Matrix input)
        {
            if (!input.IsVector)
                throw new ArgumentException("input is not a vector");
            
            var v = input.ToPackedArray();
            return Math.Sqrt(v.Sum(t => Math.Pow(t, 2)));
        }
    }
}
