namespace Utils.Parsers 
{
	public static class Convert 
	{
		public static uint HexToUInt(string x) 
		{
			uint result = 0;
			string chars = "0123456789abcdef";
			for (int i = 0; i < x.Length; i++) { result += (uint)chars.IndexOf(x[i]); result <<= 4; }
			return result;
		}

		public static string UIntToHex(uint x, int minLength = 1) 
		{
			string result = "";
			string chars = "0123456789abcdef";
			for (; x > 0; x >>= 4) { result = chars[(int)x & 0xf] + result; }
			while (result.Length < minLength) { result = "0" + result; }
			return result;
		}

		public static uint BinToUInt(string x) 
		{
			uint result = 0;
			for (int i = 0; i < x.Length; i++) { result += (uint)x[i] - 0x30; result <<= 1; }
			return result;
		}

		public static string UIntToBin(uint x, int minLength = 1) 
		{
			string result = "";
			for (; x > 0; x >>= 1) { result = ((x & 0x1) + 0x30).ToString() + result; }
			while (result.Length < minLength) { result = "0" + result; }
			return result;
		}
	}
}