#if !UNITY
namespace UnityEngine 
{
    public static class Rendering 
    {
        public enum IndexFormat 
        {
            UInt16,
            UInt32
        }
    }
}
#endif

namespace Utils.Collections 
{
	#if !UNITY
	public sealed class Vector3 
	{
		public float x, y, z;

		public static readonly Vector3 zero = new Vector3(0, 0, 0);
        public static readonly Vector3 one = new Vector3(1, 1, 1);
        public static readonly Vector3 up = new Vector3(0, 1f, 0);
        public static readonly Vector3 down = new Vector3(0, -1f, 0);
        public static readonly Vector3 left = new Vector3(-1f, 0, 0);
        public static readonly Vector3 right = new Vector3(1f, 0, 0);
        public static readonly Vector3 forward = new Vector3(0, 0, 1f);
        public static readonly Vector3 back = new Vector3(0, 0, -1f);

        public static Vector3 operator * (Vector3 a, float x) => new Vector3(a.x * x, a.y * x, a.z * x);
        public static Vector3 operator * (float x, Vector3 a) => new Vector3(a.x * x, a.y * x, a.z * x);
        public static Vector3 operator / (Vector3 a, float x) => new Vector3(a.x / x, a.y / x, a.z / x);
        public static Vector3 operator / (float x, Vector3 a) => new Vector3(a.x / x, a.y / x, a.z / x);
        public static Vector3 operator + (Vector3 l, Vector3 r) => new Vector3(l.x + r.x, l.y + r.y, l.z + r.z);
        public static Vector3 operator - (Vector3 l, Vector3 r) => new Vector3(l.x - r.x, l.y - r.y, l.z - r.z);

        public Vector3 normalized { get => Utils.Math2.Normalize(this); }
        public override string ToString() => "(" + x.ToString() + ", " + y.ToString() + ", " + z.ToString() + ")";

		public Vector3(float x, float y, float z) 
		{
			this.x = x;
			this.y = y;
			this.z = z;
		}
	}

	public sealed class Vector2 
	{
		public float x, y;

		public static readonly Vector2 zero = new Vector2(0, 0);
        public static readonly Vector2 one = new Vector2(1, 1);
        public static readonly Vector2 up = new Vector2(0, 1f);
        public static readonly Vector2 down = new Vector2(0, -1f);
        public static readonly Vector2 left = new Vector2(-1f, 0);
        public static readonly Vector2 right = new Vector2(1f, 0);

        public static Vector2 operator * (Vector2 a, float x) => new Vector2(a.x * x, a.y * x);
        public static Vector2 operator * (float x, Vector2 a) => new Vector2(a.x * x, a.y * x);
        public static Vector2 operator / (Vector2 a, float x) => new Vector2(a.x / x, a.y / x);
        public static Vector2 operator / (float x, Vector2 a) => new Vector2(a.x / x, a.y / x);
        public static Vector2 operator + (Vector2 l, Vector2 r) => new Vector2(l.x + r.x, l.y + r.y);
        public static Vector2 operator - (Vector2 l, Vector2 r) => new Vector2(l.x - r.x, l.y - r.y);

        public Vector2 normalized { get => Utils.Math2.Normalize(this); }
        public override string ToString() => "(" + x.ToString() + ", " + y.ToString() + ")";

		public Vector2(float x, float y) 
		{
			this.x = x;
			this.y = y;
		}
	}

    public sealed class Color 
    {
        public float r, g, b, a;

        public Color(float r, float g, float b, float a) 
        {
            this.r = r;
            this.g = g;
            this.b = b;
            this.a = a;
        }
    }

    public class Mesh 
    {
        public Vector3[] normals { set; get; }
        public Vector3[] vertices { set; get; }
        public Vector2[] uv { set; get; }
        public int[] triangles { set; get; }
        public UnityEngine.Rendering.IndexFormat indexFormat;
        public void RecalculateNormals() {}
        public void Optimize() {}
        public Mesh() {}
    }
	#endif
}