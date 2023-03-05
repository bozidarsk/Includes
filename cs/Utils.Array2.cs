using System;
using System.Collections.Generic;
using System.Linq;

namespace Utils 
{
	public static class Array2 
	{
		public static T[] GetArrayAt<T>(T[] array, int start, int end) 
		{
			T[] output = new T[(end - start) + 1];
			for (int i = start; i <= end; i++) { output[i - start] = array[i]; }
			return output;
		}

		public static T[] Join<T>(params T[][] arrays) 
		{
			List<T> output = new List<T>();
			for (int i = 0; i < arrays.Length; i++) { output.AddRange(arrays[i]); }
			return output.ToArray();
		}

		public static T[] Shuffle<T>(T[] array)  
        {  
        	if (array == null) { throw new NullReferenceException("Array must not be null."); }

            int n = array.Length;
            Random random = new Random();
            while (n > 1) 
            {  
                n--;
                int k = random.Next(n + 1);
                T tmp = array[k];
                array[k] = array[n];
                array[n] = tmp;
            }

            return array;
        }

		public static T[][][] Switch<T>(T[,,] array) 
		{
			T[][][] output = new T[array.GetLength(0)][][];

			for (int x = 0; x < array.GetLength(0); x++) 
			{
				output[x] = new T[array.GetLength(1)][];
				for (int y = 0; y < array.GetLength(1); y++) 
				{
					output[x][y] = new T[array.GetLength(2)];
					for (int z = 0; z < array.GetLength(2); z++) 
					{
						output[x][y][z] = array[x, y, z];
					}
				}
			}

			return output;
		}

		public static T[][] Switch<T>(T[,] array) 
		{
			T[][] output = new T[array.GetLength(0)][];

			for (int x = 0; x < array.GetLength(0); x++) 
			{
				output[x] = new T[array.GetLength(1)];
				for (int y = 0; y < array.GetLength(1); y++) 
				{
					output[x][y] = array[x, y];
				}
			}

			return output;
		}

		public static T[,,] Switch<T>(T[][][] array) 
		{
			T[,,] output = new T[array.Length, array[0].Length, array[0][0].Length];

			for (int x = 0; x < array.Length; x++) 
			{
				for (int y = 0; y < array[x].Length; y++) 
				{
					for (int z = 0; z < array[x][y].Length; z++) 
					{
						output[x, y, z] = array[x][y][z];
					}
				}
			}

			return output;
		}

		public static T[,] Switch<T>(T[][] array) 
		{
			T[,] output = new T[array.Length, array[0].Length];

			for (int x = 0; x < array.Length; x++) 
			{
				for (int y = 0; y < array[x].Length; y++) 
				{
					output[x, y] = array[x][y];
				}
			}

			return output;
		}
	}
}