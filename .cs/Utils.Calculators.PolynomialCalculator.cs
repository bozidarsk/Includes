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

		private static float[] GetCoefficients(string polynomial) 
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
			return numbers.Select(x => float.Parse(x)).ToArray();
		}

		private static bool CheckAnswer(float[] coefficients, float answer, out string newPolynomial) 
		{
			float number = coefficients[0];
			newPolynomial = (coefficients[0] >= 0) ? ("+" + coefficients[0].ToString()) : coefficients[0].ToString();

			for (int i = 0; i < coefficients.Length - 1; i++) 
			{
				number = number * answer + coefficients[i + 1];
				if (number == 0f && i + 1 >= coefficients.Length - 1) { break; }
				newPolynomial += (number >= 0f) ? ("+" + number.ToString()) : number.ToString();
			}

			if (number == 0f) { return true; }
			else { newPolynomial = null; return false; }
		}

		private static float[] GetValues(float firstCoefficient, float lastCoefficient) 
		{
			if (firstCoefficient < 0f) { throw new NotImplementedException("firstCoefficient < 0f"); }
			// firstCoefficient *= (firstCoefficient < 0f) ? -1f : 1f;
			lastCoefficient *= (lastCoefficient < 0f) ? -1f : 1f;

			List<float> last = new List<float>();
			for (int i = 1; i <= lastCoefficient; i++) { if (lastCoefficient % (float)i == 0) { last.Add((float)i); last.Add((float)-i); } }
			if (firstCoefficient == 1f) { return last.ToArray(); }

			List<float> values = new List<float>();
			for (int i = 1; i <= firstCoefficient; i++) 
			{
				if (firstCoefficient % (float)i == 0f) 
				{
					values.AddRange(last.Select(x => x / (float)i).ToArray());
					values.AddRange(last.Select(x => x / (float)-i).ToArray());
				}
			}

			values.AddRange(last);
			return values.ToArray();
		}

		private static string Format(float[] answers) 
		{
			string output = "";
			int power = 1;

			for (int i = 0; i < answers.Length; i++) 
			{
				try { while (answers[i + 1] == answers[i]) { power++; i++; } }
				catch {}

				output += "(x" + ((-answers[i] >= 0f) ? ("+" + (-answers[i]).ToString()) : (-answers[i]).ToString()) + ")";
				output += (power > 1) ? ("^" + power.ToString()) : "";
				power = 1;
			}

			return output;
		}

		public static float[] GetAnswers(string polynomial, out float[][] table) 
		{
			float[] coefficients = GetCoefficients(polynomial);
			float[] values = GetValues(coefficients[0], coefficients[coefficients.Length - 1]);
			List<List<float>> tableList = new List<List<float>>();
			List<float> answers = new List<float>();

			tableList.Add((new float[] { 0f }).ToList());
			tableList[tableList.Count - 1].AddRange(coefficients);

			for (int i = 0; i < values.Length; i++) 
			{
				while (CheckAnswer(coefficients, values[i], out polynomial)) 
				{
					coefficients = GetCoefficients(polynomial);
					tableList.Add((new float[] { values[i] }).ToList());
					tableList[tableList.Count - 1].AddRange(coefficients);
					tableList[tableList.Count - 1].Add(0);

					answers.Add(values[i]);
				}
			}

			table = tableList.Select(x => x.ToArray()).ToArray();
			return answers.ToArray();
		}

		private static string GetTable(float[][] table) 
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
			if (polynomial.IndexOf(">") < 0 && polynomial.IndexOf("<") < 0 && polynomial.IndexOf("=") < 0) { return null; }

			try { polynomial = polynomial.Remove(System.Math.Max(System.Math.Max(polynomial.IndexOf(">"), polynomial.IndexOf("<")), polynomial.IndexOf("="))); }
			catch { return null; }

			float[][] t;
			float[] answers = GetAnswers(polynomial.Replace(" ", ""), out t);
			table = GetTable(t);
			return (answers.Length > 0) ? Format(answers) : null;
		}

		public static string Solve(string polynomial) { string table, simplified; return Solve(polynomial, out simplified, out table); }
		public static string Solve(string polynomial, out string simplified, out string table) 
		{
			table = null;
			simplified = null;
			if (string.IsNullOrEmpty(polynomial)) { return null; }
			polynomial = polynomial.Replace(" ", "");

			int index = System.Math.Max(polynomial.IndexOf(">"), polynomial.IndexOf("<"));
			if (index == -1) 
			{
				if (polynomial.IndexOf("=") >= 0) { simplified = Simplify(polynomial, out table); return simplified + " = 0"; }
				return null;
			}

			string operation = polynomial[index].ToString();
			try { operation += (polynomial[index + 1] == '=') ? "=" : ""; }
			catch {}

			float[][] t;
			float[] answers = GetAnswers(polynomial.Remove(index), out t);
			simplified = (answers.Length > 0) ? Format(answers) : null;
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
					(positive++ % 2) == 0
				));
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