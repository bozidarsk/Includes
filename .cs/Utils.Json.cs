using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;

namespace Utils.Json 
{
	public static class Json 
	{
		public static string ToJson(object obj, bool prettyPrint = false) 
		{
			int stackc = (prettyPrint) ? CountInStack("Utils.Json.Json.ToJson") : -1;

			Type type = obj.GetType();
			if (type.IsEnum) { return type.GetFields()[0].GetValue(obj).ToString(); }
			if (IsBasic(type)) { return (type == typeof(string) || type == typeof(char)) ? "\"" + obj.ToString() + "\"" : obj.ToString(); }

			string output = "";
			FieldInfo[] fields = obj.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic).Where(x => IsSerialized(x)).ToArray();
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

				output += indent + "\"" + fields[i].Name + "\":" + ((prettyPrint) ? " " : "") + value;
			}

			return (prettyPrint) ? (((stackc > 1) ? "\n" : "") + Indent(stackc - 1) + "{\n" + output + "\n" + Indent(stackc - 1) + "}") : ("{" + output + "}");
		}

		public static T FromJson<T>(string json) 
		{
			int startIndex = 0;
			for (int index = json.IndexOf("//"); index >= 0; index = json.IndexOf("//", startIndex)) 
			{
				if (String2.Count(String2.GetStringAt(json, 0, index), '\"') % 2 != 0) { startIndex = index + 1; continue; }
				json = String2.GetStringAt(json, 0, index - 1) + String2.GetStringAt(json, json.IndexOf("\n", index), json.Length - 1);
			}

			startIndex = 0;
			for (int index = json.IndexOf("/*"); index >= 0; index = json.IndexOf("/*", startIndex)) 
			{
				if (String2.Count(String2.GetStringAt(json, 0, index), '\"') % 2 != 0) { startIndex = index + 1; continue; }
				json = String2.GetStringAt(json, 0, index - 1) + String2.GetStringAt(json, json.IndexOf("*/", index) + 2, json.Length - 1);
			}

			return (T)FillValues(typeof(T), GetPairsFromJson(json));
		}

		private static object FillValues(Type rootType, dynamic[] rootPairs) 
		{
			FieldInfo[] fields = rootType.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic).Where(x => IsSerialized(x)).ToArray();
			dynamic obj = rootType.GetConstructor(new Type[] {}).Invoke(null);

			for (int i = 0; i < fields.Length; i++) 
			{
				dynamic pair = null;
				try { pair = rootPairs.Where(x => x.name == fields[i].Name).ToArray()[0]; }
				catch { continue; }

				if (((Pair)pair).type.StartsWith("object")) { fields[i].SetValue(obj, FillValues(fields[i].FieldType, pair.childPairs)); }
				else if (((Pair)pair).type.StartsWith("array")) 
				{
					dynamic[] values = ((Pair)pair).childPairs.Select(x => x.value).ToArray();

					dynamic array = fields[i].FieldType.GetConstructor(new Type[] { typeof(int) }).Invoke(new object[] { pair.childPairs.Length });
					for (int t = 0; t < array.Length; t++) { array[t] = pair.childPairs[t].value; }
					fields[i].SetValue(obj, array);
				}
				else 
				{
					fields[i].SetValue(obj, (fields[i].FieldType.IsEnum)
						? Enum.ToObject(fields[i].FieldType, (int)pair.value)
						: Convert.ChangeType(pair.value, fields[i].FieldType)
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
					throw new JsonException("Json format exception. - '" + json[i - 1].ToString() + "' at " + (i-1).ToString() + "\nJson: \n" + json);
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
						Skip(ref i, json, "-+eE.1234567890null");
						currentPair.content = String2.GetStringAt(json, start, i - 2);
						currentPair.type = (currentPair.content == "null") ? "null" : ((currentPair.content.Contains(".") || currentPair.content.ToLower().Contains("e")) ? "float" : "int");
						currentPair.childPairs = null;
						currentPair.value = (currentPair.content.Contains(".") || currentPair.content.ToLower().Contains("e")) ? float.Parse(currentPair.content) : int.Parse(currentPair.content);
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
			if (array.IndexOf("{") >= 0) { elementType = "object"; }
			else if (array.IndexOf("[", 2) >= 0) { elementType = "array"; }
			else if (array.IndexOf("\"") >= 0) { elementType = "string"; }
			else { elementType = (array.Contains(".") || array.ToLower().Contains("e")) ? "float" : "int"; }

			List<dynamic[]> childs = null;
			string[] elements = null;

			switch (elementType) 
			{
				case "string":
				case "float":
				case "int":
					elements = array.Split(',');
					Pair[] pairs = new Pair[elements.Length];

					if (elementType != "string") { elements = elements.Select(x => x.Replace(" ", "").Replace("\r", "").Replace("\t", "").Replace("\n", "")).ToArray(); }
					else { elements = elements.Select(x => String2.GetStringAt(x, x.IndexOf("\"") + 1, x.LastIndexOf("\"") - 1)).ToArray(); }

					elements[0] = elements[0].Remove(0, 1);
					elements[elements.Length - 1] = elements[elements.Length - 1].Remove(elements[elements.Length - 1].Length - 1, 1);

					for (int i = 0; i < pairs.Length; i++) 
					{
						pairs[i] = new Pair();
						pairs[i].name = null;
						pairs[i].content = elements[i];
						pairs[i].type = elementType;
						pairs[i].childPairs = null;

						if (elementType == "string") { pairs[i].value = elements[i]; }
						if (elementType == "float") { pairs[i].value = float.Parse(elements[i]); }
						if (elementType == "int") { pairs[i].value = int.Parse(elements[i]); }
					}

					return pairs;
				case "array":
					childs = new List<dynamic[]>();
					elements = array.Split('[');

					for (int i = 0; i < elements.Length; i++) 
					{
						elements[i] = elements[i].TrimStart('[', ' ', '\t', '\r', '\n').TrimEnd(',', ']', ' ', '\t', '\r', '\n');
						elements[i] = "[" + elements[i] + "]";
						if (elements[i] == "[]") { continue; }

						string childType;
						childs.Add(GetArrayElements(elements[i], out childType));
						if (elementType.EndsWith("array")) { elementType += "/" + childType; }
					}

					return childs.ToArray();
				case "object":
					childs = new List<dynamic[]>();
					elements = array.Split('{');

					for (int i = 0; i < elements.Length; i++) 
					{
						elements[i] = elements[i].TrimStart('{', ' ', '\t', '\r', '\n').TrimEnd(',', '}', ' ', '\t', '\r', '\n');
						elements[i] = "{" + elements[i] + "}";
						if (elements[i] == "{}") { continue; }

						childs.Add(GetPairsFromJson(elements[i]));
					}

					return childs.ToArray();
			}

			throw new JsonException("Could not create the array.");
		}

		private static int CountInStack(string method) { return String2.Count(Environment.StackTrace, method); }
		private static string Indent(int stackc) { return String2.FillString(' ', 4 * stackc); }

		private static void Skip(ref int index, string str, string chars = "") 
		{
			char[] array = (chars + " \r\n\t").ToCharArray();
			while (Array.IndexOf(array, str[index++]) >= 0) { if (index >= str.Length - 1) { return; } }
		}

		private static bool IsSerialized(FieldInfo field) 
		{
			if (field.GetCustomAttributes(typeof(DeserializeAttribute), false).Length > 0) { return false; }
			if (field.GetCustomAttributes(typeof(SerializeAttribute), false).Length > 0) { return true; }
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

		[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Field)]
		public sealed class SerializeAttribute : Attribute { public SerializeAttribute() {} }

		[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Field)]
		public sealed class DeserializeAttribute : Attribute { public DeserializeAttribute() {} }

		public sealed class JsonException : Exception 
		{
			public JsonException() {}
			public JsonException(string message) : base(message) {}
			public JsonException(string message, Exception inner) : base(message, inner) {}
		}

		private class Pair 
		{
			public string name;
			public string content;
			public string type;
			public object value;
			public dynamic[] childPairs;

			public override string ToString() { return "name: '" + name + "'\ntype: '" + type + "'\nchildCount: " + ((childPairs == null) ? 0 : childPairs.Length).ToString() + "\nvalue: " + value.ToString() + "\ncontent: '" + content + "'"; }

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
}