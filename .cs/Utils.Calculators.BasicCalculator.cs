using System;
using System.Collections.Generic;
using Utils;

namespace Utils.Calculators 
{
	public static class BasicCalculator 
    {
    	public static TrigType trigType = TrigType.Radians;
		public static readonly string ErrMsg = "Error.";
		public static readonly string Doc = 
			"help - Displays this message.\n" + 
			"sin(x) - Returns the sine of angle 'x'.\n" + 
			"asin(x) - Returns the angle whose sine is 'x'.\n" + 
			"cos(x) - Returns the cosine of angle 'x'.\n" + 
			"acos(x) - Returns the angle whose cosine is 'x'.\n" + 
			"tan(x) - Returns the tangent of angle 'x'.\n" + 
			"atan(x) - Returns the angle whose tangent is 'x'.\n" + 
			"sqrt(x) - Returns the square root of 'x'.\n" + 
			"abs(x) - Returns the absoulte value of 'x'.\n" + 
			"ceiling(x) - Returns the smallest integral value less than or equal to 'x'.\n" + 
			"floor(x) - Returns the largest integral value less than or equal to 'x'.\n" + 
			"min(a, b) - Returns the smallest from 'a' and 'b'.\n" + 
			"max(a, b) - Returns the largest from 'a' and 'b'.\n" + 
			"lerp(a, b, x) - Returns linear interpolation of 'a' and 'b' based on weight 'x'.\n" + 
			"inverselerp(a, b, x) - Returns inverse linear interpolation.\n" + 
			"log(x) - Returns the natural (base e) logarithm of 'x'.\n" + 
			"log(x, newBase) - Returns the logarithm of 'x' in a specified base.\n" + 
			"clamp(min, max, x) - Returns 'x' clamped to the inclusive range of min and max."
		;

		private static char[] allowedChars = { '.', ',', '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', '^', '√', '*', '/', '+', '-', '(', ')', '!', '%' };
		private static char[] operators = { '^', '*', '/', '+', '-', '%', '√' };
		private static string[] functions = { "&(", "sin(", "cos(", "tan(", "asin(", "acos(", "atan(", "sqrt(", "abs(", "ceiling(", "floor(", "min(", "max(", "lerp(", "inverselerp(", "log(", "clamp(" }; // '&' brackets are calculated as function, it just needs a name

		private static string Error(string output) { return ErrMsg + " (" + output + ")"; }

	    private static int CountChar(string main, char target) 
		{
			int i = 0;
			int count = 0;
			while (i < main.Length)
			{
			    if (main[i] == target) { count++; }
			    i++;
			}

			return count;
		}

		private static string FormatNumber(string number) 
		{
			if (number.StartsWith(ErrMsg)) { return number; }
			string result = "";

			if (!number.Contains("!") && !number.Contains("√")) { return number; }
	    	else 
	    	{
	    		if (number.Contains("!")) 
	    		{
	    			if (number[number.Length - 1] != '!') { return Error("invalid position of '!' -> '" + number + "'"); }
	    			number = number.Remove(number.Length - 1, 1);
	    			result = Utils.Math.Fact(Utils.Format.stoi(number)).ToString();
	    		}

	    		if (number.Contains("√")) 
	    		{
	    			if (number[0] != '√') { return Error("invalid position of '√' -> '" + number + "'"); }
	    			if (number[0] == '-') { return Error("negative number under sqrt -> '√" + number + "'"); }
	    			number = number.Remove(0, 1);
	    			result = Utils.Format.ftos(Utils.Math.Sqrt(Utils.Format.stof(number)));
	    		}
	    	}

	    	return result;
		}

