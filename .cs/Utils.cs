#if UNITY
using UnityEngine;
#endif

using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Drawing;
using System.Drawing.Imaging;
using System.Text;
using System.IO;
using System.Threading;
using System.Diagnostics;
using Microsoft.Win32;

using Utils.Collections;

namespace Utils 
{
    public static class Tools 
    {
        /*
        [System.Runtime.InteropServices.DllImport("user32.dll")]
		static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

		ShowWindow(System.Diagnostics.Process.GetCurrentProcess().MainWindowHandle, 0);
		*/

		[DllImport("user32.dll", CharSet = CharSet.Auto)]
		private static extern int SystemParametersInfo(int uAction, int uParam, string lpvParam, int fuWinIni);

		public static void SetWallpaper(Bitmap bitmap)
		{
			int style = 1;
		    // Tiled = 0
		    // Centered = 1
		    // Stretched = 2

		    Image img = Image.FromHbitmap(bitmap.GetHbitmap());
		    string tempPath = Path.Combine(Path.GetTempPath(), "wallpaper.bmp");
		    img.Save(tempPath, ImageFormat.Bmp);

		    RegistryKey key = Registry.CurrentUser.OpenSubKey(@"Control Panel\Desktop", true);
		    if (style == 2)
		    {
		        key.SetValue(@"WallpaperStyle", 2.ToString());
		        key.SetValue(@"TileWallpaper", 0.ToString());
		    }

		    if (style == 1)
		    {
		        key.SetValue(@"WallpaperStyle", 1.ToString());
		        key.SetValue(@"TileWallpaper", 0.ToString());
		    }

		    if (style == 0)
		    {
		        key.SetValue(@"WallpaperStyle", 1.ToString());
		        key.SetValue(@"TileWallpaper", 1.ToString());
		    }

		    SystemParametersInfo(0x14, 0, tempPath, 0x3);
		}

		public static void SetWallpaper(string file)
		{
			int style = 1;
		    // Tiled = 0
		    // Centered = 1
		    // Stretched = 2

		    Stream s = new System.Net.WebClient().OpenRead(file);
		    Image img = Image.FromStream(s);
		    string tempPath = Path.Combine(Path.GetTempPath(), "wallpaper.bmp");
		    img.Save(tempPath, ImageFormat.Bmp);

		    RegistryKey key = Registry.CurrentUser.OpenSubKey(@"Control Panel\Desktop", true);
		    if (style == 2)
		    {
		        key.SetValue(@"WallpaperStyle", 2.ToString());
		        key.SetValue(@"TileWallpaper", 0.ToString());
		    }

		    if (style == 1)
		    {
		        key.SetValue(@"WallpaperStyle", 1.ToString());
		        key.SetValue(@"TileWallpaper", 0.ToString());
		    }

		    if (style == 0)
		    {
		        key.SetValue(@"WallpaperStyle", 1.ToString());
		        key.SetValue(@"TileWallpaper", 1.ToString());
		    }

		    SystemParametersInfo(0x14, 0, tempPath, 0x3);
		}

		public static string GetCurrentFilePath() { return new StackTrace(true).GetFrame(0).GetFileName(); }

        public static void ClearLastLine() 
        {
            Console.SetCursorPosition(0, Console.CursorTop - 1);
            int currentLineCursor = Console.CursorTop;
            Console.SetCursorPosition(0, Console.CursorTop);
            Console.Write(new string(' ', Console.WindowWidth)); 
            Console.SetCursorPosition(0, currentLineCursor);
        }

        public static void CMD(string command) 
        {
		    Process proccess = new Process();
		    proccess.StartInfo = new ProcessStartInfo("C:\\Windows\\System32\\cmd.exe", command);
		    proccess.StartInfo.UseShellExecute = false;
		    proccess.Start();
		    proccess.WaitForExit();
        }

        public static void StartEXE(string path, string args) 
        {
        	Process proccess = new Process();
		    proccess.StartInfo = new ProcessStartInfo(path, args);
		    proccess.StartInfo.UseShellExecute = false;
		    proccess.Start();
		    proccess.WaitForExit();
        }

		public static string FormatDirectory(string input) 
		{
			input = input.Replace("/", "\\");
			string[] folders = input.Split('\\');
			List<string> newFolders = new List<string>();
			string output = "";

			for (int i = 0; i < folders.Length; i++) 
			{
				if (folders[i] == ".") { continue; }
				if (folders[i] != "..") { newFolders.Add(folders[i]); }
				else 
				{
					if (newFolders.Count == 0) { return null; }
					newFolders.RemoveAt(newFolders.Count - 1);
				}
			}

			for (int i = 0; i < newFolders.Count; i++) { output += newFolders[i] + "\\"; }
			return output.TrimEnd('\\');
		}

