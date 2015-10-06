//-----------------------------------------------------------------------
// <copyright file="Matrix.cs" company="MyToolkit">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/MyToolkit/MyToolkit/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System;
using System.Xml.Serialization;

namespace MyToolkit.Mathematics
{
    public class Matrix
    {
        public int Columns;

        public int Rows;

        internal Matrix() { } // Serialization only

        public Matrix(bool[,] sourceMatrix)
        {
            Data = new double[sourceMatrix.GetUpperBound(0) + 1, sourceMatrix.GetUpperBound(1) + 1];
            Columns = sourceMatrix.GetUpperBound(1) + 1;
            Rows = sourceMatrix.GetUpperBound(0) + 1;

            for (var r = 0; r < Rows; r++)
            {
                for (var c = 0; c < Columns; c++)
                    this[r, c] = sourceMatrix[r, c] ? 1 : -1;
            }
        }

        public Matrix(double[,] sourceMatrix, bool createNewArray = true)
        {
            Columns = sourceMatrix.GetUpperBound(1) + 1;
            Rows = sourceMatrix.GetUpperBound(0) + 1;

            if (createNewArray)
            {
                Data = new double[sourceMatrix.GetUpperBound(0) + 1, sourceMatrix.GetUpperBound(1) + 1];
                for (var r = 0; r < Rows; r++)
                {
                    for (var c = 0; c < Columns; c++)
                        this[r, c] = sourceMatrix[r, c];
                }
            }
            else
                Data = sourceMatrix;
        }

        public Matrix(int rows, int cols)
        {
            Data = new double[rows, cols];
            Columns = cols;
            Rows = rows;
        }

        public Matrix(double[] array, int rows) : this(rows, array.Length / rows)
        {
            var i = 0; 
            for (var r = 0; r < Rows; r++)
            {
                for (var c = 0; c < Columns; c++)
                    Data[r, c] = array[i++];
            }
        }

        public int FromPackedArray(double[] array, int startIndex)
        {
            for (var r = 0; r < Rows; r++)
            {
                for (var c = 0; c < Columns; c++)
                    Data[r, c] = array[startIndex++];
            }
            return startIndex;
        }

        /// <exception cref="ArgumentException">The matrixSize parameter must be bigger than 0. </exception>
        public static Matrix Identity(int matrixSize)
        {
            if (matrixSize < 1)
                throw new ArgumentException("The matrixSize parameter must be bigger than 0. ");

            var result = new Matrix(matrixSize, matrixSize);
            for (var i = 0; i < matrixSize; i++)
                result[i, i] = 1;
            return result;
        }

        public static Matrix CreateRowMatrix(double[] input)
        {
            var data = new double[1, input.Length];
            for (var i = 0; i < input.Length; i++)
                data[0, i] = input[i];
            return new Matrix(data, false);
        }

        public static Matrix CreateColumnMatrix(double[] input)
        {
            var data = new double[input.Length, 1];
            for (var row = 0; row < data.Length; row++)
                data[row, 0] = input[row];
            return new Matrix(data, false);
        }
        
        public double this[int row, int col]
        {
            get { return Data[row, col]; }
            set
            {
#if DEBUG
                if (Double.IsInfinity(value) || Double.IsNaN(value))
                    throw new ArgumentOutOfRangeException("The value does not fall within expected range. ");
#endif

                Data[row, col] = value;
            }
        }

        [XmlIgnore]
        public double[,] Data;

        public double[] RawData // used for serialization
        {
            get
            {
                var output = new double[Columns * Rows];
                var i = 0; 
                for (var y = 0; y < Rows; y++)
                {
                    for (var x = 0; x < Columns; x++)
                    {
                        output[i] = Data[y, x];
                        i++; 
                    }
                }
                return output; 
            }
            set
            {
                Data = new double[Rows, Columns];
                for (var y = 0; y < Rows; y++)
                {
                    for (var x = 0; x < Columns; x++)
                        Data[y, x] = value[y * Columns + x];
                }
            }
        }
    
        public void Add(int row, int col, double value)
        {
            this[row, col] = this[row, col] + value;
        }

        public void Clear(double value = 0)
        {
            for (var r = 0; r < Rows; r++)
            {
                for (var c = 0; c < Columns; c++)
                    this[r, c] = value;
            }
        }

        public Matrix Clone()
        {
            return new Matrix(Data);
        }

        public bool Equals(Matrix matrix)
        {
            return Equals(matrix, 0.0001);
        }

        public bool Equals(Matrix matrix, double precision)
        {
            for (var r = 0; r < Rows; r++)
            {
                for (var c = 0; c < Columns; c++)
                {
                    if (Math.Abs(this[r, c] - matrix[r, c]) > precision)
                        return false;
                }
            }
            return true;
        }

        public Matrix GetColumn(int column)
        {
            if (column > Columns)
                throw new ArgumentException("column does not exist.");

            var newMatrix = new double[Rows, 1];
            for (var row = 0; row < Rows; row++)
                newMatrix[row, 0] = Data[row, column];

            return new Matrix(newMatrix, false);
        }

        public Matrix GetRow(int row)
        {
            if (row > Rows)
                throw new ArgumentException("row does not exist.");

            var newMatrix = new double[1, Columns];
            for (var col = 0; col < Columns; col++)
                newMatrix[0, col] = Data[row, col];

            return new Matrix(newMatrix, false);
        }

        public bool IsVector
        {
            get { return Rows == 1 || Columns == 1; }
        }

        public bool IsZero
        {
            get
            {
                for (var row = 0; row < Rows; row++)
                {
                    for (var col = 0; col < Columns; col++)
                    {
                        if (Data[row, col] != 0)
                            return false;
                    }
                }
                return true;
            }
        }

        public void Ramdomize(double min, double max)
        {
            var rand = new Random((int)(DateTime.Now.Ticks % Int32.MaxValue));
            for (var r = 0; r < Rows; r++)
            {
                for (var c = 0; c < Columns; c++)
                    Data[r, c] = (rand.NextDouble() * (max - min)) + min;
            }
        }

        public int Size
        {
            get { return Rows * Columns; }
        }

        public double[] ToPackedArray()
        {
            var result = new double[Rows * Columns];
            var index = 0;
            for (var r = 0; r < Rows; r++)
            {
                for (var c = 0; c < Columns; c++)
                    result[index++] = Data[r, c];
            }
            return result;
        }

        public static Matrix operator +(Matrix a, Matrix b)
        {
            return a.Add(b);
        }

        public static Matrix operator -(Matrix a, Matrix b)
        {
            return a.Subtract(b);
        }

        public static Matrix operator /(Matrix a, double divisor)
        {
            return a.Divide(divisor);
        }

        public static Matrix operator *(Matrix a, double factor)
        {
            return a.Multiply(factor);
        }

        public static Matrix operator *(Matrix a, Matrix b)
        {
            return a.Multiply(b);
        }
    }
}