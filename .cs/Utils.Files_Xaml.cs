using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Utils.Files 
{
	public static class Xaml 
	{
		public static string ToXaml(Element root, bool prettyPrint = false) 
		{
			int stackc = (prettyPrint) ? (CountInStack("Utils.Files.Xaml.ToXaml") - 1) : -1;
			string output = "";

			output += ((prettyPrint) ? Indent(stackc) : "") + "<" + root.Name;
			if (root.Attributes.Length > 0) { for (int i = 0; i < root.Attributes.Length; i++) { output += " " + root.Attributes[i].ToString(); } }
			if (root.IsSelfClosing) { return output += "/>" + ((prettyPrint) ? "\n" : ""); }
			output += ">";

			if (root.Childs.Length > 0) 
			{
				if (prettyPrint) { output += "\n"; }
				for (int i = 0; i < root.Childs.Length; i++) { output += ToXaml(root.Childs[i], prettyPrint); }
			}
			else { output += root.Content; }

			return output + ((prettyPrint && root.Childs.Length > 0) ? Indent(stackc) : "") + "</" + root.Name + ">" + ((prettyPrint) ? "\n" : "");
		}

		public static Element FromXamlFile(string file) { return FromXaml(System.IO.File.ReadAllText(file)); }
		public static Element FromXaml(string xaml) 
		{
			int startIndex = 0;
			for (int index = xaml.IndexOf("<!--"); index >= 0; index = xaml.IndexOf("<!--", startIndex)) 
			{
				if (String2.Count(String2.GetStringAt(xaml, 0, index), '\"') % 2 != 0) { startIndex = index + 1; continue; }
				xaml = 
					((index - 1 >= 0) ? String2.GetStringAt(xaml, 0, index - 1) : "") + 
					String2.GetStringAt(xaml, xaml.IndexOf("-->", index) + 3, xaml.Length - 1)
				;
			}

			return GetFirstElement(xaml);
		}

		private static int CountInStack(string method) { return String2.Count(Environment.StackTrace, method); }
		private static string Indent(int stackc) { return String2.FillString(' ', (4 * stackc >= 0) ? 4 * stackc : 0); }

		private static Element GetFirstElement(string xaml) 
		{
			Element element = new Element();
			int index, close;
			int i = 0;

			element.Name = "";
			Skip(ref i, xaml, "<");
			for (i--; xaml[i] != ' ' && xaml[i] != '/' && xaml[i] != '>'; i++) { element.Name += xaml[i]; }

			close = String2.EndOfScope(xaml, "<" + element.Name, "</" + element.Name, i - element.Name.Length - 1);
			element.IsSelfClosing = close == -1;

			int end = -1;
			int startIndex = i;
			for (index = xaml.IndexOf(">", i); index >= 0; index = xaml.IndexOf(">", startIndex)) 
			{
				if (String2.Count(String2.GetStringAt(xaml, 0, index), '\"') % 2 != 0) { startIndex = index + 1; continue; }
				if (end == index) { break; }
				end = index;
			}

			index = xaml.IndexOf("=", i);
			if (index < end && index != -1) 
			{
				List<Attribute> attributes = new List<Attribute>();
				for (Skip(ref i, xaml); index < end && index != -1; Skip(ref i, xaml, "\"")) 
				{
					string name = "";
					for (i--; xaml[i] != ' ' && xaml[i] != '='; i++) { name += xaml[i]; }
					Skip(ref i, xaml, "=\"");

					string value = "";
					for (i--; xaml[i] != '"' || (xaml[i] == '"' && xaml[i - 1] == '\\'); i++) { value += xaml[i]; }

					attributes.Add(new Attribute(name, value));
					index = xaml.IndexOf("=", i);
				}

				element.Attributes = attributes.ToArray();
				i = end + 1;
			}
			else { element.Attributes = new Attribute[] {}; }

			if (element.IsSelfClosing || i - (close - 1) == 1) 
			{
				element.Childs = new Element[] {};
				element.Content = "";
				return element;
			}

			if (element.Attributes.Length == 0) { i++; }
			element.Content = String2.GetStringAt(xaml, i, close - 1);
			Skip(ref i, xaml);

			if (xaml[i - 1] != '<') 
			{
				element.Childs = new Element[] {};
				return element;
			}

			List<string> childs = new List<string>();
			while (xaml[i - 1] == '<' && xaml[i] != '/') 
			{
				string childName = "";
				for (int t = i; xaml[t] != ' ' && xaml[t] != '/' && xaml[t] != '>'; t++) { childName += xaml[t]; }

				try { childs.Add(String2.GetStringAt(xaml, i - 1, childName.Length + 2 + String2.EndOfScope(xaml, "<" + childName, "</" + childName, i - 1))); }
				catch { childs.Add(String2.GetStringAt(xaml, i - 1, String2.EndOfScope(xaml, "<", ">", i - 1))); }

				i += childs[childs.Count - 1].Length;
				Skip(ref i, xaml);
			}

			element.Childs = childs.Select(x => GetFirstElement(x)).ToArray();

			element.Content = "";
			return element;
		}

		private static void Skip(ref int index, string str, string chars = "") 
		{
			char[] array = (chars + " \r\n\t").ToCharArray();
			try { while (Array.IndexOf(array, str[index++]) >= 0) { if (index >= str.Length - 1) { return; } } }
			catch {}
		}

		public sealed class Element 
		{
			public string Name;
			public string Content;
			public Element[] Childs;
			public Attribute[] Attributes;
			public bool IsSelfClosing;
			public Element() {}
		}

		public sealed class Attribute 
		{
			public string Name;
			public string Value;

			public override string ToString() { return Name + "=\"" + Value + "\""; }

			public Attribute() {}
			public Attribute(string Name, string Value) 
			{
				this.Name = Name;
				this.Value = Value;
			}
		}
	}
}