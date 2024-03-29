using System;

namespace Utils.Parsers 
{
	public sealed class SyntaxException : Exception 
	{
		public SyntaxException() : base() {}
		public SyntaxException(string message) : base(message) {}
		public SyntaxException(string message, Exception inner) : base(message, inner) {}
	}

	public class Token 
	{
		public string Name { private set; get; }
		public string Value { private set; get; }

		public override string ToString() => this.Value;

		public Token(string name, string value) 
		{
			this.Name = name;
			this.Value = value;
		}
	}

	public class TokenDefinition 
	{
		public string Regex { private set; get; }
		public string Name { private set; get; }
		public string DefaultValue { private set; get; }

		public TokenDefinition(string regex, string name, string defaultValue = null) 
		{
			this.Regex = regex;
			this.Name = name;
			this.DefaultValue = defaultValue;
		}
	}
}