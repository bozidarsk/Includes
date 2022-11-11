using System;
using System.Collections.Generic;

namespace Utils 
{
	public static class Array2 
    {
    	public static T[] Join<T>(params T[][] array) 
    	{
    		if (array == null || array.Length == 0) { throw new NullReferenceException("Array must not be null."); }

    		List<T> list = new List<T>();
    		for (int i = 0; i < array.Length; i++) 
    		{
    			for (int t = 0; t < array[i].Length; t++) 
    			{
    				list.Add(array[i][t]);
    			}
    		}

    		return list.ToArray();
    	}

    	public static T[] GetArrayAt<T>(T[] array, int start, int end) 
    	{
    		if (array == null || array.Length == 0) { throw new NullReferenceException("Array must not be null."); }
    		if (start < 0 || start > end || end >= array.Length) { throw new IndexOutOfRangeException("Start index must be grater than 0 and less than end index and end index must be less than the length of the array."); }

    		T[] output = new T[end - start + 1];
    		int index = 0;

    		for (int i = start; i <= end; i++) { output[index++] = array[i]; }
    		return output;
    	}

    	public static T[] Shuffle<T>(T[] array)  
        {  
        	if (array == null || array.Length == 0) { throw new NullReferenceException("Array must not be null."); }

            int n = array.Length;
            System.Random random = new System.Random();
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
    }
}