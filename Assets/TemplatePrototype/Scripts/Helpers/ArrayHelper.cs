using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HyrphusQ.Helpers
{
    #region Extension
    public static class ArrayHelper
    {
        public static void FillAll<T>(this T[,] array, T value)
        {
            int rows = array.GetLength(0);
            int cols = array.GetLength(1);

            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    array[i, j] = value;
                }
            }
        }
        public static void FillAll<T>(this T[] array, T value)
        {
            for (int i = 0; i < array.Length; i++)
            {
                array[i] = value;
            }
        }
    }
    #endregion
}