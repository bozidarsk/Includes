using System;

namespace Utils 
{
	public static class String2 
    {
        public static string en { get { return "abcdefghijklmnopqrstuvwxyz"; } }
        public static string bg { get { return "авбгдежзийклмнопрстуфхцчшщъьюя"; } }
        public static string chars { get { return "!\"#$%&'()*+,-./0123456789:;<=>?@[\\]^_`{|}~ "; } }

        public static bool IsNullOrEmpty(string str) { return str == null || str == "" || str.Length == 0; }

        public static string Join(params string[] strs) 
        {
        	if (strs == null || strs.Length == 0) { throw new NullReferenceException("Strings must not be null."); }

        	string output = "";
        	for (int i = 0; i < strs.Length; i++) { output += strs[i]; }
        	return output;
        }

		public static string Reverse(string str) 
		{
		    if (IsNullOrEmpty(str)) { throw new NullReferenceException("String must not be null."); }

		    string output = "";
		    for (int i = str.Length - 1; i >= 0; i--) { output += str[i]; }
		    return str;
		}

		public static int EndOfScope(string str, char open, char close, int start = 0) 
		{
			if (IsNullOrEmpty(str) || open == '\0' || close == '\0') { throw new NullReferenceException("Arguments must not be null."); }
			if (open == close) { throw new ArgumentException("Open and close char must be different."); }
			if (!str.Contains(open.ToString()) || !str.Contains(close.ToString())) { return -1; }

			int count = 0;
			for (int i = str.IndexOf(open, start); i < str.Length; i++) 
			{
				bool escape = false;
				try { escape = str[i - 1] == '\\'; } catch {}
				if (str[i] == open && !escape) { count++; }
				if (str[i] == close && !escape) { count--; }
				if (count <= 0) { return i; }
			}

			return -1;
		}

		public static string GetStringAt(string str, int start, int end) 
		{
		    if (IsNullOrEmpty(str)) { throw new NullReferenceException("String must not be null."); }
    		if (start < 0 || start > end || end >= str.Length) { throw new ArgumentOutOfRangeException("Start index must be greater than 0 and less than end index and end index must be less than the length of the string."); }

		    string output = "";
		    for (int i = start; i <= end; i++) { output += str[i]; }

		    return output;
		}

		public static int Count(string str, char target) 
		{
			if (IsNullOrEmpty(str)) { throw new NullReferenceException("String must not be null."); }

			int count = 0;
			for (int i = 0; i < str.Length; i++) { if (str[i] == target) { count++; } }
			return count;
		}

		public static int Count(string str, string target) 
		{
			if (IsNullOrEmpty(str) || IsNullOrEmpty(target)) { throw new NullReferenceException("Arguments must not be null."); }
			if (target.Length > str.Length) { throw new ArgumentException("String length must be greater than the length of target."); }

			int i = str.IndexOf(target);
			int count = 0;

			while (i >= 0) 
			{
				i = str.IndexOf(target, i + 1);
				count++;
			}

			return count;
		}

		public static string Replace(string str, string from, string to, int count) 
		{
			if (IsNullOrEmpty(str) || IsNullOrEmpty(from) || IsNullOrEmpty(to)) { throw new NullReferenceException("Strings must not be null."); }
			if (from.Length > str.Length) { throw new ArgumentException("String length must be greater than from length."); }
			if (count <= 0) { throw new ArgumentException("Count must be greater than 0."); }

			int i = str.IndexOf(from);
			if (i < 0) { return  str; }

			string removed = str.Remove(i, from.Length);
			string inserted = removed.Insert(i, to);

			return (inserted.Contains(from)) ? Replace(inserted, from, to, count - 1) : inserted;
		}

		public static string FillString(string str, char target) 
		{
			if (IsNullOrEmpty(str) || target == '\0') { throw new NullReferenceException("Arguments must not be null."); }

			for (int i = 0; i < str.Length; i++) { str += target; }
			return str;
		}

		public static string FillString(char target, int count) 
		{
			if (target == '\0') { throw new NullReferenceException("Char must not be null."); }
			if (count <= 0) { throw new ArgumentException("Count must be greater than 0."); }

			string output = "";
			for (int i = 0; i < count; i++) { output += target; }
			return output;
		}
    }
}