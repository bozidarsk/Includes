using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.ComponentModel;

namespace Utils.Parsers 
{
	public static class Lexer 
	{
		public static IEnumerable<Token> Lex(string input, IEnumerable<TokenDefinition> definitions) 
		{
			List<Token> tokens = new List<Token>();
			List<int> indexes = new List<int>();

			foreach (TokenDefinition def in definitions) 
			{
				for (Match match = Regex.Match(input, def.Regex); match.Success; match = match.NextMatch()) 
				{
					if (string.IsNullOrEmpty(match.Value)) { continue; }
					indexes.Add(match.Index);
					tokens.Add(new Token(
						def.Name,
						def.DefaultValue ?? match.Value
					));

					string substitute = "";
					for (int t = 0; t < match.Length; t++) { substitute += "\x1a"; }
					input = input.Remove(match.Index, match.Length);
					input = input.Insert(match.Index, substitute);
				}
			}

			for (int i = 0; i < tokens.Count; i++) 
			{
				for (int t = 0; t < tokens.Count; t++) 
				{
					if (indexes[i] > indexes[t]) { continue; }

					int tmpi = indexes[i];
					indexes[i] = indexes[t];
					indexes[t] = tmpi;

					Token tmpt = tokens[i];
					tokens[i] = tokens[t];
					tokens[t] = tmpt;
				}
			}

			return tokens;
		}
	}
}