		public static byte[] GetBytes(int size, params long[] items) 
		{
			if (items == null || items.Length == 0) { throw new NullReferenceException("Items must not be null."); }
			if (size <= 0) { throw new ArgumentException("Size must be greater than 0."); }
			byte[] bytes = new byte[items.Length * size];
			int index = 0;

			for (int i = 0; i < items.Length; i++) 
			{
				for (int b = size - 1; b >= 0; b--) 
				{
					bytes[index++] = (byte)((items[i] >> (b * 8)) & 0xff);
				}
			}

			return bytes;
		}
    }

    public sealed class Timer 
    {
    	public int delay { set; get; }
    	public bool done { set; get; }

    	public void Stop() { if (thread != null && !done) { thread.Abort(); } }
    	public void Start() 
    	{
    		if (thread != null && !done) { thread.Abort(); }
			thread = new Thread(Timeout);
			thread.Start();
    	}

    	private Thread thread;
    	private void Timeout() 
    	{
    		done = false;
    		Thread.Sleep(delay);
    		done = true;
    	}

		public Timer(int delay) { this.delay = delay; done = true; }
    }

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

    public static class Format 
    {
    	public static string To1Decimal(float num) 
		{
			string output = Math.Round(num, 1).ToString();
			output = (output.IndexOf(".") <= -1) ? output + ".0" : output;
			return output;
		}

		public static string To2Decimals(float num) 
		{
			string output = Math.Round(num, 2).ToString();
			output = (output.IndexOf(".") < 0) ? output + ".00" : output;
			output = (output.IndexOf(".") == output.Length - 2) ? output + "0" : output;
			return output;
		}

		public static string To3Decimals(float num) 
		{
			string output = Math.Round(num, 3).ToString();
			output = (output.IndexOf(".") < 0) ? output + ".000" : output;
			output = (output.IndexOf(".") == output.Length - 2) ? output + "00" : output;
			output = (output.IndexOf(".") == output.Length - 3) ? output + "0" : output;
			return output;
		}

		public static string UIntToBin(uint x, int minLength = 1) 
		{
			if (minLength < 1) { throw new ArgumentOutOfRangeException("minLength must be greater than 0."); }

			string output = "";
			for (; x != 0; x >>= 1) { output = (((x & 1) == 1) ? "1" : "0") + output; }
			return (output.Length < minLength) ? String2.FillString('0', minLength - output.Length) + output : output;
		}

		public static uint BinToUInt(string x) 
		{
			if (string.IsNullOrEmpty(x)) { throw new NullReferenceException("Bin string must not be null."); }

			uint output = 0;
			for (int i = 0; i < x.Length; i++) 
			{
				if (x[i] != '0' && x[i] != '1') { throw new ArgumentException("Bin string is invalid. - '" + x + "'"); }
				output = (output << 1) + (uint)((x[i] == '1') ? 1 : 0);
			}

			return output;
		}

		public static uint HexToUInt(string x) 
		{
			if (string.IsNullOrEmpty(x)) { throw new NullReferenceException("Hex string must not be null."); }

			string chars = "0123456789abcdef";
			uint output = 0;

			x = x.ToLower();
			for (int i = 0; i < x.Length; i++) 
			{
				int n = chars.IndexOf(x[i].ToString());
				if (n == -1) { throw new ArgumentException("Hex string is invalid. - '" + x + "'"); }
				output = (output << 4) + (uint)n;
			}

			return output;
		}

		public static string UIntToHex(uint x, int minLength = 1) 
		{
			if (minLength < 1) { throw new ArgumentOutOfRangeException("minLength must be greater than 0."); }

			string chars = "0123456789abcdef";
			string output = "";
			for (; x != 0; x >>= 4) { output = chars[(int)(x & 0xf)] + output; }
			return (output.Length < minLength) ? String2.FillString('0', minLength - output.Length) + output : output;
		}

		public static float itof(int num) { return float.Parse(num.ToString()); }
		public static int ftoi(float num) { return System.Convert.ToInt32(num); }
		public static float stof(string num) { return float.Parse(num); }
		public static string ftos(float num) { return num.ToString(); }
		public static int stoi(string num) { return System.Convert.ToInt32(num); }
		public static string itos(int num) { return num.ToString(); }

		public static float ToMinutes(string hoursAndMinutes) 
		{
			if (hoursAndMinutes.IndexOf(".") >= 0 || hoursAndMinutes == "" || hoursAndMinutes == null || hoursAndMinutes.IndexOf(":") <= -1) { return 0f; }
			string[] time = hoursAndMinutes.Replace(":", " ").Split();
			if (time.Length != 2) { return 0f; }
			return (float.Parse(time[0]) * 60f) + float.Parse(time[1]);
		}

