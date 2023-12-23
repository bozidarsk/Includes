using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Reflection;
using System.Linq;

namespace Utils.Parsers 
{
	public static class Json 
	{
		public static string ToJson(object obj) => Serialize(obj);

		public static void AddSerializer(Serializer serializer) 
		{
			if (serializer == null || serializer.Type == null || serializer.Method == null)
				throw new ArgumentNullException();
			serializers.Add(serializer);
		}

		public static void AddDeserializer(Deserializer deserializer) 
		{
			if (deserializer == null || deserializer.Type == null || deserializer.Method == null)
				throw new ArgumentNullException();
			deserializers.Add(deserializer);
		}

		public static T FromJsonFile<T>(string file) => FromJson<T>(System.IO.File.ReadAllText(file));
		public static T FromJson<T>(string json) 
		{
			TokenDefinition[] definitions = 
			{
				// new TokenDefinition(@"\x22(\w|[!@#$%^&*()\-_=+/|`~\[\]{},<.>/?;:]|\\[\x22nrtv]|\\x[0-9a-fA-F]+])*\x22", "STRING"),
				new TokenDefinition(@"\x22(\\\x22|[^\x22])*\x22", "STRING"),
				new TokenDefinition(@":", "COLON", ":"),
				new TokenDefinition(@",", "COMMA", ","),
				new TokenDefinition(@"\{", "LCB", "{"),
				new TokenDefinition(@"\}", "RCB", "}"),
				new TokenDefinition(@"\[", "LSB", "["),
				new TokenDefinition(@"\]", "RSB", "]"),
				new TokenDefinition(@"true|false", "BOOL"),
				new TokenDefinition(@"null", "NULL", "null"),
				new TokenDefinition(@"-?[0-9]*(\.[0-9]+)?([eE][+-][0-9]+)?", "NUMBER"), // hex?
			};

			json = Regex.Replace(json, @"\/\/.*\r?\n", "");
			json = Regex.Replace(json, @"\/\*(.|\s)*\*\/", "");

			List<Token> tokens = Lexer.Lex(json, definitions).ToList();
			return (T)Deserialize(typeof(T), GetFirstJsonPair(tokens));
		}

		private static List<Serializer> serializers = new List<Serializer>() 
		{
			new Serializer("System.String", x => "\"" + (string)x + "\""), // escape special chars
			new Serializer("System.Char", x => "\"" +  ((char)x).ToString() + "\""),
			new Serializer("System.UInt64", x => ((ulong)x).ToString()),
			new Serializer("System.Int64", x => ((long)x).ToString()),
			new Serializer("System.UInt32", x => ((uint)x).ToString()),
			new Serializer("System.Int32", x => ((int)x).ToString()),
			new Serializer("System.UInt16", x => ((ushort)x).ToString()),
			new Serializer("System.Int16", x => ((short)x).ToString()),
			new Serializer("System.Byte", x => ((byte)x).ToString()),
			new Serializer("System.SByte", x => ((sbyte)x).ToString()),
			new Serializer("System.Decimal", x => ((decimal)x).ToString()),
			new Serializer("System.Double", x => ((double)x).ToString()),
			new Serializer("System.Single", x => ((float)x).ToString()),
			new Serializer("System.Boolean", x => ((bool)x) ? "true" : "false"),
			new Serializer("System.IntPtr", x => ((IntPtr)x).ToString()),
			new Serializer("System.Enum", x => x.GetType().GetFields()[0].GetValue(x).ToString()),
			new Serializer("System.Collections.ICollection", x => {
				string json = "";
				int i = 0;

				foreach (object obj in (System.Collections.ICollection)x) 
				{
					json += Serialize(obj);

					if (++i < ((System.Collections.ICollection)x).Count) { json += ", "; }
				}

				return "[" + json + "]";
			}),
		};

		private static List<Deserializer> deserializers = new List<Deserializer>() 
		{
			new Deserializer("System.Collections.Generic.Dictionary", (Type type, JsonPair[] elements) => 
			{
				Type tKey = type.GenericTypeArguments[0];
				Type tValue = type.GenericTypeArguments[1];

				IDictionary dictionary = (IDictionary)Activator.CreateInstance(type);
				foreach (JsonPair pair in elements) 
					dictionary.Add(
						Deserialize(tKey, pair.Childs[0]),
						Deserialize(tValue, pair.Childs[1])
					);

				return dictionary;
			}),
			new Deserializer("System.Collections.Generic.List", (Type type, JsonPair[] elements) => 
			{
				Type t = type.GenericTypeArguments[0];

				IList list = (IList)Activator.CreateInstance(type);
				foreach (JsonPair pair in elements) 
					list.Add(
						Deserialize(t, pair)
					);

				return list;
			}),
		};

