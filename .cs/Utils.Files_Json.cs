using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Reflection;
using System.Linq;
using System.IO;
using System.Text;

namespace Utils.Files 
{
	public static class Json 
	{
		public static string ToJson(object obj, bool prettyPrint = false) 
		{
			if (obj == null) { return "null"; }
			int stackc = (prettyPrint) ? CountInStack("Utils.Files.Json.ToJson") : -1;

			Type type = obj.GetType();
			if (type.IsEnum) { return type.GetFields()[0].GetValue(obj).ToString(); }
			if (IsBasic(type)) { return (type == typeof(string) || type == typeof(char)) ? "\"" + obj.ToString().Replace("\"", "\\\"") + "\"" : obj.ToString().ToLower(); }

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
						try { currentPair.content = String2.GetStringAt(json, i, json.IndexOf("\"", i) - 1); }
						catch { currentPair.content = ""; }
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

					// why did i put this here from the first place ????
					// elements[0] = elements[0].Remove(0, 1);
					// elements[elements.Length - 1] = elements[elements.Length - 1].Remove(elements[elements.Length - 1].Length - 1, 1);

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
}