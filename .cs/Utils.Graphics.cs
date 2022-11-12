using System;
using System.Linq;

#if UNITY
using UnityEngine;
#else
using Utils.Collections;
#endif

namespace Utils.Graphics 
{
	// NOTHING IS TESTED !!!
	public abstract class Drawable 
	{
		[Utils.Files.Serialize] protected Color[] colors;
		protected int w; // width
		protected int h; // height

		public void SetPixel(int x, int y, Color color) 
		{
			if (x < 0 || x > w) { throw new ArgumentOutOfRangeException("x"); }
			if (y < 0 || y > h) { throw new ArgumentOutOfRangeException("y"); }

			colors[(y * w) + x] = color;
		}

		public Color GetPixel(int x, int y) 
		{
			if (x < 0 || x > w) { throw new ArgumentOutOfRangeException("x"); }
			if (y < 0 || y > h) { throw new ArgumentOutOfRangeException("y"); }

			return colors[(y * w) + x];
		}

		public void Fill(Color color) { colors = colors.Select(x => color).ToArray(); }

		public void FlipHorizontal() 
		{
			for (int y = 0; y < h / 2; y++) 
			{
				for (int x = 0; x < w; x++) 
				{
					Color tmp = GetPixel(x, y);
					SetPixel(x, y, GetPixel(x, h - y - 1));
					SetPixel(x, h - y - 1, tmp);
				}
			}
		}

		public void FlipVertical() 
		{
			for (int y = 0; y < h; y++) 
			{
				for (int x = 0; x < w / 2; x++) 
				{
					Color tmp = GetPixel(x, y);
					SetPixel(x, y, GetPixel(w - x - 1, y));
					SetPixel(w - x - 1, y, tmp);
				}
			}
		}
	}
}