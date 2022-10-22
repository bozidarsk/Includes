#if UNITY
using UnityEngine;
#endif

using System;

namespace Utils.Collections.Generic 
{
	public class List<T> : System.Collections.IEnumerable
	{
		public int Count { get { return position; } }
		public int Capacity { get { return array.Length; } }
		public T this[int index] { set { array[index] = value; } get { return array[index]; } }

		private T[] array;
		private int position;

		public void Remove(T item) { RemoveRange(IndexOf(item), 1); }
		public void RemoveAt(int index) { RemoveRange(index, 1); }
		public void RemoveRange(int start, int count) 
		{
			if (start < 0 || count > Count || start + count >= Count) { throw new IndexOutOfRangeException(); }
			for (int i = start; i < Count - position; i++) { array[i] = array[i + count]; }
			position -= count;
		}

		public void Insert(T item, int index) 
		{
			if (index < 0 || index >= Count + 1) { throw new IndexOutOfRangeException(); }
			if (Count >= Capacity) { DoubleCapacity(); }
			for (int i = position++; i > index; i--) { array[i] = array[i - 1]; }
			array[index] = item;
		}

		public void AddRange(params T[] items) { for (int i = 0; i < items.Length; i++) { Add(items[i]); } }
		public void Add(T item) 
		{
			if (Count >= Capacity) { DoubleCapacity(); }
			array[position++] = item;
		}

		public bool Contains(T item) { for (int i = 0; i < Count; i++) { if (array[i].Equals(item)) { return true; } } return false;  }
		public int IndexOf(T item) { for (int i = 0; i < Count; i++) { if (array[i].Equals(item)) { return i; } } return -1;  }
		public int IndexOf(T item, int start) { for (int i = start; i < Count; i++) { if (array[i].Equals(item)) { return i; } } return -1;  }
		public int IndexOf(T item, int start, int count) { for (int i = start; i < Count && count-- >= 0; i++) { if (array[i].Equals(item)) { return i; } } return -1;  }
		public int LastIndexOf(T item) { for (int i = Count - 1; i >= 0; i--) { if (array[i].Equals(item)) { return i; } } return -1;  }
		public int LastIndexOf(T item, int start) { for (int i = start; i >= 0; i--) { if (array[i].Equals(item)) { return i; } } return -1;  }
		public int LastIndexOf(T item, int start, int count) { for (int i = start; i >= 0 && count-- >= 0; i--) { if (array[i].Equals(item)) { return i; } } return -1;  }

		public T[] ToArray() { return array; }
		public void Clear() { position = 0; }
		public void Sort() { Array.Sort(array); }
		public void Reverse() { Array.Reverse(array); }

		public void TrimExcess() 
		{
			T[] newArray = new T[Count];
			for (int i = 0; i < Count; i++) { newArray[i] = array[i]; }
			array = newArray;
		}

		private void DoubleCapacity() 
		{
			T[] newArray = new T[Capacity * 2];
			for (int i = 0; i < Capacity; i++) { newArray[i] = array[i]; }
			array = newArray;
		}

		[Obsolete("Not tested.")]
		public List() 
		{
			this.array = new T[2];
			this.position = 0;
		}

		[Obsolete("Not tested.")]
		public List(int size) 
		{
			this.array = new T[size];
			this.position = 0;
		}

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() { return (System.Collections.IEnumerator) GetEnumerator(); }
		public ListEnum<T> GetEnumerator() { return new ListEnum<T>(this); }
	}

	public class ListEnum<T> : System.Collections.IEnumerator 
	{
		public List<T> list;
		private int position = -1;
		public ListEnum(List<T> list) { this.list = list; }

		public bool MoveNext() { position++; return position < list.Count; }
		public void Reset() { position = -1; }
		object System.Collections.IEnumerator.Current { get { return Current; } }
		public T Current 
		{
			get 
			{
				try { return list[position]; }
				catch (System.IndexOutOfRangeException) { throw new System.InvalidOperationException(); }
			}
		}
	}

	public class Dictionary<TKey, TValue> 
	{
		public int Count { get { return keys.Count; } }
		public TValue this[TKey key] 
		{
			set 
			{
				int index = keys.IndexOf(key);
				if (index < 0) { throw new System.Collections.Generic.KeyNotFoundException(); }
				values[keys.IndexOf(key)] = value;
			}

			get 
			{
				int index = keys.IndexOf(key);
				if (index < 0) { throw new System.Collections.Generic.KeyNotFoundException(); }
				return values[keys.IndexOf(key)];
			}
		}

		private List<TKey> keys;
		private List<TValue> values;

		public void Add(TKey key, TValue value) 
		{
			keys.Add(key);
			values.Add(value);
		}

		public void Remove(TKey key) { TValue value; Remove(key, out value); }
		public void Remove(TKey key, out TValue value) 
		{
			int index = keys.IndexOf(key);
			if (index < 0) { throw new System.Collections.Generic.KeyNotFoundException(); }
			value = values[index];
			keys.RemoveAt(index);
			values.RemoveAt(index);
		}

		public bool ContainsKey(TKey key) { return keys.Contains(key); }
		public bool ContainsValue(TValue value) { return values.Contains(value); }

		public void Clear() { keys.Clear(); values.Clear(); }
		public void TrimExcess() { keys.TrimExcess(); values.TrimExcess(); }

		[Obsolete("Not tested.")]
		public Dictionary() 
		{
			this.keys = new List<TKey>(2);
			this.values = new List<TValue>(2);
		}

		[Obsolete("Not tested.")]
		public Dictionary(int size) 
		{
			this.keys = new List<TKey>(size);
			this.values = new List<TValue>(size);
		}
	}

	public class Tree<T> 
	{
		[Obsolete("Not tested.")]
		public Tree() 
		{
			throw new NotImplementedException();
		}
	}
}