	    private static string Calculate(string content) 
	    {
	    	if (content.StartsWith(ErrMsg)) { return content; }
	    	if (content.Contains("(")) { content = CheckForFunctions(content); }

	    	List<string> _numbers = new List<string>();
	    	List<char> _operators = new List<char>();
	    	float result = 0f;
	    	string number = "";
	    	int i = 0;

	    	while (i < content.Length) 
	    	{
	    		if (Array.IndexOf(operators, content[i]) < 0 || ((content[i] == '√' || content[i] == '-') && (i == 0 || Array.IndexOf(operators, content[i - 1]) >= 0))) 
	    		{
	    			number += content[i];
	    			if (i + 1 >= content.Length) { _numbers.Add(number); }
				}
	    		else 
	    		{
	    			_numbers.Add(number);
	    			_operators.Add(content[i]);
	    			number = "";
	    		}

	    		i++;
	    	}

	    	if (_numbers.Count - 1 != _operators.Count && _numbers.Count > 1) { return Error("'_numbers.Count != _operators.Count - 1' -> '" + content + "'"); }

	    	result = Utils.Format.stof(FormatNumber(_numbers[0]));

	    	i = 1;
	    	while (i < _numbers.Count) 
	    	{
	    		switch (_operators[i - 1]) 
	    		{
	    			case '^':
	    				_numbers[i] = FormatNumber(_numbers[i]);
	    				result = Utils.Math.Pow(result, Utils.Format.stof(_numbers[i]));
	    				break;
	    			case '√':
	    				_numbers[i] = FormatNumber(_numbers[i]);
	    				if(_numbers[i][0] == '-') { return Error("negative number under root -> '" + Utils.Format.ftos(result) + "√" + _numbers[i] + "'"); }
	    				result = Utils.Math.Pow(Utils.Format.stof(_numbers[i]), 1f / result);
	    				break;
	    			case '*':
	    				_numbers[i] = FormatNumber(_numbers[i]);
	    				result *= Utils.Format.stof(_numbers[i]);
	    				break;
	    			case '/':
	    				_numbers[i] = FormatNumber(_numbers[i]);
	    				if (Utils.Format.stof(_numbers[i]) == 0f) { return Error("dividing by zero -> '" + Utils.Format.ftos(result) + "/" + _numbers[i] + "'"); }
	    				result /= Utils.Format.stof(_numbers[i]);
	    				break;
	    			case '+':
	    				_numbers[i] = FormatNumber(_numbers[i]);
	    				result += Utils.Format.stof(_numbers[i]);
	    				break;
	    			case '-':
	    				_numbers[i] = FormatNumber(_numbers[i]);
	    				result -= Utils.Format.stof(_numbers[i]);
	    				break;
	    			case '%':
	    				_numbers[i] = FormatNumber(_numbers[i]);
	    				if (Utils.Format.stof(_numbers[i]) == 0f) { return Error("dividing by zero -> '" + Utils.Format.ftos(result) + "/" + _numbers[i] + "'"); }
	    				result = result % Utils.Format.stof(_numbers[i]);
	    				break;
	    		}

	    		i++;
	    	}

	    	return Utils.Format.ftos(result);
	    }