		private static string Serialize(object obj) 
		{
			if (obj == null) { return "null"; }
			Type type = obj.GetType();

			foreach (Serializer serializer in serializers) 
				if (Type.GetType(serializer.Type).IsAssignableFrom(type)) 
					return serializer.Invoke(obj);

			string json = "";

			foreach (FieldInfo field in type
				.GetFields()
				.Where(x => !x.IsStatic && ((x.IsPublic && !HasAttribute<DeserializeAttribute>(x)) || (x.IsPrivate && HasAttribute<SerializeAttribute>(x))))
			) {
				string name = HasAttribute<NameAttribute>(field, out NameAttribute n) ? n.Name : field.Name;
				json += $"\"{name}\": {Serialize(field.GetValue(obj))},";
			}

			foreach (PropertyInfo property in type
				.GetProperties()
				.Where(x => x.CanRead && !x.GetMethod.IsStatic && ((x.GetMethod.IsPublic && !HasAttribute<DeserializeAttribute>(x)) || (x.GetMethod.IsPrivate && HasAttribute<SerializeAttribute>(x))))
			) {
				string name = HasAttribute<NameAttribute>(property, out NameAttribute n) ? n.Name : property.Name;
				json += $"\"{name}\": {Serialize(property.GetMethod.Invoke(obj, null))},";
			}

			return "{" + json.Substring(0, json.Length - 1) + "}";
		}

		private static object Deserialize(Type type, JsonPair root) 
		{
			switch (root.Type) 
			{
				case JsonType.Null:
					return null;
				case JsonType.Bool:
				case JsonType.String:
				case JsonType.Number:
					return (type.IsEnum)
						? Enum.ToObject(type, System.Convert.ChangeType(root.Value, typeof(int)))
						: System.Convert.ChangeType(root.Value, type)
					;
				case JsonType.Object:
					object obj = Activator.CreateInstance(type);

					foreach (FieldInfo field in type
						.GetFields()
						.Where(x => !x.IsStatic && ((x.IsPublic && !HasAttribute<DeserializeAttribute>(x)) || (x.IsPrivate && HasAttribute<SerializeAttribute>(x))))
					) {
						string name = HasAttribute<NameAttribute>(field, out NameAttribute n) ? n.Name : field.Name;
						JsonPair pair = root.Childs.FirstOrDefault(x => x.Name == name);

						if (pair == default(JsonPair))
							continue;

						field.SetValue(obj, Deserialize(field.FieldType, pair));
					}

					foreach (PropertyInfo property in type
						.GetProperties()
						.Where(x => x.CanWrite && !x.SetMethod.IsStatic && ((x.SetMethod.IsPublic && !HasAttribute<DeserializeAttribute>(x)) || (x.SetMethod.IsPrivate && HasAttribute<SerializeAttribute>(x))))
					) {
						string name = HasAttribute<NameAttribute>(property, out NameAttribute n) ? n.Name : property.Name;
						JsonPair pair = root.Childs.FirstOrDefault(x => x.Name == name);

						if (pair == default(JsonPair)) 
							continue;

						property.SetMethod.Invoke(obj, new object[] { Deserialize(property.PropertyType, pair) });
					}

					return obj;
				case JsonType.Array:
					Type elementType = type.GetElementType();

					if (elementType == null) 
					{
						string name = type.ToString();
						name = name.Substring(0, name.IndexOf("`"));

						Deserializer deserializer = deserializers.FirstOrDefault(x => x.Type == name);
						if (deserializer == default(Deserializer)) 
							throw new NotSupportedException($"Deserialization of type '{type}' is not supported.");

						return deserializer.Invoke(type, root.Childs);
					}

					int index = 0;
					Array array = Array.CreateInstance(elementType, root.Childs.Length);
					foreach (JsonPair pair in root.Childs) 
						array.SetValue(
							Deserialize(elementType, pair),
							index++
						);

					return array;
				default:
					throw new ArgumentException($"Unhandled JsonType '{root.Type}'.");
			}
		}

