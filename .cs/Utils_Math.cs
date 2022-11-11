#if UNITY
using UnityEngine;
#else
using Utils.Collections;
#endif

namespace Utils 
{
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
}