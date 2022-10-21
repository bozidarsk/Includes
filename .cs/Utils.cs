using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Imaging;
using System.Text;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Diagnostics;
using Microsoft.Win32;

using Utils.Web;
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

		    Stream s = new WebClient().OpenRead(file);
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

        public class Timer 
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
    }

    public class ColorConsole 
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

		public static int HexToDecimal(string hex) 
		{
			if (hex == "" || hex == " ") { return 0; }
			int sum = 0;
			int pow = 0;
			int i = hex.Length - 1;
			char[] hexBy1 = { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'a', 'b', 'c', 'd', 'e', 'f' };
			hex = hex.ToLower();
			while (i > -1) 
			{
				sum += Array.IndexOf(hexBy1, hex[i]) * (int)System.Math.Pow(16, pow);
				pow++;
				i--;
			}

			return sum;
		}

		public static string DecimalToHex(int dec) 
		{
			string[] hexBy1 = { "0", "1", "2", "3", "4", "5", "6", "7", "8", "9", "a", "b", "c", "d", "e", "f" };
			int num = (int)System.Math.Floor((double)dec / 16);
			string hex = hexBy1[dec % 16];

			while (num > 0) 
			{
				hex = hexBy1[num % 16] + hex;
				num = (int)System.Math.Floor((double)num / 16);
			}

			return hex;
		}

		public static int BinaryToDecimal(string bin) 
		{
			if (bin == "" || bin == " ") { return 0; }
			int sum = 0;
			int pow = 0;
			int i = bin.Length - 1;
			bin = bin.ToLower();
			while (i > -1) 
			{
				sum += ((bin[i] == '1') ? 1 : 0) * (int)Math.Pow(2, pow);
				pow++;
				i--;
			}

			return sum;
		}

		public static string DecimalToBinary(int dec) 
		{
			int num = (int)System.Math.Floor((double)dec / 2);
			string bin = ((dec % 2 == 0) ? "0" : "1");

			while (num > 0) 
			{
				bin = ((num % 2 == 0) ? 0 : 1) + bin;
				num = (int)System.Math.Floor((double)num / 2);
			}

			return bin;
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

    public static class Math 
    {
        public static float PI { get { return 3.1415926535897932384626433832795f; } }
        public static float TAU { get { return 6.283185307179586476925286766559f; } }
        public static float E { get { return 2.7182818284590452353602874713527f; } }
        public static float goldenRatio { get { return 1.6180339887498948482045868343657f; } }
        public static float DEG2RAD { get { return 0.01745329251994329576923690768489f; } }
        public static float RAD2DEG { get { return 57.295779513082320876798154814105f; } }
        public static float EPSILON { get { return 0.0001f; } }

        public static float e(float x) { return (float)System.Math.Pow((double)E, (double)x); }
        public static float Round(float x) { return Round(x, 0); }
        public static float Round(float x, int place) { return (float)System.Math.Round((double)x, place); }
	    public static float Sqrt(float x) { return (float)System.Math.Sqrt((double)x); }
	    public static float Abs(float x) { return (x < 0f) ? x * -1f : x; }
	    public static float Ceiling(float x) { return (float)System.Math.Ceiling((double)x); }
	    public static float Floor(float x) { return (float)System.Math.Floor((double)x); }
	    public static float Max(float a, float b) { return (a > b) ? a : b; }
	    public static float Min(float a, float b) { return (a < b) ? a : b; }
	    public static float Sin(float x) { return (float)System.Math.Sin((double)x); }
	    public static float Cos(float x) { return (float)System.Math.Cos((double)x); }
	    public static float Tan(float x) { return (float)System.Math.Tan((double)x); }
	    public static float Asin(float x) { return (float)System.Math.Asin((double)x); }
	    public static float Acos(float x) { return (float)System.Math.Acos((double)x); }
	    public static float Atan(float x) { return (float)System.Math.Atan((double)x); }
	    public static float Atan2(float y, float x) { return (float)System.Math.Atan2((double)y, (double)x); }
	    public static float Log(float x) { return (float)System.Math.Log((double)x); }
	    public static float Log(float x, float a) { return (float)System.Math.Log((double)x, (double)a); }
	    public static float Clamp(float a, float b, float x) { return Max(a, Min(x, b)); }
	    public static Vector3 Clamp(Vector3 a, Vector3 b, Vector3 x) { return (Distance(a, b) < Distance(a, x)) ? b : ((Distance(a, b) < Distance(b, x)) ? a : x); }
	    public static Vector2 Clamp(Vector2 a, Vector2 b, Vector2 x) { return (Distance(a, b) < Distance(a, x)) ? b : ((Distance(a, b) < Distance(b, x)) ? a : x); }
	    public static float InverseLerp(float a, float b, float x) { return (x - a) / (b - a); }
	    public static float Lerp(float a, float b, float x) { return a + (x * (b - a)); }
        public static Vector2 Lerp(Vector2 a, Vector2 b, float x) { return new Vector2(Lerp(a.x, b.x, x), Lerp(a.y, b.y, x)); }
        public static Vector3 Lerp(Vector3 a, Vector3 b, float x) { return new Vector3(Lerp(a.x, b.x, x), Lerp(a.y, b.y, x), Lerp(a.z, b.z, x)); }
        public static float SmoothMax(float a, float b, float x) { return a * Clamp(0f, 1f, (b - a + x) / (2f * x)) + b * (1f - Clamp(0f, 1f, (b - a + x) / (2f * x))) - x * Clamp(0f, 1f, (b - a + x) / (2f * x)) * (1f - Clamp(0f, 1f, (b - a + x) / (2f * x))); }
        public static float SmoothMin(float a, float b, float x) { return SmoothMax(a, b, -1f * x); }
        public static float SmoothStep(float a, float b, float x) { float t = Clamp(0, 1, (x - a) / (b - a)); return t * t * (3f - (2f * t)); }
        public static float Square(float a) { return a * a; }
        public static Vector2 Square(Vector2 a) { return new Vector2(a.x * a.x, a.y * a.y); }
        public static Vector3 Square(Vector3 a) { return new Vector3(a.x * a.x, a.y * a.y, a.z * a.z); }
        public static float GetInt(float x) { return (int)x; }
		public static float GetDecimal(float x) { return x - (float)((int)x); }

        public static float DistanceSquared(Vector3 a, Vector3 b) { return ((b.x - a.x) * (b.x - a.x)) + ((b.y - a.y) * (b.y - a.y)) + ((b.z - a.z) * (b.z - a.z)); }
        public static float DistanceSquared(Vector2 a, Vector2 b) { return ((b.x - a.x) * (b.x - a.x)) + ((b.y - a.y) * (b.y - a.y)); }
        public static float Distance(Vector3 a, Vector3 b) { return Sqrt(((b.x - a.x) * (b.x - a.x)) + ((b.y - a.y) * (b.y - a.y)) + ((b.z - a.z) * (b.z - a.z))); }
        public static float Distance(Vector2 a, Vector2 b) { return Sqrt(((b.x - a.x) * (b.x - a.x)) + ((b.y - a.y) * (b.y - a.y))); }
        public static float Length(Vector3 a) { return Sqrt((a.x * a.x) + (a.y * a.y) + (a.z * a.z)); }
        public static float Length(Vector2 a) { return Sqrt((a.x * a.x) + (a.y * a.y)); }
        public static Vector3 Normalize(Vector3 a) { return a / Length(a); }
        public static Vector2 Normalize(Vector2 a) { return a / Length(a); }
        public static float Dot(Vector3 a, Vector3 b) { return (a.x * b.x) + (a.y * b.y) + (a.z * b.z); }
        public static float Dot(Vector2 a, Vector2 b) { return (a.x * b.x) + (a.y * b.y); }
        public static Vector3 Cross(Vector3 a, Vector3 b) { return new Vector3((a.y*b.z) - (a.z*b.y), (a.z*b.x) - (a.x*b.z), (a.x*b.y) - (a.y*b.x)); }

        public static float AngleBetweenVectors(Vector2 a, Vector2 b) { return Acos(CosBetweenVectors(a, b)); }
        public static float AngleBetweenVectors(Vector3 a, Vector3 b) { return Acos(CosBetweenVectors(a, b)); }
        public static float SinBetweenVectors(Vector2 a, Vector2 b) { return Sqrt(1f - (CosBetweenVectors(a, b)*CosBetweenVectors(a, b))); }
        public static float SinBetweenVectors(Vector3 a, Vector3 b) { return Sqrt(1f - (CosBetweenVectors(a, b)*CosBetweenVectors(a, b))); }
        public static float CosBetweenVectors(Vector2 a, Vector2 b) { return Dot(a, b) / (Length(a) * Length(b)); }
        public static float CosBetweenVectors(Vector3 a, Vector3 b) { return Dot(a, b) / (Length(a) * Length(b)); }

        public static Vector3 PerpendicularToLine(Vector3 a, Vector3 b, Vector3 c) 
        {
        	float da = ((b.x - c.x) * (b.x - c.x)) + ((b.y - c.y) * (b.y - c.y)) + ((b.z - c.z) * (b.z - c.z));
        	float db = ((a.x - c.x) * (a.x - c.x)) + ((a.y - c.y) * (a.y - c.y)) + ((a.z - c.z) * (a.z - c.z));
        	float dc = ((b.x - a.x) * (b.x - a.x)) + ((b.y - a.y) * (b.y - a.y)) + ((b.z - a.z) * (b.z - a.z));
        	float t = -((da - db - dc) / (2f * dc));
        	return a + (t * (b - a));
        }

        public static Vector2 PerpendicularToLine(Vector2 a, Vector2 b, Vector2 c) 
        {
        	float da = ((b.x - c.x) * (b.x - c.x)) + ((b.y - c.y) * (b.y - c.y));
        	float db = ((a.x - c.x) * (a.x - c.x)) + ((a.y - c.y) * (a.y - c.y));
        	float dc = ((b.x - a.x) * (b.x - a.x)) + ((b.y - a.y) * (b.y - a.y));
        	float t = -((da - db - dc) / (2f * dc));
        	return a + (t * (b - a));
        }

        public static float AreaOfTriangle(Vector3 a, Vector3 b, Vector3 c) { return (Distance(a, b) * Distance(c, PerpendicularToLine(a, b, c))) / 2f; }
        public static float AreaOfTriangle(Vector2 a, Vector2 b, Vector2 c) { return (Distance(a, b) * Distance(c, PerpendicularToLine(a, b, c))) / 2f; }

        public static float DistanceToSquare(Vector2 a, Vector2 b, Vector2 c, Vector2 d, Vector2 p) 
        {
        	if (b.x < a.x) { Vector2 tmp = a; a = b; b = tmp; }
        	if (c.x < d.x) { Vector2 tmp = d; d = c; c = tmp; }
        	if (a.y > d.y) { Vector2 tmp = a; a = d; d = tmp; }
        	if (b.y > c.y) { Vector2 tmp = b; b = c; c = tmp; }

        	float tX = -((DistanceSquared(b, p) - DistanceSquared(a, p) - DistanceSquared(a, b)) / (2f * DistanceSquared(a, b)));
        	float tY = -((DistanceSquared(c, p) - DistanceSquared(b, p) - DistanceSquared(c, b)) / (2f * DistanceSquared(c, b)));

        	Vector2 hUp = Clamp(d, c, d + (tY * (c - d)));
    		Vector2 hDown = Clamp(a, b, a + (tY * (b - a)));
    		Vector2 hLeft = Clamp(a, d, a + (tX * (d - a)));
    		Vector2 hRight = Clamp(c, b, b + (tX * (c - b)));

    		float output = Min(Min(Distance(p, hUp), Distance(p, hDown)), Min(Distance(p, hLeft), Distance(p, hRight)));
    		return (hUp.y - p.y > 0f && p.y - hDown.y > 0f && p.x - hLeft.x > 0f && p.x - hRight.x < 0f) ? -output : output;
        }

        public static Vector2 RaySphere(Vector3 center, float radius, Vector3 rayOrigin, Vector3 rayDir) // returns new Vector2(distance to sphere, distance inside sphere)
		{
		    float a = 1f;
		    Vector3 offset = new Vector3(rayOrigin.x - center.x, rayOrigin.y - center.y, rayOrigin.z - center.z);
		    float b = 2f * Dot(offset, rayDir);
		    float c = Dot(offset, offset) - radius * radius;

		    float disciminant = b * b - 4f * a * c;

		    if (disciminant > 0f) 
		    {
		        float s = Sqrt(disciminant);
		        float dstToSphereNear = Max(0f, (-b - s) / (2f * a));
		        float dstToShpereFar = (-b + s) / (2f * a);

		        if (dstToShpereFar >= 0f) 
		        {
		            return new Vector2(dstToSphereNear, dstToShpereFar - dstToSphereNear);
		        }
		    }

		    return new Vector2(-1f, 0f);
		}

        public static Vector2 Rotate(Vector2 origin, Vector2 point, float angle) 
        {
            float x = origin.x + ((point.x - origin.x) * Cos(angle)) + ((point.y - origin.y) * Sin(angle));
            float y = origin.y + ((point.x - origin.x) * Sin(angle)) + ((point.y - origin.y) * Cos(angle));

            return new Vector2(x, y);
        }

        public static Vector3 MidPoint(Vector3 a, Vector3 b) { return new Vector3((a.x + b.x) / 2, (a.y + b.y) / 2, (a.z + b.z) / 2); }
        public static Vector2 MidPoint(Vector2 a, Vector2 b) { return new Vector2((a.x + b.x) / 2, (a.y + b.y) / 2); }
        public static Vector3 MovePoint(Vector3 a, Vector3 b, float distance) { return a + (distance * Normalize(b - a)); }
        public static Vector2 MovePoint(Vector2 a, Vector2 b, float distance) { return a + (distance * Normalize(b - a)); }
        public static Vector3 MovePoint01(Vector3 a, Vector3 b, float distance) { return Lerp(a, b, distance); }
        public static Vector2 MovePoint01(Vector2 a, Vector2 b, float distance) { return Lerp(a, b, distance); }

        public static float Pow(float a, float b) { return (float)System.Math.Pow((double)a, (double)b); }
        public static long Pow(int num, int pow) 
        {
            long newNum = num;
            if (pow < 0) { return 0; }
            if (pow == 0) { return 1; }
            while (pow - 1 > 0) { newNum *= num; pow--; }
            return newNum;
        }

        public static long Fact(int x) 
        {
            long newNum = 1;
            while (x > 0) 
            {
                newNum *= x;
                x--;
            }

            return newNum;
        }

        public static bool IsPrime(int x)
		{
		    if (x < 1) { return false; }
		    if (x == 1) { return true; }
		 
		    for (int i = 2; i <= (int)Math.Sqrt(x); i++) 
		    {
		        if (x % i == 0) { return false; }
		    }
		 
		    return true;
		}

		#if UNITY
		public static Vector3 MousePositionToWorldXY(Vector3 mousePosition) 
		{
			Ray mouseRay = UnityEditor.HandleUtility.GUIPointToWorldRay(mousePosition);
			float distanceToDrawPlane = -mouseRay.origin.z / mouseRay.direction.z;
			return mouseRay.origin + (mouseRay.direction * distanceToDrawPlane);
		}

		public static Vector3 MousePositionToWorldZY(Vector3 mousePosition) 
		{
			Ray mouseRay = UnityEditor.HandleUtility.GUIPointToWorldRay(mousePosition);
			float distanceToDrawPlane = -mouseRay.origin.x / mouseRay.direction.x;
			return mouseRay.origin + (mouseRay.direction * distanceToDrawPlane);
		}

		public static Vector3 MousePositionToWorldXZ(Vector3 mousePosition) 
		{
			Ray mouseRay = UnityEditor.HandleUtility.GUIPointToWorldRay(mousePosition);
			float distanceToDrawPlane = -mouseRay.origin.y / mouseRay.direction.y;
			return mouseRay.origin + (mouseRay.direction * distanceToDrawPlane);
		}
		#endif
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

    public static class String2 
    {
        public static string en { get { return "abcdefghijklmnopqrstuvwxyz"; } }
        public static string bg { get { return "авбгдежзийклмнопрстуфхцчшщъьюя"; } }
        public static string chars { get { return "!\"#$%&'()*+,-./0123456789:;<=>?@[\\]^_`{|}~ "; } }

        public static bool IsNullOrEmpty(string str) { return str == null || str == ""; }

        public static string Join(params string[] strs) { return Join("", strs); }
        public static string Join(string separator, params string[] strs) 
        {
        	string output = "";
        	for (int i = 0; i < strs.Length; i++) { output += strs[i] + separator; }
        	return output.Remove(output.Length - separator.Length, separator.Length);
        }

		public static string Remove(string str, int start) 
		{
			if (IsNullOrEmpty(str) || start < 0 || start >= str.Length) { return str; }

			int i = 0;
			string output = "";
			while (i < start) { output += str[i]; i++; }
			return output;
		}

		public static string Remove(string str, int start, int count) 
		{
			if (IsNullOrEmpty(str) || start + count > str.Length || count <= 0 || count > str.Length || start < 0 || start >= str.Length) { return str; }

			int i = 0;
			int s = 0;
			string output = "";
			while (i < str.Length) 
			{
				if (i == start) { i += count; }
				output += str[i];
				s++;
				i++;
			}

			return output;
		}

		public static string Reverse(string str) 
		{
		    if (IsNullOrEmpty(str)) { return str; }

		    string output = "";
		    for (int i = str.Length - 1; i >= 0; i--) { output += str[i]; }
		    return str;
		}

		public static int IndexOfScope(string main, char open, char close) 
		{
			if (main == null || main == "" || open == close || !(main.Contains(open.ToString()) && main.Contains(close.ToString()))) { throw new ArgumentException(); }

			int count = 0;
			for (int i = main.IndexOf(open); i < main.Length; i++) 
			{
				bool escape = false;
				try { escape = main[i - 1] == '\\'; } catch {}
				if (main[i] == open && !escape) { count++; }
				if (main[i] == close && !escape) { count--; }
				if (count <= 0) { return i; }
			}

			return -1;
		}

		public static string GetStringAt(string str, int startPos, int endPos) 
		{
		    if (IsNullOrEmpty(str) || startPos < 0 || endPos <= 0 || endPos < startPos || endPos >= str.Length) { return ""; }

		    string output = "";
		    int i = startPos;
		    int t = 0;
		    while (t < (endPos - startPos) + 1) 
		    {
		        output += str[i];
		        i++;
		        t++;
		    }

		    return output;
		}

		public static bool StartsWith(string str, string target) 
		{
			if (IsNullOrEmpty(str) || IsNullOrEmpty(target) || target.Length > str.Length) { return false; }

		    int i = 0;
		    bool pass = true;
		    while (i < target.Length && pass) 
		    {
		        if (str[i] != target[i]) { pass = false; }
		        i++;
		    }

		    return pass;
		}

		public static bool EndsWith(string str, string target) 
		{
			if (IsNullOrEmpty(str) || IsNullOrEmpty(target) || target.Length > str.Length) { return false; }

		    int i = str.Length - 1;
		    int t = target.Length - 1;
		    bool pass = true;
		    while (i >= str.Length - target.Length && pass) 
		    {
		        if (str[i] != target[t]) { pass = false; }
		        i--;
		        t--;
		    }

		    return pass;
		}

		public static int IndexOf(string str, string target) 
		{
			if (IsNullOrEmpty(str) || IsNullOrEmpty(target) || target.Length > str.Length) { return -1; }

			int i = 0;
			int t = 0;
			while (i < str.Length) 
			{
				t = 0;
				while (t < target.Length) 
				{
					if (str[i + t] != target[t]) { break; }
					t++;
				}

				if (t >= target.Length) { return i; }
				i++;
			}

			return -1;
		}

		public static int IndexOf(string str, string target, int start) 
		{
			if (IsNullOrEmpty(str) || IsNullOrEmpty(target) || target.Length > str.Length || start < 0 || start >= str.Length) { return -1; }

			int i = start;
			int t = 0;
			while (i < str.Length) 
			{
				t = 0;
				while (t < target.Length) 
				{
					if (str[i + t] != target[t]) { break; }
					t++;
				}

				if (t >= target.Length) { return i; }
				i++;
			}

			return -1;
		}

		public static int LastIndexOf(string str, string target) 
		{
			if (IsNullOrEmpty(str) || IsNullOrEmpty(target) || target.Length > str.Length) { return -1; }

			int i = str.Length - target.Length;
			int t = 0;
			while (i >= 0) 
			{
				t = 0;
				while (t < target.Length) 
				{
					if (str[i + t] != target[t]) { break; }
					t++;
				}

				if (t >= target.Length) { return i; }
				i--;
			}

			return -1;
		}

		public static bool Contains(string str, string target) 
		{
			if (IsNullOrEmpty(str) || IsNullOrEmpty(target) || target.Length > str.Length) { return false; }

			int i = 0;
			int t = 0;
			while (i < str.Length) 
			{
				t = 0;
				while (t < target.Length) 
				{
					if (str[i + t] != target[t]) { break; }
					t++;
				}

				if (t >= target.Length) { return true; }
				i++;
			}

			return false;
		}

		public static int Count(string str, char target) 
		{
			if (IsNullOrEmpty(str) || target == '\x0') { return 0; }

			int count = 0;
			for (int i = 0; i < str.Length; i++) { count += (str[i] == target) ? 1 : 0; }
			return count;
		}

		public static int Count(string str, string target) 
		{
			if (IsNullOrEmpty(str) || IsNullOrEmpty(target) || target.Length > str.Length) { return 0; }

			int i = IndexOf(str, target);
			int count = 0;

			while (i >= 0) 
			{
				i = IndexOf(str, target, i + 1);
				count++;
			}

			return count;
		}

		public static string Insert(string str, string target, int index) 
		{
			if (IsNullOrEmpty(str) || IsNullOrEmpty(target) || index < 0 || index >= str.Length) { return str; }

			string output = "";
			for (int i = 0; i < str.Length; i++) 
			{
				if (i == index) { output += target; }
				output += str[i];
			}

			return output;
		}

		public static string Replace(string str, string _from, string _to) 
		{
			if (IsNullOrEmpty(str) || IsNullOrEmpty(_from) || IsNullOrEmpty(_to) || _from.Length > str.Length) { return str; }

			int i = IndexOf(str, _from);
			if (i < 0) { return  str; }

			string removed = Remove(str, i, _from.Length);
			string inserted = Insert(removed, _to, i);

			return (Contains(inserted, _from)) ? Replace(inserted, _from, _to) : inserted;
		}

		public static string Replace(string str, string _from, string _to, int count) 
		{
			if (count <= 0 || IsNullOrEmpty(str) || IsNullOrEmpty(_from) || IsNullOrEmpty(_to) || _from.Length > str.Length) { return str; }

			int i = IndexOf(str, _from);
			if (i < 0) { return  str; }

			string removed = Remove(str, i, _from.Length);
			string inserted = Insert(removed, _to, i);

			return (Contains(inserted, _from)) ? Replace(inserted, _from, _to, count - 1) : inserted;
		}

		public static string FillString(string str, char target) 
		{
			if (IsNullOrEmpty(str) || target == '\x0') { return str; }

			for (int i = 0; i < str.Length; i++) { str += target; }
			return str;
		}

		public static string FillString(char target, int count) 
		{
			if (target == '\x0' || count <= 0) { return ""; }

			string output = "";
			for (int i = 0; i < count; i++) { output += target; }
			return output;
		}
    }

    public static class Array<T> 
    {
    	public static T[] Join(params T[][] array) 
    	{
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

    	public static T[,] Replace(T[,] array, T from, T to) 
        {
            int width = array.GetLength(0);
            int height = array.GetLength(1);
            int x = 0;
            int y = 0;

            while (y < height) 
            {
                x = 0;
                while (x < width) 
                {
                    if (EqualityComparer<T>.Default.Equals(array[x, y], from)) { array[x, y] = to; }
                }

                y++;
            }

            return array;
        }

        public static T[,] Replace(T[,] array, T from, T to, int count) 
        {
            int width = array.GetLength(0);
            int height = array.GetLength(1);
            int x = 0;
            int y = 0;

            while (y < height) 
            {
                x = 0;
                while (x < width) 
                {
                    if (EqualityComparer<T>.Default.Equals(array[x, y], from)) { array[x, y] = to; count--; if (count <= 0) { return array; } }
                }

                y++;
            }

            return array;
        }

    	public static T[] Shuffle(T[] array)  
        {  
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