		private static JsonPair GetFirstJsonPair(List<Token> tokens) 
		{
			List<JsonPair> childs = new List<JsonPair>();
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
						childs.Add(new JsonPair(name, JsonType.String, tokens[i + 2].Value.Substring(1, tokens[i + 2].Value.Length - 2)));
						i += 2;
						continue;
					case "NUMBER":
						childs.Add(new JsonPair(name, JsonType.Number, double.Parse(tokens[i + 2].Value)));
						i += 2;
						continue;
					case "BOOl":
						childs.Add(new JsonPair(name, JsonType.Bool, bool.Parse(tokens[i + 2].Value)));
						i += 2;
						continue;
					case "NULL":
						childs.Add(new JsonPair(name, JsonType.Null, null));
						i += 2;
						continue;
					case "LCB":
						end2 = EndOfScope(tokens, "LCB", "RCB", i + 2);
						childs.Add(new JsonPair(name, JsonType.Object, GetFirstJsonPair(tokens.GetRange(i + 2, (end2 - (i + 2)) + 1)).Childs));
						i += 1 + (end2 - (i + 2)) + 1;
						continue;
					case "LSB":
						end2 = EndOfScope(tokens, "LSB", "RSB", i + 2);
						childs.Add(new JsonPair(name, JsonType.Array, TokensToArray(tokens.GetRange(i + 2, (end2 - (i + 2)) + 1))));
						i += 1 + (end2 - (i + 2)) + 1;
						continue;
				}
			}

			return new JsonPair(null, JsonType.Object, childs.ToArray());
		}

		private static JsonPair[] TokensToArray(List<Token> tokens) 
		{
			List<JsonPair> childs = new List<JsonPair>();
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
						childs.Add(new JsonPair(null, JsonType.String, tokens[i].Value.Substring(1, tokens[i].Value.Length - 2)));
						i++;
						continue;
					case "NUMBER":
						childs.Add(new JsonPair(null, JsonType.Number, double.Parse(tokens[i].Value)));
						i++;
						continue;
					case "BOOL":
						childs.Add(new JsonPair(null, JsonType.Bool, bool.Parse(tokens[i].Value)));
						i++;
						continue;
					case "NULL":
						childs.Add(new JsonPair(null, JsonType.Null, null));
						i++;
						continue;
					case "LCB":
						end2 = EndOfScope(tokens, "LCB", "RCB", i);
						childs.Add(new JsonPair(null, JsonType.Object, GetFirstJsonPair(tokens.GetRange(i, (end2 - (i)) + 1)).Childs));
						i += (end2 - (i)) + 1;
						continue;
					case "LSB":
						end2 = EndOfScope(tokens, "LSB", "RSB", i);
						childs.Add(new JsonPair(null, JsonType.Array, TokensToArray(tokens.GetRange(i, (end2 - (i)) + 1))));
						i += (end2 - (i)) + 1;
						continue;
				}
			}

			return childs.ToArray();
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

		private static bool HasAttribute<T>(MemberInfo info) where T : Attribute => HasAttribute<T>(info, out T attribute);
		private static bool HasAttribute<T>(MemberInfo info, out T attribute) where T : Attribute
		{
			object[] objs = info.GetCustomAttributes(typeof(T), false);
			if (objs.Length == 0) { attribute = null; return false; }
			attribute = (T)objs[0];
			return true;
		}

		public enum JsonType 
		{
			Number,
			String,
			Bool,
			Array,
			Object,
			Null,
		}

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

		public class JsonPair 
		{
			public string Name;
			public JsonType Type;
			public object Value;
			public JsonPair[] Childs;

			public JsonPair(string name, JsonType type, object value) 
			{
				this.Name = name;
				this.Type = type;
				this.Value = value;
				this.Childs = null;
			}

			public JsonPair(string name, JsonType type, JsonPair[] childs) 
			{
				this.Name = name;
				this.Type = type;
				this.Value = null;
				this.Childs = childs;
			}
		}

		public sealed class Serializer 
		{
			public string Type { private set; get; }
			public Func<object, string> Method { private set; get; }

			public string Invoke(object obj) => this.Method(obj);

			public Serializer(string type, Func<object, string> method) 
			{
				this.Type = type;
				this.Method = method;
			}
		}

		public sealed class Deserializer 
		{
			public string Type { private set; get; }
			public Func<Type, JsonPair[], object> Method { private set; get; }

			public object Invoke(Type type, JsonPair[] elements) => this.Method(type, elements);

			public Deserializer(string type, Func<Type, JsonPair[], object> method) 
			{
				this.Type = type;
				this.Method = method;
			}
		}
	}
}