		private static string CalculateFunction(string content, string type) 
		{
			if (content.StartsWith(ErrMsg)) { return content; }

			string[] args = content.Split(',');
			float calculated = Utils.Format.stof(Calculate(args[0]));
			switch (type) 
			{
				case "&(":
					if (args.Length != 1) { return Error("invalid argument -> '" + type + content + ")'"); }
					return Utils.Format.ftos(calculated);
				case "sin(":
					if (args.Length != 1) { return Error("invalid argument -> '" + type + content + ")'"); }
					if (trigType == TrigType.Degrees) { return Utils.Format.ftos(Utils.Math.Sin(calculated * Utils.Math.DEG2RAD) * Utils.Math.RAD2DEG); }
					else { return Utils.Format.ftos(Utils.Math.Sin(calculated)); }
				case "cos(":
					if (args.Length != 1) { return Error("invalid argument -> '" + type + content + ")'"); }
					if (trigType == TrigType.Degrees) { return Utils.Format.ftos(Utils.Math.Cos(calculated * Utils.Math.DEG2RAD) * Utils.Math.RAD2DEG); }
					else { return Utils.Format.ftos(Utils.Math.Cos(calculated)); }
				case "tan(":
					if (args.Length != 1) { return Error("invalid argument -> '" + type + content + ")'"); }
					if (trigType == TrigType.Degrees) { return Utils.Format.ftos(Utils.Math.Tan(calculated * Utils.Math.DEG2RAD) * Utils.Math.RAD2DEG); }
					else { return Utils.Format.ftos(Utils.Math.Tan(calculated)); }
				case "asin(":
					if (args.Length != 1) { return Error("invalid argument -> '" + type + content + ")'"); }
					if (trigType == TrigType.Degrees) { return Utils.Format.ftos(Utils.Math.Asin(calculated * Utils.Math.DEG2RAD) * Utils.Math.RAD2DEG); }
					else { return Utils.Format.ftos(Utils.Math.Asin(calculated)); }
				case "acos(":
					if (args.Length != 1) { return Error("invalid argument -> '" + type + content + ")'"); }
					if (trigType == TrigType.Degrees) { return Utils.Format.ftos(Utils.Math.Acos(calculated * Utils.Math.DEG2RAD) * Utils.Math.RAD2DEG); }
					else { return Utils.Format.ftos(Utils.Math.Acos(calculated)); }
				case "atan(":
					if (args.Length != 1) { return Error("invalid argument -> '" + type + content + ")'"); }
					if (trigType == TrigType.Degrees) { return Utils.Format.ftos(Utils.Math.Atan(calculated * Utils.Math.DEG2RAD) * Utils.Math.RAD2DEG); }
					else { return Utils.Format.ftos(Utils.Math.Atan(calculated)); }
				case "sqrt(":
					if (args.Length != 1) { return Error("invalid argument -> '" + type + content + ")'"); }
					if (calculated < 0f) { return Error("negative number under sqrt -> 'sqrt(" + content + ")' -> 'sqrt(" + Utils.Format.ftos(calculated) + ")'"); }
					return Utils.Format.ftos(Utils.Math.Sqrt(calculated));
				case "abs(":
					if (args.Length != 1) { return Error("invalid argument -> '" + type + content + ")'"); }
					return Utils.Format.ftos(Utils.Math.Abs(calculated));
				case "ceiling(":
					if (args.Length != 1) { return Error("invalid argument -> '" + type + content + ")'"); }
					return Utils.Format.ftos(Utils.Math.Ceiling(calculated));
				case "floor(":
					if (args.Length != 1) { return Error("invalid argument -> '" + type + content + ")'"); }
					return Utils.Format.ftos(Utils.Math.Floor(calculated));
				case "min(":
					if (args.Length != 2) { return Error("invalid arguments -> '" + type + content + ")'"); }
					return Utils.Format.ftos(Utils.Math.Min(calculated, Utils.Format.stof(Calculate(args[1]))));
				case "max(":
					if (args.Length != 2) { return Error("invalid arguments -> '" + type + content + ")'"); }
					return Utils.Format.ftos(Utils.Math.Max(calculated, Utils.Format.stof(Calculate(args[1]))));
				case "lerp(":
					if (args.Length != 3) { return Error("invalid arguments -> '" + type + content + ")'"); }
					return Utils.Format.ftos(Utils.Math.Lerp(calculated, Utils.Format.stof(Calculate(args[1])), Utils.Format.stof(Calculate(args[2]))));
				case "inverselerp(":
					if (args.Length != 3) { return Error("invalid arguments -> '" + type + content + ")'"); }
					return Utils.Format.ftos(Utils.Math.InverseLerp(calculated, Utils.Format.stof(Calculate(args[1])), Utils.Format.stof(Calculate(args[2]))));
				case "clamp(":
					if (args.Length != 3) { return Error("invalid arguments -> '" + type + content + ")'"); }
					return Utils.Format.ftos(Utils.Math.Clamp(calculated, Utils.Format.stof(Calculate(args[1])), Utils.Format.stof(Calculate(args[2]))));
				case "log(":
					switch (args.Length) 
					{
						case 1:
							return Utils.Format.ftos(Utils.Math.Log(calculated));
						case 2:
							return Utils.Format.ftos(Utils.Math.Log(calculated, Utils.Format.stof(Calculate(args[1]))));
						default:
							return Error("invalid arguments -> '" + type + content + ")'");
					}
			}

			return content;
		}

