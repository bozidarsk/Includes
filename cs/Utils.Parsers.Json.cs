using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Reflection;
using System.Linq;

namespace Utils.Parsers 
{
	public static class Json 
	{
		[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
		public sealed class SerializeAttribute : Attribute { public SerializeAttribute() {} }

		[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
		public sealed class DeserializeAttribute : Attribute { public DeserializeAttribute() {} }

		[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
		public sealed class NameAttribute : Attribute 
		{
			public string Name;
			public NameAttribute(string name) { this.Name = name; }
		}

		public static string ToJson(object obj, bool prettyPrint = false) => ToJson(obj, prettyPrint, "").Remove(0, 1);
		private static string ToJson(object obj, bool prettyPrint, string tab) 
		{
			if (obj == null) { return "null"; }

			Type type = obj.GetType();
			string output = "";

			if (IsBasic(type)) { return obj.ToString().ToLower(); }
			if (type.IsEnum) { return type.GetFields()[0].GetValue(obj).ToString(); }
			if (type == typeof(string)) { return "\"" + obj.ToString() + "\""; }
			if (type == typeof(char)) { return "'" + obj.ToString() + "'"; }
			if (type.IsArray) 
			{
				Array array = (Array)obj;
				if (array.GetLength(0) == 0 && array.Rank == 1) { return "[]"; }
				return ArrayToString(array, new int[array.Rank], 1, prettyPrint, (prettyPrint) ? (tab + "\t") : "");
			}

			List<FieldInfo> fields = type.GetFields(BindingFlags.Public | BindingFlags.GetField | BindingFlags.Instance).ToList();
			fields.AddRange(type.GetFields(BindingFlags.NonPublic | BindingFlags.GetField | BindingFlags.Instance).Where(x => HasAttribute<Json.SerializeAttribute>(x, out Json.SerializeAttribute s)).ToArray());

			for (int i = 0; i < fields.Count; i++) 
			{
				if (HasAttribute<Json.DeserializeAttribute>(fields[i], out Json.DeserializeAttribute d)) { continue; }

				string name = fields[i].Name;
				if (HasAttribute<Json.NameAttribute>(fields[i], out Json.NameAttribute n)) { name = n.Name; }

				output += ((prettyPrint) ? ("\t" + tab) : "") + "\"" + name + "\": " + ToJson(fields[i].GetValue(obj), prettyPrint, (prettyPrint) ? (tab + "\t") : "");
				if (i < fields.Count - 1) { output += "," + ((prettyPrint) ? "\n" : ""); }
			}

			return (prettyPrint) ? ("\n" + tab + "{\n" + output + "\n" + tab + "}") : ("{" + output + "}");
		}

		public static T FromJsonFile<T>(string file) => FromJson<T>(System.IO.File.ReadAllText(file));
		public static T FromJson<T>(string json) 
		{
			TokenDefinition[] definitions = 
			{
				new TokenDefinition("\"(\\w|[!@#$%^&*()\\-_=+/|`~\\[\\]{},<.>/?;:]|\\[\"nrtx])*\"", "STRING"),
				new TokenDefinition(":", "COLON", ":"),
				new TokenDefinition(",", "COMMA", ","),
				new TokenDefinition("\\{", "LCB", "{"),
				new TokenDefinition("\\}", "RCB", "}"),
				new TokenDefinition("\\[", "LSB", "["),
				new TokenDefinition("\\]", "RSB", "]"),
				new TokenDefinition("true|false", "BOOL"),
				new TokenDefinition("null", "NULL", "null"),
				new TokenDefinition("-?[0-9]*(\\.[0-9]+)?([eE][+-][0-9]+)?", "NUMBER"),
			};

			json = Regex.Replace(json, "\\/\\/.*\\r?\\n", "");
			json = Regex.Replace(json, "\\/\\*(.|\\s)*\\*\\/", "");

			List<Token> tokens = Lexer.Lex(json, definitions);
			return (T)FillValues(typeof(T), GetFirstPair(tokens));
		}

		private static object FillValues(Type type, Pair root) 
		{
			object obj = Activator.CreateInstance(type);
			List<FieldInfo> fields = type.GetFields(BindingFlags.Public | BindingFlags.GetField | BindingFlags.Instance).ToList();
			fields.AddRange(type.GetFields(BindingFlags.NonPublic | BindingFlags.GetField | BindingFlags.Instance).Where(x => HasAttribute<Json.SerializeAttribute>(x, out Json.SerializeAttribute s)).ToArray());

			for (int i = 0; i < fields.Count; i++) 
			{
				if (HasAttribute<Json.DeserializeAttribute>(fields[i], out Json.DeserializeAttribute d)) { continue; }

				string name = fields[i].Name;
				if (HasAttribute<Json.NameAttribute>(fields[i], out Json.NameAttribute n)) { name = n.Name; }

				Pair pair = null;
				try { pair = root.Childs.Where(x => x.Name == name).ToArray()[0]; }
				catch { continue; }

				fields[i].SetValue(obj, GetValue(fields[i].FieldType, pair));
			}

			return obj;
		}

		private static object GetValue(Type type, Pair pair) 
		{
			switch (pair.Type) 
			{
				case "basic":
					return (type.IsEnum)
						? Enum.ToObject(type, (int)pair.Value)
						: System.Convert.ChangeType(pair.Value, type)
					;
				case "null":
					return null;
				case "object":
					return FillValues(type, pair);
				case "array":
					string strtype = type.ToString();
					if (strtype.Contains(",")) 
					{
						string from = ",";
						string to = "[][]";
						while (strtype.Contains(",")) 
						{
							strtype = strtype.Replace("[" + from + "]", to);
							from += ",";
							to += "[]";
						}
					}

					strtype = strtype.Remove(strtype.Length - 2, 2);
					Type elementType = Type.GetType(strtype);

					Array array = Array.CreateInstance(elementType, pair.Childs.Length);
					for (int t = 0; t < array.GetLength(0); t++) { array.SetValue(GetValue(elementType, pair.Childs[t]), t); }

					return array;
				default:
					throw new ArgumentException("Unhandled pair type: '" + pair.Type + "'");
			}
		}

		private static Pair GetFirstPair(List<Token> tokens) 
		{
			List<Pair> childs = new List<Pair>();
			int start = tokens.FindIndex(x => x.Name == "LCB");
			int end = EndOfScope(tokens, "LCB", "RCB", start);

			for (int i = start + 1; i < end; i++) 
			{
				if (tokens[i].Name == "COMMA") { continue; }

				int end2;
				string name = tokens[i].Value.Substring(1, tokens[i].Value.Length - 2);
				switch (tokens[i + 2].Name) 
				{
					case "STRING":
						childs.Add(new Pair(name, "basic", tokens[i + 2].Value.Substring(1, tokens[i + 2].Value.Length - 2)));
						i += 2;
						continue;
					case "NUMBER":
						childs.Add(new Pair(name, "basic", double.Parse(tokens[i + 2].Value)));
						i += 2;
						continue;
					case "BOOl":
						childs.Add(new Pair(name, "basic", bool.Parse(tokens[i + 2].Value)));
						i += 2;
						continue;
					case "NULL":
						childs.Add(new Pair(name, "null", null));
						i += 2;
						continue;
					case "LCB":
						end2 = EndOfScope(tokens, "LCB", "RCB", i + 2);
						childs.Add(new Pair(name, "object", GetFirstPair(tokens.GetRange(i + 2, (end2 - (i + 2)) + 1)).Childs));
						i += 1 + (end2 - (i + 2)) + 1;
						continue;
					case "LSB":
						end2 = EndOfScope(tokens, "LSB", "RSB", i + 2);
						childs.Add(new Pair(name, "array", TokensToArray(tokens.GetRange(i + 2, (end2 - (i + 2)) + 1))));
						i += 1 + (end2 - (i + 2)) + 1;
						continue;
				}
			}

			return new Pair(null, "object", childs.ToArray());
		}

		private static Pair[] TokensToArray(List<Token> tokens) 
		{
			List<Pair> childs = new List<Pair>();
			int start = 0;
			int end = EndOfScope(tokens, "LSB", "RSB", start);

			for (int i = start + 1; i < end; i++) 
			{
				int end2;
				switch (tokens[i].Name) 
				{
					case "COMMA":
						continue;
					case "STRING":
						childs.Add(new Pair(null, "basic", tokens[i].Value.Substring(1, tokens[i].Value.Length - 2)));
						i++;
						continue;
					case "NUMBER":
						childs.Add(new Pair(null, "basic", double.Parse(tokens[i].Value)));
						i++;
						continue;
					case "BOOL":
						childs.Add(new Pair(null, "basic", bool.Parse(tokens[i].Value)));
						i++;
						continue;
					case "NULL":
						childs.Add(new Pair(null, "null", null));
						i++;
						continue;
					case "LCB":
						end2 = EndOfScope(tokens, "LCB", "RCB", i);
						childs.Add(new Pair(null, "object", GetFirstPair(tokens.GetRange(i, (end2 - (i)) + 1)).Childs));
						i += (end2 - (i)) + 1;
						continue;
					case "LSB":
						end2 = EndOfScope(tokens, "LSB", "RSB", i);
						childs.Add(new Pair(null, "array", TokensToArray(tokens.GetRange(i, (end2 - (i)) + 1))));
						i += (end2 - (i)) + 1;
						continue;
				}
			}

			return childs.ToArray();
		}

		private static string ArrayToString(Array array, int[] coords, int rank, bool prettyPrint, string tab) 
		{
			string output = "";
			string space = (prettyPrint) ? " " : "";

			output += "[" + space;
			for (coords[rank - 1] = 0; coords[rank - 1] < array.GetLength(rank - 1); coords[rank - 1]++) 
			{
				output += (rank + 1 > array.Rank) 
					? ToJson(array.GetValue(coords), prettyPrint, tab)
					: ArrayToString(array, coords, rank + 1, prettyPrint, tab)
				;

				if (coords[rank - 1] < array.GetLength(rank - 1) - 1) { output += "," + space; }
			}
			output += space + "]";

			return output;
		}

		private static bool IsBasic(Type type) 
		{
			return
				type.IsPointer | 
				type == typeof(bool) | 
				type == typeof(double) | 
				type == typeof(decimal) | 
				type == typeof(float) | 
				type == typeof(uint) | 
				type == typeof(int) | 
				type == typeof(ushort) | 
				type == typeof(short) | 
				type == typeof(byte) | 
				type == typeof(sbyte)
			;
		}

		private static int EndOfScope(List<Token> tokens, string lName, string rName, int start) 
		{
			int end = -1;

			int bracketCount = 1;
			for (int i = start + 1; bracketCount > 0; i++) 
			{
				if (tokens[i].Name == lName) { bracketCount++; }
				if (tokens[i].Name == rName) { bracketCount--; }
				end = i;
			}

			return end;
		}

		private static bool HasAttribute<T>(MemberInfo info, out T attribute) where T : Attribute
		{
			object[] objs = info.GetCustomAttributes(typeof(T), false);
			if (objs.Length == 0) { attribute = null; return false; }
			attribute = (T)objs[0];
			return true;
		}

		public class Pair 
		{
			public string Name;
			public string Type; // basic|null|array|object
			public object Value;
			public Pair[] Childs;

			public Pair() {}

			public Pair(string name, string type, object value) 
			{
				this.Name = name;
				this.Type = type;
				this.Value = value;
				this.Childs = null;
			}

			public Pair(string name, string type, Pair[] childs) 
			{
				this.Name = name;
				this.Type = type;
				this.Value = null;
				this.Childs = childs;
			}
		}
	}
}