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

	public static class Json 
	{
		public static string ToJson(object obj, bool prettyPrint = false) 
		{
			if (obj == null) { return "null"; }
			int stackc = (prettyPrint) ? CountInStack("Utils.Files.Json.ToJson") : -1;

			Type type = obj.GetType();
			if (type.IsEnum) { return type.GetFields()[0].GetValue(obj).ToString(); }
			if (IsBasic(type)) { return (type == typeof(string) || type == typeof(char)) ? "\"" + obj.ToString() + "\"" : obj.ToString().ToLower(); }

			string output = "";
			FieldInfo[] fields = type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic).Where(x => IsSerialized(x)).ToArray();
			for (int i = 0; i < fields.Length; i++) 
			{
				if (i > 0) { output += "," + ((prettyPrint) ? "\n" : ""); }
				string value = "";

				if (fields[i].FieldType.IsArray) 
				{
					string content = "";
					Array array = (Array)fields[i].GetValue(obj);
					for (int t = 0; t < array.Length; t++) { content += ToJson(array.GetValue(t), prettyPrint) + ((t < array.Length - 1) ? ("," + ((prettyPrint) ? " " : "")) : ""); }
					value = "[ " + content + " ]";
				}
				else { value = ToJson(fields[i].GetValue(obj), prettyPrint); }

				string name = fields[i].Name;
				object[] attrs = fields[i].GetCustomAttributes(typeof(JsonNameAttribute), false);
				if (attrs.Length > 0) { name = ((JsonNameAttribute)attrs[0]).name; }

				output += Indent(stackc) + "\"" + name + "\":" + ((prettyPrint) ? " " : "") + value;
			}

			return (prettyPrint) ? (((stackc > 1) ? "\n" : "") + Indent(stackc - 1) + "{\n" + output + "\n" + Indent(stackc - 1) + "}") : ("{" + output + "}");
		}

		public static T FromJsonFile<T>(string file) { return FromJson<T>(File.ReadAllText(file)); }
		public static T FromJson<T>(string json) 
		{
			int startIndex = 0;
			for (int index = json.IndexOf("//"); index >= 0; index = json.IndexOf("//", startIndex)) 
			{
				if (String2.Count(String2.GetStringAt(json, 0, index), '\"') % 2 != 0) { startIndex = index + 1; continue; }
				json = 
					((index - 1 >= 0) ? String2.GetStringAt(json, 0, index - 1) : "") + 
					String2.GetStringAt(json, json.IndexOf("\n", index), json.Length - 1)
				;
			}

			startIndex = 0;
			for (int index = json.IndexOf("/*"); index >= 0; index = json.IndexOf("/*", startIndex)) 
			{
				if (String2.Count(String2.GetStringAt(json, 0, index), '\"') % 2 != 0) { startIndex = index + 1; continue; }
				json = 
					((index - 1 >= 0) ? String2.GetStringAt(json, 0, index - 1) : "") + 
					String2.GetStringAt(json, json.IndexOf("*/", index) + 2, json.Length - 1)
				;
			}

			return (T)FillValues(typeof(T), GetPairsFromJson(json));
		}

		private static object FillValues(Type rootType, dynamic[] rootPairs) 
		{
			FieldInfo[] fields = rootType.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic).Where(x => IsSerialized(x)).ToArray();
			object obj = Activator.CreateInstance(rootType);

			for (int i = 0; i < fields.Length; i++) 
			{
				string name = fields[i].Name;
				object[] attrs = fields[i].GetCustomAttributes(typeof(JsonNameAttribute), false);
				if (attrs.Length > 0) { name = ((JsonNameAttribute)attrs[0]).name; }

				dynamic pair = null;
				try { pair = rootPairs.Where(x => x.name == name).ToArray()[0]; }
				catch { continue; }

				if (pair.type.StartsWith("object")) { fields[i].SetValue(obj, FillValues(fields[i].FieldType, pair.childPairs)); }
				else if (pair.type.StartsWith("array")) 
				{
					dynamic array = fields[i].FieldType.GetConstructor(new Type[] { typeof(int) }).Invoke(new object[] { pair.childPairs.Length });

					string tmp = array.GetType().ToString();
					tmp = tmp.Remove(tmp.Length - 2, 2);
					Type elementType = Type.GetType(tmp);

					#if UNITY
					string[] dlls = { "UnityEngine.dll", "UniytEditor.dll" };
					for (int t = 0; t < dlls.Length && elementType == null; t++) { elementType = Type.GetType(tmp + "," + dlls[t]); }
					#endif

					for (int t = 0; t < array.Length; t++) 
					{
						array[t] = (pair.type.StartsWith("array/object") || pair.type.StartsWith("array/array")) 
							? FillValues(elementType, pair.childPairs[t])
							: pair.childPairs[t].value
						;
					}

					fields[i].SetValue(obj, array);
				}
				else 
				{
					fields[i].SetValue(obj, (fields[i].FieldType.IsEnum)
						? Enum.ToObject(fields[i].FieldType, (int)pair.value)
						: ((pair.type == "null") ? null : Convert.ChangeType(pair.value, fields[i].FieldType))
					);
				}
			}

			return obj;
		}

		private static dynamic[] GetPairsFromJson(string json) 
		{
			List<Pair> pairs = new List<Pair>();
			int i = 0;

			for (Skip(ref i, json, "{"); i < json.Length; ) 
			{
				int nameEnd = json.IndexOf("\"", i) - 1;
				if (json[i - 1] != '\"') 
				{
					if (nameEnd < 0) { return pairs.ToArray(); }
					throw new FileSyntaxException("Json format exception. - '" + json[i - 1].ToString() + "' at " + (i-1).ToString());
				}

				Pair currentPair = new Pair();
				currentPair.name = String2.GetStringAt(json, i, nameEnd);

				i += currentPair.name.Length + 1;
				Skip(ref i, json, ":");
				int end = -1;

				switch (json[i - 1]) 
				{
					case '\"':
						currentPair.content = String2.GetStringAt(json, i, json.IndexOf("\"", i) - 1);
						currentPair.type = "string";
						currentPair.childPairs = null;
						currentPair.value = currentPair.content;
						pairs.Add(currentPair);

						i += currentPair.content.Length + 2;
						Skip(ref i, json, ",");
						break;
					case '[':
						string elementType;
						end = String2.EndOfScope(json, '[', ']', i - 1);
						currentPair.content = String2.GetStringAt(json, i - 1, end);
						currentPair.childPairs = GetArrayElements(currentPair.content, out elementType);
						currentPair.type = "array/" + elementType;
						pairs.Add(currentPair);

						i = end + 1;
						Skip(ref i, json, ",");
						break;
					case '{':
						end = String2.EndOfScope(json, '{', '}', i - 1);
						currentPair.content = String2.GetStringAt(json, i - 1, end);
						currentPair.type = "object";
						currentPair.childPairs = GetPairsFromJson(currentPair.content);
						pairs.Add(currentPair);

						i = end + 1;
						Skip(ref i, json, ",");
						break;
					default:
						int start = i - 1;
						Skip(ref i, json, "-+eE.1234567890nulltruefalse");
						currentPair.content = String2.GetStringAt(json, start, i - 2).TrimStart().TrimEnd();

						switch (currentPair.content) 
						{
							case "false":
							case "true":
								currentPair.type = "bool";
								currentPair.value = (currentPair.content == "true") ? true : false;
								break;
							case "nul":
							case "null":
								currentPair.type = "null";
								currentPair.value = null;
								break;
							default:
								if (currentPair.content.Contains(".") || currentPair.content.ToLower().Contains("e")) { currentPair.type = "float"; currentPair.value = float.Parse(currentPair.content); }
								else { currentPair.type = "int"; currentPair.value = int.Parse(currentPair.content); }
								break;
						}

						currentPair.childPairs = null;
						pairs.Add(currentPair);

						i--;
						Skip(ref i, json, ",");
						break;
				}

				Skip(ref i, json);
				i--;
			}

			return pairs.ToArray();
		}

		private static dynamic[] GetArrayElements(string array, out string elementType) 
		{
			if (array.Length == String2.Count(array, ' ') + 2) { elementType = "array"; return new dynamic[] {}; }

			if (array.IndexOf("{") >= 0) { elementType = "object"; }
			else if (array.IndexOf("[", 2) >= 0) { elementType = "array"; }
			else if (array.IndexOf("\"") >= 0) { elementType = "string"; }
			else if (array.Contains("true") || array.Contains("false")) { elementType = "bool"; }
			else { elementType = (array.Contains(".") || array.ToLower().Contains("e")) ? "float" : "int"; }

			List<string> objelements = new List<string>();
			List<dynamic[]> childs = new List<dynamic[]>();
			int end = 0;

			switch (elementType) 
			{
				case "bool":
				case "string":
				case "float":
				case "int":
					string[] elements = array.Split(',');
					Pair[] pairs = new Pair[elements.Length];

					if (elementType != "string") { elements = elements.Select(x => x.Replace(" ", "").Replace("\r", "").Replace("\t", "").Replace("\n", "")).ToArray(); }
					else { elements = elements.Select(x => String2.GetStringAt(x, x.IndexOf("\"") + 1, x.LastIndexOf("\"") - 1)).ToArray(); }

					for (int i = 0; i < pairs.Length; i++) 
					{
						pairs[i] = new Pair();
						pairs[i].name = null;
						pairs[i].content = elements[i];
						pairs[i].type = elementType;
						pairs[i].childPairs = null;

						switch (elementType) 
						{
							case "string":
								pairs[i].value = elements[i];
								break;
							case "bool":
								pairs[i].value = (elements[i] == "true") ? true : false;
								break;
							case "float":
								pairs[i].value = float.Parse(elements[i]);
								break;
							case "int":
								pairs[i].value = int.Parse(elements[i]);
								break;
						}
					}

					return pairs;
				case "array":
					for (int start = array.IndexOf("["); start >= 0; start = array.IndexOf("[", end)) 
					{
						end = String2.EndOfScope(array, '[', ']', start);
						objelements.Add(String2.GetStringAt(array, start, end));
					}

					for (int i = 0; i < objelements.Count; i++) 
					{
						// objelements[i] = objelements[i].TrimStart('[', ' ', '\t', '\r', '\n').TrimEnd(',', ']', ' ', '\t', '\r', '\n');
						// objelements[i] = "[" + objelements[i] + "]";
						// if (objelements[i] == "[]") { continue; }
						if (objelements[i] == "") { continue; }

						string childType;
						childs.Add(GetArrayElements(objelements[i], out childType));
						if (elementType.EndsWith("array")) { elementType += "/" + childType; }
					}

					return childs.ToArray();
				case "object":
					for (int start = array.IndexOf("{"); start >= 0; start = array.IndexOf("{", end)) 
					{
						end = String2.EndOfScope(array, '{', '}', start);
						objelements.Add(String2.GetStringAt(array, start, end));
					}

					for (int i = 0; i < objelements.Count; i++) 
					{
						// objelements[i] = objelements[i].TrimStart('{', ' ', '\t', '\r', '\n').TrimEnd(',', '}', ' ', '\t', '\r', '\n');
						// objelements[i] = "{" + objelements[i] + "}";
						// if (objelements[i] == "{}") { continue; }
						if (objelements[i] == "") { continue; }

						childs.Add(GetPairsFromJson(objelements[i]));
					}

					return childs.ToArray();
			}

			throw new FileSyntaxException("Could not create json array.");
		}

		private static int CountInStack(string method) { return String2.Count(Environment.StackTrace, method); }
		private static string Indent(int stackc) { return String2.FillString(' ', (4 * stackc >= 0) ? 4 * stackc : 0); }

		private static void Skip(ref int index, string str, string chars = "") 
		{
			char[] array = (chars + " \r\n\t").ToCharArray();
			try { while (Array.IndexOf(array, str[index++]) >= 0) { if (index >= str.Length - 1) { return; } } }
			catch {}
		}

		private static bool IsSerialized(FieldInfo field) 
		{
			if (field.GetCustomAttributes(typeof(DeserializeAttribute), false).Length > 0 || field.GetCustomAttributes(typeof(NonSerializedAttribute), false).Length > 0) { return false; }
			if (field.GetCustomAttributes(typeof(SerializeAttribute), false).Length > 0 || field.GetCustomAttributes(typeof(SerializableAttribute), false).Length > 0) { return true; }
			return field.IsPublic;
		}

		private static bool IsBasic(FieldInfo field) { return IsBasic(field.FieldType); }
		private static bool IsBasic(Type type) 
		{
			return
			type.IsPointer || 
			type == typeof(string) || 
			type == typeof(sbyte) || 
			type == typeof(byte) || 
			type == typeof(short) || 
			type == typeof(ushort) || 
			type == typeof(int) || 
			type == typeof(uint) || 
			type == typeof(long) || 
			type == typeof(ulong) || 
			type == typeof(char) || 
			type == typeof(float) || 
			type == typeof(double) || 
			type == typeof(decimal) || 
			type == typeof(bool);
		}

		private class Pair 
		{
			public string name;
			public string content;
			public string type;
			public object value;
			public dynamic[] childPairs;

			public override string ToString() { return "name: '" + name + "'\ntype: '" + type + "'\nchildCount: " + ((childPairs == null) ? 0 : childPairs.Length).ToString() + "\nvalue: " + ((value == null) ? "null" : value).ToString() + "\ncontent: '" + content + "'"; }

			public Pair() 
			{
				this.name = "";
				this.content = "";
				this.type = "root";
				this.value = null;
				this.childPairs = null;
			}
		}
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