		private static string CheckForFunctions(string main) 
		{
			if (main.StartsWith(ErrMsg)) { return main; }
			if (!main.Contains("(")) { return main; }

			int i = 0;
			int t = 0;
			int count = 0;
			string content = "";
			while (i < functions.Length) 
			{
				while (main.Contains(functions[i])) 
				{
					count = 1;
					content = "";
					t = main.IndexOf(functions[i]) + functions[i].Length;
					while (t < main.Length) 
					{
						if (main[t] == '(') { count++; }
						if (main[t] == ')') { count--; }
						if (count == 0) { break; }
						content += main[t];
						t++;
					}

					content = (CountChar(content, '(') - CountChar(content, ')') != 0 || (!content.Contains("(") && content[content.Length - 1] == ')')) ? content.Remove(content.Length - 1, 1) : content;
					main = main.Replace(functions[i] + content + ")", CalculateFunction(content, functions[i]));
				}

				i++;
			}

			return main;
		}

		private static string FormatBrackets(string main) 
		{
			int i = 0;
			while (i < main.Length) 
			{
				if (main[i] == '(' && Array.IndexOf(allowedChars, main[(i == 0) ? i : i - 1]) >= 0) 
				{
					main = String2.GetStringAt(main, 0, i - 1) + "&" + String2.GetStringAt(main, i, main.Length - 1);
				}

				if (main[i] == '|') 
				{
					// how?
				}

				i++;
			}

			return main;
		}

		private static string ReplaceConstants(string main) 
		{
			if (main.Contains("π")) { main = main.Replace("π", Utils.Format.ftos(Utils.Math.PI)); }
			if (main.Contains("pi")) { main = main.Replace("pi", Utils.Format.ftos(Utils.Math.PI)); }

			int i = 0;
			while (i < main.Length) 
			{
				if (main[i] == 'e') 
				{
					if (Array.IndexOf(allowedChars, main[(i - 1 < 0) ? 1 : i - 1]) >= 0 && Array.IndexOf(allowedChars, main[(i + 1 >= main.Length) ? main.Length - 2 : i + 1]) >= 0) 
					{
						main = ((i - 1 < 0) ? "" : String2.GetStringAt(main, 0, i - 1)) + Utils.Format.ftos(Utils.Math.E) + ((i + 1 >= main.Length) ? "" : String2.GetStringAt(main, i + 1, main.Length - 1));
					}
				}

				i++;
			}

			return main;
		}

		private static string Format(string main) 
		{
			if (main == "" || main == " " || CountChar(main, ' ') == main.Length) { return Error("empty input"); }
			if (main.Contains("&")) { return Error("'&' is not allowed -> " + Utils.Format.itos(main.IndexOf("&"))); }
			if (CountChar(main, '(') != CountChar(main, ')')) { return Error("'(' or ')'"); }
			if (CountChar(main, '|') % 2 != 0) { return Error("'|'"); }

			main = ReplaceConstants(main);
			main = FormatBrackets(main);
			main = CheckForFunctions(main);

			int i = 0;
			while (i < main.Length) 
			{
				if (Array.IndexOf(allowedChars, main[i]) < 0) { return Error("'" + main[i].ToString() + "' is not allowed -> " + Utils.Format.itos(main.IndexOf(main[i]))); }
				i++;
			}

			return main;
		}

		public static string Solve(string input, TrigType trigType = TrigType.Radians, bool useFullErrorMsgs = true) 
		{
			BasicCalculator.trigType = trigType;
			string output = ErrMsg;

			try { output = Calculate(Format(input.Replace(" ", "").ToLower())); }
			catch (Exception e) { return (useFullErrorMsgs) ? e.GetType().ToString().Replace("System.", "") : ErrMsg; }

			if (output.StartsWith(ErrMsg)) { return (useFullErrorMsgs) ? output : ErrMsg; }
			else { return output; }
		}
    }
}