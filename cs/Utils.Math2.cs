using System;
using Utils.Collections;

#if UNITY
using UnityEngine;
#endif

namespace Utils 
{
	public static class Math2 
	{
		public const float PI = 3.1415926535897932384626433832795f;
		public const float TAU = 6.283185307179586476925286766559f;
		public const float E = 2.7182818284590452353602874713527f;
		public const float DEG2RAD = 0.01745329251994329576923690768489f;
        public const float RAD2DEG = 57.295779513082320876798154814105f;
        public const float GoldenRatio = 1.6180339887498948482045868343657f;

		public static float Fact(float x) { float result = 1; for (int i = (int)x; i > 1; i--) { result *= i; } return result; }
		public static float Sqrt(float x) => (float)Math.Sqrt((double)x);
		public static float Abs(float x) => (float)Math.Abs((double)x);
		public static float Sin(float x) => (float)Math.Sin((double)x);
		public static float Cos(float x) => (float)Math.Cos((double)x);
		public static float Tan(float x) => (float)Math.Tan((double)x);
		public static float Asin(float x) => (float)Math.Asin((double)x);
		public static float Acos(float x) => (float)Math.Acos((double)x);
		public static float Atan(float x) => (float)Math.Atan((double)x);
		public static float Floor(float x) => (float)Math.Floor((double)x);
		public static float Ceil(float x) => (float)Math.Ceiling((double)x);
		public static float Ceiling(float x) => (float)Math.Ceiling((double)x);
		public static float Clamp(float x, float a, float b) => Math2.Max(a, Math2.Min(b, x));
		public static float Log(float x, float a) => (float)Math.Log((double)x, (double)a);
		public static float Log10(float x) => (float)Math.Log10((double)x);
		public static float Ln(float x) => (float)Math.Log((double)x, Math.E);
		public static float Exp(float x) => (float)Math.Pow(Math.E, (double)x);
		public static float Min(float a, float b) => (float)Math.Min((double)a, (double)b);
		public static float Max(float a, float b) => (float)Math.Max((double)a, (double)b);
		public static float Pow(float x, float y) => (float)Math.Pow((double)x, (double)y);
		public static float Round(float x) => (float)Math.Round((double)x);
		public static float Round(float x, float a) => (float)Math.Round((double)x, (int)a);
		public static float Atan2(float y, float x) => (float)Math.Atan2((double)y, (double)x);

		public static float Lerp(float a, float b, float x) => a + (x * (b - a));
		public static Vector2 Lerp(Vector2 a, Vector2 b, float x) => new Vector2(Math2.Lerp(a.x, b.x, x), Math2.Lerp(a.y, b.y, x));
		public static Vector3 Lerp(Vector3 a, Vector3 b, float x) => new Vector3(Math2.Lerp(a.x, b.x, x), Math2.Lerp(a.y, b.y, x), Math2.Lerp(a.z, b.z, x));

		public static float InverseLerp(float a, float b, float x) => (x - a) / (b - a);
		public static Vector2 InverseLerp(Vector2 a, Vector2 b, float x) => new Vector2(Math2.InverseLerp(a.x, b.x, x), Math2.InverseLerp(a.y, b.y, x));
		public static Vector3 InverseLerp(Vector3 a, Vector3 b, float x) => new Vector3(Math2.InverseLerp(a.x, b.x, x), Math2.InverseLerp(a.y, b.y, x), Math2.InverseLerp(a.z, b.z, x));

		public static float Length(Vector2 x) => Math2.Sqrt((x.x*x.x) + (x.y*x.y));
		public static float Length(Vector3 x) => Math2.Sqrt((x.x*x.x) + (x.y*x.y) + (x.z*x.z));

		public static float Distance(Vector2 a, Vector2 b) => Math2.Sqrt(((b.x - a.x)*(b.x - a.x)) + ((b.y - a.y)*(b.y - a.y)));
		public static float Distance(Vector3 a, Vector3 b) => Math2.Sqrt(((b.x - a.x)*(b.x - a.x)) + ((b.y - a.y)*(b.y - a.y)) + ((b.z - a.z)*(b.z - a.z)));

		public static Vector2 Normalize(Vector2 x) => x / Math2.Length(x);
		public static Vector3 Normalize(Vector3 x) => x / Math2.Length(x);

		public static float Dot(Vector2 a, Vector2 b) => (a.x*b.x) + (a.y*b.y);
		public static float Dot(Vector3 a, Vector3 b) => (a.x*b.x) + (a.y*b.y) + (a.z*b.z);

		public static Vector2 Rotate(Vector2 origin, Vector2 point, float angle) 
		{
			float x = origin.x + ((point.x - origin.x) * Math2.Cos(angle)) + ((point.y - origin.y) * Math2.Sin(angle));
            float y = origin.y + ((point.x - origin.x) * Math2.Sin(angle)) + ((point.y - origin.y) * Math2.Cos(angle));
            return new Vector2(x, y);
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
}