using System;
using System.Collections.Generic;
using System.Linq;
using Utils;

namespace Utils.Calculators 
{
	public static class PolynomialCalculator 
	{
		public static readonly string ErrMsg = "Error.";
		public static readonly string Doc = "example: \"5x^4 -2x^3 +3x -1 >= 0\" is equal to \"+5-2+0+3-1>=\"";

		private static int[] GetCoefficients(string polynomial) 
		{
			List<string> numbers = new List<string>();
			string currentNumber = "";

			for (int i = 0; i < polynomial.Length; i++) 
			{
				if ((polynomial[i] == '+' || polynomial[i] == '-') && i != 0) 
				{
					numbers.Add(currentNumber);
					currentNumber = "";
				}

				currentNumber += polynomial[i];
			}

			numbers.Add(currentNumber);
			return numbers.Select(x => int.Parse(x)).ToArray();
		}

		private static bool CheckAnswer(string polynomial, int answer, out string newPolynomial) 
		{
			int[] coefficients = GetCoefficients(polynomial);
			int number = coefficients[0];
			newPolynomial = (coefficients[0] >= 0) ? ("+" + coefficients[0].ToString()) : coefficients[0].ToString();

			for (int i = 0; i < coefficients.Length - 1; i++) 
			{
				number = number * answer + coefficients[i + 1];
				if (number == 0 && i + 1 >= coefficients.Length - 1) { break; }
				newPolynomial += (number >= 0) ? ("+" + number.ToString()) : number.ToString();
			}

			if (number == 0) { return true; }
			else { newPolynomial = null; return false; }
		}

		private static string Format(int[] answers) 
		{
			string output = "";
			int power = 1;

			for (int i = 0; i < answers.Length; i++) 
			{
				try { while (answers[i + 1] == answers[i]) { power++; i++; } }
				catch {}

				output += "(x" + ((-answers[i] >= 0) ? ("+" + (-answers[i]).ToString()) : (-answers[i]).ToString()) + ")";
				output += (power > 1) ? ("^" + power.ToString()) : "";
				power = 1;
			}

			return output;
		}

		public static int[] GetAnswers(string polynomial, out int[][] table) 
		{
			int[] values = { 1, -1, 2, -2, 3, -3, 4, -4, 5, -5, 6, -6, 7, -7, 8, -8, 9, -9 };
			List<List<int>> tableList = new List<List<int>>();
			List<int> answers = new List<int>();
			string newPolynomial = "";

			tableList.Add((new int[] { 0 }).ToList());
			tableList[tableList.Count - 1].AddRange(GetCoefficients(polynomial));

			for (int i = 0; i < values.Length; i++) 
			{
				while (CheckAnswer(polynomial, values[i], out newPolynomial)) 
				{
					tableList.Add((new int[] { values[i] }).ToList());
					tableList[tableList.Count - 1].AddRange(GetCoefficients(newPolynomial));
					tableList[tableList.Count - 1].Add(0);

					answers.Add(values[i]);
					polynomial = newPolynomial;
				}
			}

			table = tableList.Select(x => x.ToArray()).ToArray();
			return answers.ToArray();
		}

		private static string GetTable(int[][] table) 
		{
			string output = "";
			for (int y = 0; y < table.Length; y++) 
			{
				for (int x = 0; x < table[y].Length; x++) 
				{
					string number = table[y][x].ToString();
					while (number.Length < 3) { number = " " + number; }
					output += number;
					output += (x == 0) ? "|  " : "  ";
				}

				output += "\n";
			}

			return "    " + output.Remove(0, 4);
		}

		public static string Simplify(string polynomial) { string table; return Simplify(polynomial, out table); }
		public static string Simplify(string polynomial, out string table) 
		{
			table = null;
			if (string.IsNullOrEmpty(polynomial)) { return null; }

			int[][] t;
			int[] answers = GetAnswers(polynomial.Replace(" ", ""), out t);
			table = GetTable(t);
			return (answers.Length > 0) ? Format(answers.ToArray()) : null;
		}

		public static string Solve(string polynomial) { string table, simplified; return Solve(polynomial, out simplified, out table); }
		public static string Solve(string polynomial, out string simplified, out string table) 
		{
			table = null;
			simplified = null;
			if (string.IsNullOrEmpty(polynomial)) { return null; }
			polynomial = polynomial.Replace(" ", "");

			int index = System.Math.Max(polynomial.IndexOf(">"), polynomial.IndexOf("<"));
			if (index == -1) { return null; }

			string operation = polynomial[index].ToString();
			try { operation += (polynomial[index + 1] == '=') ? "=" : ""; }
			catch {}

			int[][] t;
			int[] answers = GetAnswers(polynomial.Remove(index), out t);
			simplified = (answers.Length > 0) ? Format(answers.ToArray()) : null;
			simplified += " " + operation + " 0";
			table = GetTable(t);
			Array.Sort(answers);

			List<Range> ranges = new List<Range>();

			int positive = 0;
			for (int i = answers.Length - 1; i >= -1; i--) 
			{
				string down = "-inf";
				string up = "+inf";

				try { down = answers[i].ToString(); } catch {}
				try { up = answers[i + 1].ToString(); } catch {}

				ranges.Add(new Range(
					down,
					up,
					(positive % 2) == 0
				));

				positive++;
			}

			string output = "";
			for (int i = ranges.Count - 1; i >= 0; i--) 
			{
				if (!((operation[0] == '>' && ranges[i].positive) || (operation[0] == '<' && !ranges[i].positive)) || ranges[i].from == ranges[i].to) { continue; }

				string range = ranges[i].ToString().Remove(0, 1);
				if (operation.Length > 1 && !ranges[i].from.Contains("inf")) { range = range.Replace("(", "["); }
				if (operation.Length > 1 && !ranges[i].to.Contains("inf")) { range = range.Replace(")", "]"); }
				output += range + "U";
			}

			return "x = " + output.Remove(output.Length - 1, 1);
		}

		private class Range 
		{
			public bool positive { set; get; }
			public string from { set; get; }
			public string to { set; get; }

			public override string ToString() { return ((positive) ? "+" : "-") + "(" + from + "; " + to + ")"; }
			
			public Range() {}
			public Range(string from, string to, bool positive) 
			{
				this.from = from;
				this.to = to;
				this.positive = positive;
			}
		}
	}
}