		public static float ToSeconds(string minutesAndSeconds) 
		{
			if (minutesAndSeconds.IndexOf(".") >= 0 || minutesAndSeconds == "" || minutesAndSeconds == null || minutesAndSeconds.IndexOf(":") <= -1) { return 0f; }
			string[] time = minutesAndSeconds.Replace(":", " ").Split();
			if (time.Length != 2) { return 0f; }
			return (float.Parse(time[0]) * 60f) + float.Parse(time[1]);
		}

		public static string ToHoursAndMinutes(float minutes) 
		{
			if (minutes < 0) { return "0:00"; }
			float newHours = Math.Floor(minutes / 60f);
			float newMinutes = Math.Abs(minutes - (newHours * 60f));
			string newMinutess = System.Convert.ToString(newMinutes);
			return System.Convert.ToString(newHours) + ":" + ((newMinutess.Length == 1) ? ("0" + newMinutess) : newMinutess);
		}

		public static string ToHoursAndMinutes(string minutes) 
		{
			float minutesf = float.Parse(minutes);
			if (minutes == "" || minutes == " " || minutesf < 0) { return "0:00"; }
			float newHours = Math.Floor(minutesf / 60f);
			float newMinutes = Math.Abs(minutesf - (newHours * 60f));
			string newMinutess = System.Convert.ToString(newMinutes);
			return System.Convert.ToString(newHours) + ":" + ((newMinutess.Length == 1) ? ("0" + newMinutess) : newMinutess);
		}

		public static string ToMinutesAndSeconds(float seconds) 
		{
			if (seconds < 0) { return "0:00"; }
			float newMinutes = Math.Floor(seconds / 60f);
			float newSeconds = Math.Abs(seconds - (newMinutes * 60f));
			string newSecondss = System.Convert.ToString(newSeconds);
			return System.Convert.ToString(newMinutes) + ":" + ((newSecondss.Length == 1) ? ("0" + newSecondss) : newSecondss);
		}

		public static string ToMinutesAndSeconds(string seconds) 
		{
			float secondsf = float.Parse(seconds);
			if (seconds == "" || seconds == " " || secondsf < 0) { return "0:00"; }
			float newMinutes = Math.Floor(secondsf / 60f);
			float newSeconds = Math.Abs(secondsf - (newMinutes * 60f));
			string newSecondss = System.Convert.ToString(newSeconds);
			return System.Convert.ToString(newMinutes) + ":" + ((newSecondss.Length == 1) ? ("0" + newSecondss) : newSecondss);
		}
    }

    public static class Meshes 
    {
    	#if UNITY
        public static Mesh Combine(MeshFilter[] filters) 
        {
            CombineInstance[] combiner = new CombineInstance[filters.Length];

            for (int i = 0; i < filters.Length; i++)
            {
                combiner[i].subMeshIndex = 0;
                combiner[i].mesh = filters[i].sharedMesh;
                combiner[i].transform = filters[i].transform.localToWorldMatrix;
            }

            Mesh newMesh = new Mesh();
            newMesh.CombineMeshes(combiner);

            return newMesh;
        }
        #endif

        public static Mesh Icosahedron() 
		{
			Mesh mesh = new Mesh();
			Vector3[] newVertices = new Vector3[12];
			float numA = 3f;
			float numB = 2f;
			int a = 0;
			int b = 4;
			int c = 8;
			
			newVertices[0] = new Vector3(numA / 2f, numB / 2f, 0f);
			newVertices[1] = new Vector3(numA / 2f, -numB / 2f, 0f);
			newVertices[2] = new Vector3(-numA / 2f, -numB / 2f, 0f);
			newVertices[3] = new Vector3(-numA / 2f, numB / 2f, 0f);

			newVertices[4] = new Vector3(numB / 2f, 0f, numA / 2f);
			newVertices[5] = new Vector3(numB / 2f, 0f, -numA / 2f);
			newVertices[6] = new Vector3(-numB / 2f, 0f, -numA / 2f);
			newVertices[7] = new Vector3(-numB / 2f, 0f, numA / 2f);

			newVertices[8] = new Vector3(0f, numA / 2f, numB / 2f);
			newVertices[9] = new Vector3(0f, -numA / 2f, numB / 2f);
			newVertices[10] = new Vector3(0f, -numA / 2f, -numB / 2f);
			newVertices[11] = new Vector3(0f, numA / 2f, -numB / 2f);

			int[] newTriangles = { a+2, a+3, b+2, a+2, b+2, c+2, b+1, c+2, b+2, c+3, b+2, a+3, c+3, b+1, b+2, c+3, a+0, b+1, c+3, c+0, a+0, c+0, b+0, a+0, c+0, b+3, b+0, c+3, a+3, c+0, c+0, a+3, b+3, a+3, a+2, b+3, b+3, a+2, c+1, b+0, b+3, c+1, a+0, b+0, a+1, b+0, c+1, a+1, b+1, a+0, a+1, b+1, a+1, c+2, c+2, a+1, c+1, c+2, c+1, a+2 };
			mesh.vertices = newVertices;
			mesh.triangles = newTriangles;
			return mesh;
		}

