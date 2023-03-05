namespace Utils.Collections.Generic 
{
	public sealed class Vector4<T> 
	{
		public T x, y, z, w;
		public override string ToString() => "(" + x.ToString() + ", " + y.ToString() + ", " + z.ToString() + ", " + w.ToString() + ")";
		public Vector4(T x, T y, T z, T w) 
		{
			this.x = x;
			this.y = y;
			this.z = z;
			this.w = w;
		}
	}

	public sealed class Vector3<T> 
	{
		public T x, y, z;
		public override string ToString() => "(" + x.ToString() + ", " + y.ToString() + ", " + z.ToString() + ")";
		public Vector3(T x, T y, T z) 
		{
			this.x = x;
			this.y = y;
			this.z = z;
		}
	}

	public sealed class Vector2<T> 
	{
		public T x, y;
		public override string ToString() => "(" + x.ToString() + ", " + y.ToString() + ")";
		public Vector2(T x, T y) 
		{
			this.x = x;
			this.y = y;
		}
	}

	public sealed class Range<T> 
    {
        public T min, max;
        public override string ToString() { return "(" + min.ToString() + ", " + max.ToString() + ")"; }
        public Range(T min, T max) 
        {
            this.min = min;
            this.max = max;
        }
    }
}