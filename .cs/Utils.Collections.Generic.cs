namespace Utils.Collections.Generic 
{
	public sealed class Vector4<T> 
    {
        public T x;
        public T y;
        public T z;
        public T w;

        public override string ToString() { return "(" + x.ToString() + ", " + y.ToString() + ", " + z.ToString() + ", " + w.ToString() + ")"; }

        public Vector4() {}
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
        public T x;
        public T y;
        public T z;

        public override string ToString() { return "(" + x.ToString() + ", " + y.ToString() + ", " + z.ToString() + ")"; }

        public Vector3() {}
        public Vector3(T x, T y, T z) 
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }
    }

    public sealed class Vector2<T> 
    {
        public T x;
        public T y;

        public override string ToString() { return "(" + x.ToString() + ", " + y.ToString() + ")"; }

        public Vector2() {}
        public Vector2(T x, T y) 
        {
            this.x = x;
            this.y = y;
        }
    }

    public sealed class Range<T> 
    {
        public T min;
        public T max;

        public override string ToString() { return "(" + min.ToString() + ", " + max.ToString() + ")"; }

        public Range() {}
        public Range(T min, T max) 
        {
            this.min = min;
            this.max = max;
        }
    }
}