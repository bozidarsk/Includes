using System;

namespace Utils 
{
	public static class ColorConsole 
	{
		public static readonly string BlackF       = "\x00ff";
		public static readonly string BlueF        = "\x09ff";
		public static readonly string CyanF        = "\x0bff";
		public static readonly string DarkBlueF    = "\x01ff";
		public static readonly string DarkCyanF    = "\x03ff";
		public static readonly string DarkGrayF    = "\x08ff";
		public static readonly string DarkGreenF   = "\x02ff";
		public static readonly string DarkMagentaF = "\x05ff";
		public static readonly string DarkRedF     = "\x04ff";
		public static readonly string DarkYellowF  = "\x06ff";
		public static readonly string GrayF        = "\x07ff";
		public static readonly string GreenF       = "\x0aff";
		public static readonly string MagentaF     = "\x0dff";
		public static readonly string RedF         = "\x0cff";
		public static readonly string WhiteF       = "\x0fff";
		public static readonly string YellowF      = "\x0eff";

		public static readonly string BlackB       = "\x10ff";
		public static readonly string BlueB        = "\x19ff";
		public static readonly string CyanB        = "\x1bff";
		public static readonly string DarkBlueB    = "\x11ff";
		public static readonly string DarkCyanB    = "\x13ff";
		public static readonly string DarkGrayB    = "\x18ff";
		public static readonly string DarkGreenB   = "\x12ff";
		public static readonly string DarkMagentaB = "\x15ff";
		public static readonly string DarkRedB     = "\x14ff";
		public static readonly string DarkYellowB  = "\x16ff";
		public static readonly string GrayB        = "\x17ff";
		public static readonly string GreenB       = "\x1aff";
		public static readonly string MagentaB     = "\x1dff";
		public static readonly string RedB         = "\x1cff";
		public static readonly string WhiteB       = "\x1fff";
		public static readonly string YellowB      = "\x1eff";

		public static void WriteLine(string input) { ColorConsole.Write(input + "\n"); }
		public static void Write(string input) 
		{
			/*
              0 = foreground color, 1 = backgroun color
			  |the actual color from System.Console.Color
			  ||indicates a color change
			  ||||
			\x07ff
			*/

			int i = 0;
			int color = 7;
			bool useBackground = false;

			while (i < input.Length) 
			{
				int n = (BitConverter.GetBytes(input[i])[1] << 8) + BitConverter.GetBytes(input[i])[0];
				if ((n & 0xff) << 8 == 0xff00) 
				{
					color = (n >> 8) & 0xf;
					useBackground = (((n >> 8) & 0x10) >> 4) == 1;
					i++;
				}

				if (!useBackground) { System.Console.ForegroundColor = (System.ConsoleColor)color; }
				else { System.Console.BackgroundColor = (System.ConsoleColor)color; }
				System.Console.Write(input[i++]);
			}
		}
	}
}