#if UNITY
using UnityEngine;
#endif

using System;
using System.Collections.Generic;

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
        public float x { set; get; }
        public float y { set; get; }
        public float z { set; get; }

        public static readonly Vector3 zero = new Vector3(0, 0, 0);
        public static readonly Vector3 one = new Vector3(1, 1, 1);
        public static readonly Vector3 back = new Vector3(0, 0, -1f);
        public static readonly Vector3 down = new Vector3(0, -1f, 0);
        public static readonly Vector3 forward = new Vector3(0, 0, 1f);
        public static readonly Vector3 left = new Vector3(-1f, 0, 0);
        public static readonly Vector3 right = new Vector3(1f, 0, 0);
        public static readonly Vector3 up = new Vector3(0, 1f, 0);

        public static Vector3 operator * (Vector3 a, float x) { return new Vector3(a.x * x, a.y * x, a.z * x); }
        public static Vector3 operator * (float x, Vector3 a) { return new Vector3(a.x * x, a.y * x, a.z * x); }

        public static Vector3 operator / (Vector3 a, float x)
        {
            if (x == 0f) { throw new DivideByZeroException(); }
            return new Vector3(a.x / x, a.y / x, a.z / x);
        }

        public static Vector3 operator / (float x, Vector3 a)
        {
            if (x == 0f) { throw new DivideByZeroException(); }
            return new Vector3(a.x / x, a.y / x, a.z / x);
        }

        public static Vector3 operator + (Vector3 a, Vector3 b) { return new Vector3(a.x + b.x, a.y + b.y, a.z + b.z); }
        public static Vector3 operator - (Vector3 a, Vector3 b) { return new Vector3(a.x - b.x, a.y - b.y, a.z - b.z); }

        public Vector3(float x, float y, float z) 
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }
    }

    public sealed class Vector2 
    {
        public float x { set; get; }
        public float y { set; get; }

        public static readonly Vector2 zero = new Vector2(0, 0);
        public static readonly Vector2 one = new Vector2(1, 1);
        public static readonly Vector2 down = new Vector2(0, -1f);
        public static readonly Vector2 left = new Vector2(-1f, 0);
        public static readonly Vector2 right = new Vector2(1f, 0);
        public static readonly Vector2 up = new Vector2(0, 1f);

        public static Vector2 operator * (Vector2 a, float x) { return new Vector2(a.x * x, a.y * x); }
        public static Vector2 operator * (float x, Vector2 a) { return new Vector2(a.x * x, a.y * x); }

        public static Vector2 operator / (Vector2 a, float x)
        {
            if (x == 0f) { throw new DivideByZeroException(); }
            return new Vector2(a.x / x, a.y / x);
        }

        public static Vector2 operator / (float x, Vector2 a)
        {
            if (x == 0f) { throw new DivideByZeroException(); }
            return new Vector2(a.x / x, a.y / x);
        }

        public static Vector2 operator + (Vector2 a, Vector2 b) { return new Vector2(a.x + b.x, a.y + b.y); }
        public static Vector2 operator - (Vector2 a, Vector2 b) { return new Vector2(a.x - b.x, a.y - b.y); }

        public Vector2(float x, float y) 
        {
            this.x = x;
            this.y = y;
        }
    }
    #endif

    public class ObjectMesh 
	{
		public int vertexCount { get { return vertices.Count; } }
		public Mesh mesh 
		{
			get 
			{
				Mesh mesh = new Mesh();
				mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
				mesh.vertices = vertices.ToArray();
				mesh.triangles = triangles.ToArray();
				mesh.uv = uvs.ToArray();
				mesh.RecalculateNormals();
				mesh.Optimize();
				return mesh;
			}
		}

		public List<Vector3> vertices;
		public List<int> triangles;
		public List<Vector2> uvs;

		public virtual void Clear() 
		{
			vertices = new List<Vector3>();
			triangles = new List<int>();
			uvs = new List<Vector2>();
		}

		public void Add(params Vector3[] x) { for (int i = 0; i < x.Length; i++) { vertices.Add(x[i]); } }
		public void Add(params int[] x) { for (int i = 0; i < x.Length; i++) { triangles.Add(x[i]); } }
		public void Add(params Vector2[] x) { for (int i = 0; i < x.Length; i++) { uvs.Add(x[i]); } }
		public void Add(params Mesh[] x) 
		{
			for (int i = 0; i < x.Length; i++) 
			{
				Add(x[i].vertices);
				Add(x[i].triangles);
				Add(x[i].uv);
			}
		}

		public ObjectMesh() 
		{
			this.vertices = new List<Vector3>();
			this.triangles = new List<int>();
			this.uvs = new List<Vector2>();
		}
	}

	#if !UNITY
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