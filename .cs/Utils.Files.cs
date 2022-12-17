using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Reflection;
using System.Linq;
using System.IO;
using System.Text;

using Utils.Compression;

#if UNITY
using UnityEngine;
#else
using Utils.Collections;
#endif

namespace Utils.Files 
{
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Field)]
	public sealed class SerializeAttribute : Attribute { public SerializeAttribute() {} }

	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Field)]
	public sealed class DeserializeAttribute : Attribute { public DeserializeAttribute() {} }

	public sealed class JsonNameAttribute : Attribute 
	{
		public string name;
		public JsonNameAttribute(string name) { this.name = name; }
	}

	public sealed class FileSyntaxException : Exception 
	{
		public FileSyntaxException() : base() {}
		public FileSyntaxException(string message) : base(message) {}
		public FileSyntaxException(string message, Exception inner) : base(message, inner) {}
	}

	public sealed class PNG : Utils.Graphics.Drawable
	{
		public static readonly byte[] Signature = { 0x89, 0x50, 0x4e, 0x47, 0x0d, 0x0a, 0x1a, 0x0a };

		public int Width { get { return width; } }
		public int Height { get { return height; } }
		public byte Depth { get { return depth; } }
		public byte ColorType { get { return colorType; } }
		public byte Compression { get { return compression; } }
		public byte Filter { get { return filter; } }
		public bool Interlaced { get { return interlaced; } }

		[Serialize] private int width;                // (4 bytes)
		[Serialize] private int height;               // (4 bytes)
		[Serialize] private byte depth = 8;           // (1 byte, values 1, 2, 4, 8, or 16)
		[Serialize] private byte colorType = 6;       // (1 byte, values 0, 2, 3, 4, or 6)
		[Serialize] private byte compression = 0; // (1 byte, value 0)
		[Serialize] private byte filter = 0;          // (1 byte, value 0)
		[Serialize] private bool interlaced = false;  // (1 byte, values 0 "no interlace" or 1 "Adam7 interlace") (13 data bytes total)

		/*

		89 50 4e 47 0d 0a 1a 0a

		CRC of chunk's type and data (but not length)

		IHDR must be the first chunk, it contains (in this order) the image's
		width (4 bytes)
		height (4 bytes)
		bit depth (1 byte, values 1, 2, 4, 8, or 16)
		color type (1 byte, values 0, 2, 3, 4, or 6)
		compression method (1 byte, value 0)
		filter method (1 byte, value 0)
		interlace method (1 byte, values 0 "no interlace" or 1 "Adam7 interlace") (13 data bytes total)


		Length	 Chunk type   	 Chunk data	      CRC
		4 bytes	 4 bytes	     $Length bytes	  4 bytes



		ColorType:
		0 grayscale
		2 red, green and blue: rgb/truecolor
		3 indexed: channel containing indices into a palette of colors
		4 grayscale and alpha: level of opacity for each pixel
		6 red, green, blue and alpha

		*/

		public Color this[int x, int y] 
		{
			set { SetPixel(x, y, value); }
			get { return GetPixel(x, y); }
		}

		public void Save(string file) { File.WriteAllBytes(file, Encode()); }

		public PNG(int width, int height) 
		{
			this.width = width;
			this.height = height;
			this.colors = new Color[this.width * this.height];
			for (int i = 0; i < this.colors.Length; i++) { this.colors[i] = new Color(0f, 0f, 0f, 0f); }

			base.w = this.width;
			base.h = this.height;
		}

		public PNG(int width, int height, Color color) 
		{
			this.width = width;
			this.height = height;
			this.colors = new Color[this.width * this.height];
			for (int i = 0; i < this.colors.Length; i++) { this.colors[i] = color; }

			base.w = this.width;
			base.h = this.height;
		}

		public PNG(string file) 
		{
			List<Chunk> chunks = new List<Chunk>();
			byte[] bytes = File.ReadAllBytes(file);

			int i = PNG.Signature.Length;
			while (i < bytes.Length) 
			{
				chunks.Add(new Chunk(bytes, i));
				i += 4 + 4 + chunks[chunks.Count - 1].Length + 4;
			}

			this.width = (chunks[0].Data[0] << 24) + (chunks[0].Data[1] << 16) + (chunks[0].Data[2] << 8) + (chunks[0].Data[3] << 0);
			this.height = (chunks[0].Data[4] << 24) + (chunks[0].Data[5] << 16) + (chunks[0].Data[6] << 8) + (chunks[0].Data[7] << 0);
			this.depth = chunks[0].Data[8];
			this.colorType = chunks[0].Data[9];
			this.compression = chunks[0].Data[10];
			this.filter = chunks[0].Data[11];
			this.interlaced = chunks[0].Data[12] == 1;
			this.colors = new Color[this.width * this.height];

			base.w = this.width;
			base.h = this.height;

			// byte[] data = chunks.Where(x => x.Name == "IDAT").ToArray()[0].Data;
			// data = Deflate.Decompress(data);

			// FILL COLORS ARRAY FROM DATA[]
			for (i = 0; i < this.colors.Length; i++) { this.colors[i] = new Color(0f, 0.5f, 0.5f, 1f); }
		}

		private byte[] Encode() 
		{
			if (colorType != 6 || depth != 8) { throw new NotImplementedException(); }

			byte[] length, type, data;
			List<byte> bytes = new List<byte>();
			bytes.AddRange(PNG.Signature);


			/// <IHDR>
			type = new byte[] { 0x49, 0x48, 0x44, 0x52 };
			data = Array2.Join<byte>(
				Tools.GetBytes(sizeof(int), width, height),
				new byte[] { depth, (byte)colorType, compression, filter, (byte)(interlaced ? 1 : 0) }
			);
			length = Tools.GetBytes(sizeof(int), data.Length);
			bytes.AddRange(Array2.Join<byte>(length, type, data));
			bytes.AddRange(Tools.GetBytes(sizeof(uint), CRC.CRC32(Array2.Join<byte>(type, data))));
			/// </IHDR>


			/// <IDAT>
			type = new byte[] { 0x49, 0x44, 0x41, 0x54 };
			data = Array2.Join<byte>(colors.Select(x => new byte[] // NOT WORKING
				{
					(byte)(x.r * 255f),
					(byte)(x.g * 255f),
					(byte)(x.b * 255f),
					(byte)(x.a * 255f)
				}
			).ToArray());
			length = Tools.GetBytes(sizeof(int), data.Length);
			data = Deflate.Compress(data);
			bytes.AddRange(Array2.Join<byte>(length, type, data));
			bytes.AddRange(Tools.GetBytes(sizeof(uint), CRC.CRC32(Array2.Join<byte>(type, data))));
			/// </IDAT>


			/// <IEND>
			type = new byte[] { 0x49, 0x45, 0x4e, 0x44 };
			length = Tools.GetBytes(sizeof(int), 0);
			bytes.AddRange(Array2.Join<byte>(length, type));
			bytes.AddRange(new byte[] { 0xae, 0x42, 0x60, 0x82 });
			/// </IEND>

			return bytes.ToArray();
		}

		public sealed class Chunk 
		{
			public int Length;
			public byte[] Type;
			public byte[] Data;
			public byte[] Crc;

			public string Name { get { return Encoding.ASCII.GetString(Type); } }

			public override string ToString() 
			{
				string output = "";

				output += "Chunk:\n";
				output += "  Length: " + Length.ToString() + "\n";
				output += "  Type: " + Name + "\n";

				output += "  Data: { ";
				for (int i = 0; i < Length; i++) { string c = Format.UIntToHex(Data[i], 2); output += ((c.Length == 1) ? "0" + c : c) + ((i >= Length - 1) ? " " : ", "); }
				output += "}\n";

				output += "  Crc: { ";
				for (int i = 0; i < Crc.Length; i++) { string c = Format.UIntToHex(Crc[i], 2); output += ((c.Length == 1) ? "0" + c : c) + ((i >= 4 - 1) ? " " : ", "); }
				output += "}\n";

				return output;
			}

			public Chunk(byte[] content, int index) 
			{
				this.Length = (content[index] << 24) + (content[index + 1] << 16) + (content[index + 2] << 8) + (content[index + 3] << 0);
				this.Type = Array2.GetArrayAt<byte>(content, index + 4, index + 7);
				this.Data = (this.Length > 0) ? Array2.GetArrayAt<byte>(content, index + 8, index + 8 + this.Length - 1) : new[] { (byte)0x00 };
				this.Crc = Array2.GetArrayAt<byte>(content, index + ((this.Length > 0) ? 9 : 8) + this.Length, index + ((this.Length > 0) ? 9 : 8) + this.Length + 3);
			}
		}
	}
}