		public static Mesh IcoSphere(int resolution) 
		{
			Mesh mesh = Icosahedron();
			Vector3[] borderVertices = new Vector3[2];
			Vector3[] previousBorderVertices = new Vector3[2];
			List<Vector3> newVertices = new List<Vector3>();
			List<int> newTriangles = new List<int>();
			float section = 0f;
			float lineSection = 0f;

			int pointsInside = 0;
			int triangleIndex = 0;
			int line = 0;
			int t = 0;
			int v = 0;

			v = 0; while (v < mesh.vertices.Length) { newVertices.Add(mesh.vertices[v]); v++; }

			while (t < mesh.triangles.Length) 
			{
				borderVertices[0] = mesh.vertices[mesh.triangles[t]];
				borderVertices[1] = mesh.vertices[mesh.triangles[t + 1]];
				previousBorderVertices[0] = borderVertices[0];
				previousBorderVertices[1] = borderVertices[1];

				v = 0;
				lineSection = 1f / ((float)resolution + 1f);
				while (v < resolution) 
				{
					newVertices.Add(Math.MovePoint01(borderVertices[0], borderVertices[1], lineSection));
					lineSection += 1f / ((float)resolution + 1f);
					v++;
				}
				
				line = 0;
				lineSection = 1f / ((float)resolution + 1f);
				while (line < resolution) 
				{
					newVertices.Add(Math.MovePoint01(mesh.vertices[mesh.triangles[t]], mesh.vertices[mesh.triangles[t + 2]], lineSection));
					borderVertices[0] = newVertices[newVertices.Count - 1];
					newVertices.Add(Math.MovePoint01(mesh.vertices[mesh.triangles[t + 1]], mesh.vertices[mesh.triangles[t + 2]], lineSection));
					borderVertices[1] = newVertices[newVertices.Count - 1];
					lineSection += 1f / ((float)resolution + 1f);

					v = 0;
					pointsInside = 0;
					section = 1f / (float)(resolution - line);
					while (v < resolution - 1 - line) 
					{
						newVertices.Add(Math.MovePoint01(borderVertices[0], borderVertices[1], section));
						section += 1f / (float)(resolution - line);
						pointsInside++;
						v++;
					}

					triangleIndex = 0;
					while (triangleIndex < resolution - line) 
					{
						newTriangles.Add(newVertices.Count - ((2 + pointsInside + (resolution - line)) - triangleIndex));
						newTriangles.Add(newVertices.LastIndexOf(borderVertices[0]) + ((triangleIndex == 0) ? 0 : (2 + triangleIndex - 1)));
						newTriangles.Add((triangleIndex == 0) ? newVertices.LastIndexOf(previousBorderVertices[0]) : newVertices.Count - ((2 + pointsInside + (resolution - line)) - (triangleIndex - 1)));

						newTriangles.Add(newVertices.Count - ((2 + pointsInside + (resolution - line)) - triangleIndex));
						newTriangles.Add(((resolution - line) - 1 - triangleIndex <= 0) ? newVertices.LastIndexOf(borderVertices[1]) : newVertices.Count - ((resolution - line) - 1 - triangleIndex));
						newTriangles.Add(newVertices.LastIndexOf(borderVertices[0]) + ((triangleIndex == 0) ? 0 : (2 + triangleIndex - 1)));

						triangleIndex++;
					}
					
					triangleIndex--;
					newTriangles.Add(newVertices.Count - ((2 + pointsInside + (resolution - line)) - triangleIndex));
					newTriangles.Add(newVertices.LastIndexOf(previousBorderVertices[1]));
					newTriangles.Add(((resolution - line) - 1 - triangleIndex <= 0) ? newVertices.LastIndexOf(borderVertices[1]) : newVertices.Count - ((resolution - line) - 1 - triangleIndex));

					previousBorderVertices[0] = borderVertices[0];
					previousBorderVertices[1] = borderVertices[1];
					line++;
				}
				
				line--;
				triangleIndex--;
				newTriangles.Add(((resolution - line) - 1 - triangleIndex <= 0) ? newVertices.LastIndexOf(borderVertices[1]) : newVertices.Count - ((resolution - line) - 1 - triangleIndex));
				newTriangles.Add(mesh.triangles[t + 2]);
				newTriangles.Add(newVertices.LastIndexOf(borderVertices[0]) + ((triangleIndex == 0) ? 0 : (2 + triangleIndex - 1)));
				t += 3;
			}

			mesh.vertices = newVertices.ToArray();
			mesh.triangles = newTriangles.ToArray();
			return mesh;
		}
    }
}