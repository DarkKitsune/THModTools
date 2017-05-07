using System;
using System.Text.RegularExpressions;

namespace THModTools
{
	public static class Preprocessor
	{
		static Regex REPrec2 = new Regex(@"[^[0-9a-zA-Z_](([-~])([%$]\w+))[^.]", RegexOptions.Compiled);
		static Regex REPrec3 = new Regex(@"(([0-9]+\.?[0-9]*f?|[$%][A-Za-z_]\w*|\[-\d+\]|\[-\d+.\d+f\])\s*([*/%])\s*([0-9]+\.?[0-9]*f?|[$%][A-Za-z_]\w*|\[-\d+\]|\[-\d+.\d+f\]))", RegexOptions.Compiled);
		static Regex REPrec4 = new Regex(@"(([0-9]+\.?[0-9]*f?|[$%][A-Za-z_]\w*|\[-\d+\]|\[-\d+.\d+f\])\s*([+-])\s*([0-9]+\.?[0-9]*f?|[$%][A-Za-z_]\w*|\[-\d+\]|\[-\d+.\d+f\]))", RegexOptions.Compiled);
		static Regex REPrec5 = new Regex(@"(([0-9]+\.?[0-9]*f?|[$%][A-Za-z_]\w*|\[-\d+\]|\[-\d+.\d+f\])\s*(<<|>>)\s*([0-9]+\.?[0-9]*f?|[$%][A-Za-z_]\w*|\[-\d+\]|\[-\d+.\d+f\]))", RegexOptions.Compiled);
		static Regex REPrec6 = new Regex(@"(([0-9]+\.?[0-9]*f?|[$%][A-Za-z_]\w*|\[-\d+\]|\[-\d+.\d+f\])\s*(<|<=|>|>=)\s*([0-9]+\.?[0-9]*f?|[$%][A-Za-z_]\w*|\[-\d+\]|\[-\d+.\d+f\]))", RegexOptions.Compiled);
		static Regex REPrec7 = new Regex(@"(([0-9]+\.?[0-9]*f?|[$%][A-Za-z_]\w*|\[-\d+\]|\[-\d+.\d+f\])\s*(==|!=)\s*([0-9]+\.?[0-9]*f?|[$%][A-Za-z_]\w*|\[-\d+\]|\[-\d+.\d+f\]))", RegexOptions.Compiled);
		static Regex REPrec10 = new Regex(@"((\d+)\s*\|\s*(\d+))", RegexOptions.Compiled);
		static Regex REPrec11 = new Regex(@"(([0-9]+\.?[0-9]*f?|[$%][A-Za-z_]\w*|\[-\d+\]|\[-\d+.\d+f\])\s*(&&)\s*([0-9]+\.?[0-9]*f?|[$%][A-Za-z_]\w*|\[-\d+\]|\[-\d+.\d+f\]))", RegexOptions.Compiled);
		static Regex REPrec12 = new Regex(@"(([0-9]+\.?[0-9]*f?|[$%][A-Za-z_]\w*|\[-\d+\]|\[-\d+.\d+f\])\s*(\|\|)\s*([0-9]+\.?[0-9]*f?|[$%][A-Za-z_]\w*|\[-\d+\]|\[-\d+.\d+f\]))", RegexOptions.Compiled);

		static Func<string, string> Inst = ECLScriptReader.GetInst;

		public static string[] Expression(string line)
		{
			string oldLine = line;
			string[] ret = new string[] {"", ""};

			var first = 0;
			while (first > -1)
			{
				first = line.IndexOf('(', first);
				if (first == -1)
					break;
				var last = -1;

				var dontuse = false;
				for (var previous = first - 1; previous >= 0; previous--)
				{
					if (first != ' ')
					{
						if (
							(line[previous] >= 'a' && line[previous] <= 'z') ||
							(line[previous] >= 'A' && line[previous] <= 'Z') ||
							(line[previous] >= '0' && line[previous] <= '9') ||
							line[previous] == '_')
							dontuse = true;
						break;
					}
				}
				if (dontuse)
				{
					first++;
					continue;
				}

				var depth = 1;
				for (var i = first + 1; i < line.Length; i++)
				{
					if (line[i] == '(')
						depth++;
					else
						if (line[i] == ')')
							depth--;

					if (depth == 0)
					{
						last = i;
						break;
					}
				}

				if (last == -1)
					throw new Exception("Unmatched '(' near \"" + ECLScriptReader.CurLine + "\" (line " + (ECLScriptReader.CurLineNumber + 1) + ")");

				var newExpr = line.Substring(first + 1, last - first - 1);
				var pp = Expression(newExpr);
				ret[0] += pp[0];
				line = line.Remove(first, last - first + 1).Insert(first, pp[1]);

				first++;
			}

			var prec2 = Prec2(line);
			ret[0] += prec2[0];
			line = prec2[1];

			var prec3 = Prec3(line);
			ret[0] += prec3[0];
			line = prec3[1];

			var prec4 = Prec4(line);
			ret[0] += prec4[0];
			line = prec4[1];

			var prec5 = Prec5(line);
			ret[0] += prec5[0];
			line = prec5[1];

			var prec6 = Prec6(line);
			ret[0] += prec6[0];
			line = prec6[1];

			var prec7 = Prec7(line);
			ret[0] += prec7[0];
			line = prec7[1];

			var prec10 = Prec10(line);
			ret[0] += prec10[0];
			line = prec10[1];

			var prec11 = Prec11(line);
			ret[0] += prec11[0];
			line = prec11[1];

			var prec12 = Prec12(line);
			ret[0] += prec12[0];
			line = prec12[1];

			ret[0] = ret[0].Replace("[-1];\n", "").Replace("[-1.0f];\n", "");
			ret[1] = line;

			return ret;
		}

		static string[] Prec2(string line)
		{
			var oldLine = line;
			var ret = new string[2] {"", ""};

			Match match;
			while ((match = REPrec2.Match(line)).Success)
			{
				var op = match.Groups[2].Value;
				var arg = match.Groups[3].Value;

				int iarg;
				if (int.TryParse(arg, out iarg))
				{
					switch (op)
					{
						case ("~"):
							line = line.Remove(
								match.Groups[1].Index,
								Math.Min(match.Groups[1].Length, line.Length - match.Groups[1].Index)
							).Insert(match.Groups[1].Index,
								(iarg != 0 ? 0 : 1).ToString()
							);
							break;
					}
				}
				else
				{
					line = line.Remove(
						match.Groups[1].Index,
						Math.Min(match.Groups[1].Length, line.Length - match.Groups[1].Index)
					).Insert(match.Groups[1].Index, "[-1]");
					switch (op)
					{
						case ("~"):
							ret[0] += arg + ";\n" + Inst("not") + "();\n";
							break;
						case ("-"):
							ret[0] += "0;\n" + arg + ";\n" + Inst("sub") + "();\n";
							break;
					}
				}
			}

			ret[1] = line;
			return ret;
		}


		static string[] Prec3(string line)
		{
			var oldLine = line;
			var ret = new string[2] {"", ""};

			Match match;
			while ((match = REPrec3.Match(line)).Success)
			{
				var arg1 = match.Groups[2].Value;
				var op = match.Groups[3].Value;
				var arg2 = match.Groups[4].Value;

				line = line
					.Remove(
						match.Groups[1].Index,
						Math.Min(match.Groups[1].Length, line.Length - match.Groups[1].Index)
					);

				int iarg1, iarg2;
				if (int.TryParse(arg1, out iarg1) && int.TryParse(arg2, out iarg2))
				{

					switch (op)
					{
						case ("*"):
							line = line.Insert(
								match.Groups[1].Index,
								(iarg1 * iarg2).ToString()
							);
							break;
						case ("/"):
							line = line.Insert(
								match.Groups[1].Index,
								(iarg1 / iarg2).ToString()
							);
							break;
						case ("%"):
							line = line.Insert(
								match.Groups[1].Index,
								(iarg1 % iarg2).ToString()
							);
							break;
					}
				}
				else
				{
					float farg1, farg2;
					if (float.TryParse(STF(arg1), out farg1) && float.TryParse(STF(arg2), out farg2))
					{

						switch (op)
						{
							case ("*"):
								line = line.Insert(
									match.Groups[1].Index,
									FTS(farg1 * farg2)
								);
								break;
							case ("/"):
								line = line.Insert(
									match.Groups[1].Index,
									FTS(farg1 / farg2)
								);
								break;
							case ("%"):
								line = line.Insert(
									match.Groups[1].Index,
									FTS(farg1 % farg2)
								);
								break;
						}
					}
					else
					{
						if (
							((arg1[0] == '$' || (!arg1.EndsWith(".0f]") && arg1.EndsWith("]"))) && (arg2[0] == '$' || (!arg2.EndsWith(".0f]") && arg2.EndsWith("]")) || (!arg2.EndsWith("f") && int.TryParse(arg2, out iarg2)))) ||
							((arg2[0] == '$' || (!arg2.EndsWith(".0f]") && arg2.EndsWith("]"))) && (arg1[0] == '$' || (!arg1.EndsWith(".0f]") && arg1.EndsWith("]")) || (!arg1.EndsWith("f") && int.TryParse(arg1, out iarg1))))
						)
						{
							if (arg2 == "[-1]")
								arg2 = "[-2]";
							line = line.Insert(
								match.Groups[1].Index,
								"[-1]"
							);
							switch (op)
							{
								case ("*"):
									ret[0] += arg1 + ";\n" + arg2 + ";\n" + Inst("mul") + "();\n";
									break;
								case ("/"):
									ret[0] += arg1 + ";\n" + arg2 + ";\n" + Inst("div") + "();\n";
									break;
								case ("%"):
									ret[0] += arg1 + ";\n" + arg2 + ";\n" + Inst("mod") + "();\n";
									break;
							}
						}
						else
							if (
								((arg1[0] == '%' || arg1.EndsWith(".0f]")) && (arg2[0] == '%' || arg2.EndsWith(".0f]") || (arg2.EndsWith("f") && float.TryParse(STF(arg2), out farg2)))) ||
								((arg2[0] == '%' || arg2.EndsWith(".0f]")) && (arg1[0] == '%' || arg1.EndsWith(".0f]") || (arg1.EndsWith("f") && float.TryParse(STF(arg1), out farg1))))
							)
							{
								if (arg2 == "[-1.0f]")
									arg2 = "[-2.0f]";
								line = line.Insert(
									match.Groups[1].Index,
									"[-1.0f]"
								);
								switch (op)
								{
									case ("*"):
										ret[0] += arg1 + ";\n" + arg2 + ";\n" + Inst("mulf") + "();\n";
										break;
									case ("/"):
										ret[0] += arg1 + ";\n" + arg2 + ";\n" + Inst("divf") + "();\n";
										break;
									case ("%"):
										throw new Exception("Modular division with float type near \"" + ECLScriptReader.CurLine + "\" (line " + (ECLScriptReader.CurLineNumber + 1) + ")");
								}
							}
							else
								throw new Exception("Invalid left- or right-hand operand for '" + op + "' near \"" + ECLScriptReader.CurLine + "\" (line " + (ECLScriptReader.CurLineNumber + 1) + ")");
					}
				}
			}

			ret[1] = line;
			return ret;
		}


		static string[] Prec4(string line)
		{
			var oldLine = line;
			var ret = new string[2] {"", ""};

			Match match;
			while ((match = REPrec4.Match(line)).Success)
			{
				var arg1 = match.Groups[2].Value;
				var op = match.Groups[3].Value;
				var arg2 = match.Groups[4].Value;
				
				line = line
					.Remove(
						match.Groups[1].Index,
						Math.Min(match.Groups[1].Length, line.Length - match.Groups[1].Index)
					);

				int iarg1, iarg2;
				if (int.TryParse(arg1, out iarg1) && int.TryParse(arg2, out iarg2))
				{

					switch (op)
					{
						case ("+"):
							line = line.Insert(
								match.Groups[1].Index,
								(iarg1 + iarg2).ToString()
							);
							break;
						case ("-"):
							line = line.Insert(
								match.Groups[1].Index,
								(iarg1 - iarg2).ToString()
							);
							break;
					}
				}
				else
				{
					float farg1, farg2;
					if (float.TryParse(STF(arg1), out farg1) && float.TryParse(STF(arg2), out farg2))
					{

						switch (op)
						{
							case ("+"):
								line = line.Insert(
									match.Groups[1].Index,
									FTS(farg1 + farg2)
								);
								break;
							case ("-"):
								line = line.Insert(
									match.Groups[1].Index,
									FTS(farg1 - farg2)
								);
								break;
						}
					}
					else
					{
						if (
							((arg1[0] == '$' || (!arg1.EndsWith(".0f]") && arg1.EndsWith("]"))) && (arg2[0] == '$' || (!arg2.EndsWith(".0f]") && arg2.EndsWith("]")) || (!arg2.EndsWith("f") && int.TryParse(arg2, out iarg2)))) ||
							((arg2[0] == '$' || (!arg2.EndsWith(".0f]") && arg2.EndsWith("]"))) && (arg1[0] == '$' || (!arg1.EndsWith(".0f]") && arg1.EndsWith("]")) || (!arg1.EndsWith("f") && int.TryParse(arg1, out iarg1))))
						)
						{
							if (arg2 == "[-1]")
								arg2 = "[-2]";
							line = line.Insert(
								match.Groups[1].Index,
								"[-1]"
							);
							switch (op)
							{
								case ("+"):
									ret[0] += arg1 + ";\n" + arg2 + ";\n" + Inst("add") + "();\n";
									break;
								case ("-"):
									ret[0] += arg1 + ";\n" + arg2 + ";\n" + Inst("sub") + "();\n";
									break;
							}
						}
						else
							if (
								((arg1[0] == '%' || arg1.EndsWith(".0f]")) && (arg2[0] == '%' || arg2.EndsWith(".0f]") || (arg2.EndsWith("f") && float.TryParse(STF(arg2), out farg2)))) ||
								((arg2[0] == '%' || arg2.EndsWith(".0f]")) && (arg1[0] == '%' || arg1.EndsWith(".0f]") || (arg1.EndsWith("f") && float.TryParse(STF(arg1), out farg1))))
							)
							{
								if (arg2 == "[-1.0f]")
									arg2 = "[-2.0f]";
								line = line.Insert(
									match.Groups[1].Index,
									"[-1.0f]"
								);
								switch (op)
								{
									case ("+"):
										ret[0] += arg1 + ";\n" + arg2 + ";\n" + Inst("addf") + "();\n";
										break;
									case ("-"):
										ret[0] += arg1 + ";\n" + arg2 + ";\n" + Inst("subf") + "();\n";
										break;
								}
							}
							else
								throw new Exception("Invalid left- or right-hand operand for '" + op + "' near \"" + ECLScriptReader.CurLine + "\" (line " + (ECLScriptReader.CurLineNumber + 1) + ")");
					}
				}
			}

			ret[1] = line;
			return ret;
		}


		static string[] Prec5(string line)
		{
			var oldLine = line;
			var ret = new string[2] {"", ""};

			Match match;
			while ((match = REPrec5.Match(line)).Success)
			{
				var arg1 = match.Groups[2].Value;
				var op = match.Groups[3].Value;
				var arg2 = match.Groups[4].Value;

				line = line
					.Remove(
						match.Groups[1].Index,
						Math.Min(match.Groups[1].Length, line.Length - match.Groups[1].Index)
					);

				int iarg1, iarg2;
				if (int.TryParse(arg1, out iarg1) && int.TryParse(arg2, out iarg2))
				{

					switch (op)
					{
						case ("<<"):
							line = line.Insert(
								match.Groups[1].Index,
								(iarg1 << iarg2).ToString()
							);
							break;
						case (">>"):
							line = line.Insert(
								match.Groups[1].Index,
								(iarg1 >> iarg2).ToString()
							);
							break;
					}
				}
				else
				{
					throw new Exception("Bitwise operator \"" + op + "\" with non-\"int\" type operand(s) near \"" + ECLScriptReader.CurLine + "\" (line " + (ECLScriptReader.CurLineNumber + 1) + ")");
				}
			}

			ret[1] = line;
			return ret;
		}


		static string[] Prec6(string line)
		{
			var oldLine = line;
			var ret = new string[2] {"", ""};

			Match match;
			while ((match = REPrec6.Match(line)).Success)
			{
				var arg1 = match.Groups[2].Value;
				var op = match.Groups[3].Value;
				var arg2 = match.Groups[4].Value;

				line = line
					.Remove(
						match.Groups[1].Index,
						Math.Min(match.Groups[1].Length, line.Length - match.Groups[1].Index)
					);

				int iarg1, iarg2;
				if (int.TryParse(arg1, out iarg1) && int.TryParse(arg2, out iarg2))
				{

					switch (op)
					{
						case ("<"):
							line = line.Insert(
								match.Groups[1].Index,
								(iarg1 < iarg2 ? 1 : 0).ToString()
							);
							break;
						case ("<="):
							line = line.Insert(
								match.Groups[1].Index,
								(iarg1 <= iarg2 ? 1 : 0).ToString()
							);
							break;
						case (">"):
							line = line.Insert(
								match.Groups[1].Index,
								(iarg1 > iarg2 ? 1 : 0).ToString()
							);
							break;
						case (">="):
							line = line.Insert(
								match.Groups[1].Index,
								(iarg1 >= iarg2 ? 1 : 0).ToString()
							);
							break;
					}
				}
				else
				{
					float farg1, farg2;
					if (float.TryParse(STF(arg1), out farg1) && float.TryParse(STF(arg2), out farg2))
					{

						switch (op)
						{
							case ("<"):
								line = line.Insert(
									match.Groups[1].Index,
									(farg1 < farg2 ? 1 : 0).ToString()
								);
								break;
							case ("<="):
								line = line.Insert(
									match.Groups[1].Index,
									(farg1 <= farg2 ? 1 : 0).ToString()
								);
								break;
							case (">"):
								line = line.Insert(
									match.Groups[1].Index,
									(farg1 > farg2 ? 1 : 0).ToString()
								);
								break;
							case (">="):
								line = line.Insert(
									match.Groups[1].Index,
									(farg1 >= farg2 ? 1 : 0).ToString()
								);
								break;
						}
					}
					else
					{
						if (
							((arg1[0] == '$' || (!arg1.EndsWith(".0f]") && arg1.EndsWith("]"))) && (arg2[0] == '$' || (!arg2.EndsWith(".0f]") && arg2.EndsWith("]")) || (!arg2.EndsWith("f") && int.TryParse(arg2, out iarg2)))) ||
							((arg2[0] == '$' || (!arg2.EndsWith(".0f]") && arg2.EndsWith("]"))) && (arg1[0] == '$' || (!arg1.EndsWith(".0f]") && arg1.EndsWith("]")) || (!arg1.EndsWith("f") && int.TryParse(arg1, out iarg1))))
						)
						{
							if (arg2 == "[-1]")
								arg2 = "[-2]";
							line = line.Insert(
								match.Groups[1].Index,
								"[-1]"
							);
							switch (op)
							{
								case ("<"):
									ret[0] += arg1 + ";\n" + arg2 + ";\n" + Inst("lt") + "();\n";
									break;
								case ("<="):
									ret[0] += arg1 + ";\n" + arg2 + ";\n" + Inst("lteq") + "();\n";
									break;
								case (">"):
									ret[0] += arg1 + ";\n" + arg2 + ";\n" + Inst("gt") + "();\n";
									break;
								case (">="):
									ret[0] += arg1 + ";\n" + arg2 + ";\n" + Inst("gteq") + "();\n";
									break;
							}
						}
						else
							if (
								((arg1[0] == '%' || arg1.EndsWith(".0f]")) && (arg2[0] == '%' || arg2.EndsWith(".0f]") || (arg2.EndsWith("f") && float.TryParse(STF(arg2), out farg2)))) ||
								((arg2[0] == '%' || arg2.EndsWith(".0f]")) && (arg1[0] == '%' || arg1.EndsWith(".0f]") || (arg1.EndsWith("f") && float.TryParse(STF(arg1), out farg1))))
							)
							{
								if (arg2 == "[-1.0f]")
									arg2 = "[-2.0f]";
								line = line.Insert(
									match.Groups[1].Index,
									"[-1.0f]"
								);
								switch (op)
								{
									case ("<"):
										ret[0] += arg1 + ";\n" + arg2 + ";\n" + Inst("ltf") + "();\n";
										break;
									case ("<="):
										ret[0] += arg1 + ";\n" + arg2 + ";\n" + Inst("lteqf") + "();\n";
										break;
									case (">"):
										ret[0] += arg1 + ";\n" + arg2 + ";\n" + Inst("gtf") + "();\n";
										break;
									case (">="):
										ret[0] += arg1 + ";\n" + arg2 + ";\n" + Inst("gteqf") + "();\n";
										break;
								}
							}
							else
								throw new Exception("Invalid left- or right-hand operand for '" + op + "' near \"" + ECLScriptReader.CurLine + "\" (line " + (ECLScriptReader.CurLineNumber + 1) + ")");
					}
				}
			}

			ret[1] = line;
			return ret;
		}


		static string[] Prec7(string line)
		{
			var oldLine = line;
			var ret = new string[2] {"", ""};

			Match match;
			while ((match = REPrec7.Match(line)).Success)
			{
				var arg1 = match.Groups[2].Value;
				var op = match.Groups[3].Value;
				var arg2 = match.Groups[4].Value;

				line = line
					.Remove(
						match.Groups[1].Index,
						Math.Min(match.Groups[1].Length, line.Length - match.Groups[1].Index)
					);

				int iarg1, iarg2;
				if (int.TryParse(arg1, out iarg1) && int.TryParse(arg2, out iarg2))
				{

					switch (op)
					{
						case ("=="):
							line = line.Insert(
								match.Groups[1].Index,
								(iarg1 == iarg2 ? 1 : 0).ToString()
							);
							break;
						case ("!="):
							line = line.Insert(
								match.Groups[1].Index,
								(iarg1 != iarg2 ? 1 : 0).ToString()
							);
							break;
					}
				}
				else
				{
					float farg1, farg2;
					if (float.TryParse(STF(arg1), out farg1) && float.TryParse(STF(arg2), out farg2))
					{

						switch (op)
						{
							case ("=="):
								line = line.Insert(
									match.Groups[1].Index,
									(farg1 == farg2 ? 1 : 0).ToString()
								);
								break;
							case ("!="):
								line = line.Insert(
									match.Groups[1].Index,
									(farg1 != farg2 ? 1 : 0).ToString()
								);
								break;
						}
					}
					else
					{
						if (
							((arg1[0] == '$' || (!arg1.EndsWith(".0f]") && arg1.EndsWith("]"))) && (arg2[0] == '$' || (!arg2.EndsWith(".0f]") && arg2.EndsWith("]")) || (!arg2.EndsWith("f") && int.TryParse(arg2, out iarg2)))) ||
							((arg2[0] == '$' || (!arg2.EndsWith(".0f]") && arg2.EndsWith("]"))) && (arg1[0] == '$' || (!arg1.EndsWith(".0f]") && arg1.EndsWith("]")) || (!arg1.EndsWith("f") && int.TryParse(arg1, out iarg1))))
						)
						{
							if (arg2 == "[-1]")
								arg2 = "[-2]";
							line = line.Insert(
								match.Groups[1].Index,
								"[-1]"
							);
							switch (op)
							{
								case ("=="):
									ret[0] += arg1 + ";\n" + arg2 + ";\n" + Inst("eq") + "();\n";
									break;
								case ("!="):
									ret[0] += arg1 + ";\n" + arg2 + ";\n" + Inst("neq") + "();\n";
									break;
							}
						}
						else
							if (
								((arg1[0] == '%' || arg1.EndsWith(".0f]")) && (arg2[0] == '%' || arg2.EndsWith(".0f]") || (arg2.EndsWith("f") && float.TryParse(STF(arg2), out farg2)))) ||
								((arg2[0] == '%' || arg2.EndsWith(".0f]")) && (arg1[0] == '%' || arg1.EndsWith(".0f]") || (arg1.EndsWith("f") && float.TryParse(STF(arg1), out farg1))))
							)
							{
								if (arg2 == "[-1.0f]")
									arg2 = "[-2.0f]";
								line = line.Insert(
									match.Groups[1].Index,
									"[-1.0f]"
								);
								switch (op)
								{
									case ("=="):
										ret[0] += arg1 + ";\n" + arg2 + ";\n" + Inst("eqf") + "();\n";
										break;
									case ("!="):
										ret[0] += arg1 + ";\n" + arg2 + ";\n" + Inst("neqf") + "();\n";
										break;
								}
							}
							else
								throw new Exception("Invalid left- or right-hand operand for '" + op + "' near \"" + ECLScriptReader.CurLine + "\" (line " + (ECLScriptReader.CurLineNumber + 1) + ")");
					}
				}
			}

			ret[1] = line;
			return ret;
		}


		static string[] Prec10(string line)
		{
			var oldLine = line;
			var ret = new string[2] {"", ""};

			Match match;
			while ((match = REPrec10.Match(line)).Success)
			{
				var arg1 = match.Groups[2].Value;
				var arg2 = match.Groups[3].Value;

				int iarg1, iarg2;
				if (int.TryParse(arg1, out iarg1) && int.TryParse(arg2, out iarg2))
				{
					line = line
						.Remove(
							match.Groups[1].Index,
							Math.Min(match.Groups[1].Length, line.Length - match.Groups[1].Index)
						)
						.Insert(
							match.Groups[1].Index,
							(iarg1 | iarg2).ToString()
						);
				}
				else
				{
					throw new Exception("Bitwise operator \"|\" with non-\"int\" type operand(s) near \"" + ECLScriptReader.CurLine + "\" (line " + (ECLScriptReader.CurLineNumber + 1) + ")");
				}
			}

			ret[1] = line;
			return ret;
		}


		static string[] Prec11(string line)
		{
			var oldLine = line;
			var ret = new string[2] {"", ""};

			Match match;
			while ((match = REPrec11.Match(line)).Success)
			{
				var arg1 = match.Groups[2].Value;
				var op = match.Groups[3].Value;
				var arg2 = match.Groups[4].Value;

				line = line
					.Remove(
						match.Groups[1].Index,
						Math.Min(match.Groups[1].Length, line.Length - match.Groups[1].Index)
					);

				int iarg1, iarg2;
				if (int.TryParse(arg1, out iarg1) && int.TryParse(arg2, out iarg2))
				{

					switch (op)
					{
						case ("&&"):
							line = line.Insert(
								match.Groups[1].Index,
								(iarg1 == 1 && iarg2 == 1 ? 1 : 0).ToString()
							);
							break;
					}
				}
				else
				{
					float farg1, farg2;
					if (float.TryParse(STF(arg1), out farg1) && float.TryParse(STF(arg2), out farg2))
					{

						switch (op)
						{
							case ("&&"):
								line = line.Insert(
									match.Groups[1].Index,
									(farg1 != 0f && farg2 != 0f ? 1 : 0).ToString()
								);
								break;
						}
					}
					else
					{
						if (
							((arg1[0] == '$' || (!arg1.EndsWith(".0f]") && arg1.EndsWith("]"))) && (arg2[0] == '$' || (!arg2.EndsWith(".0f]") && arg2.EndsWith("]")) || (!arg2.EndsWith("f") && int.TryParse(arg2, out iarg2)))) ||
							((arg2[0] == '$' || (!arg2.EndsWith(".0f]") && arg2.EndsWith("]"))) && (arg1[0] == '$' || (!arg1.EndsWith(".0f]") && arg1.EndsWith("]")) || (!arg1.EndsWith("f") && int.TryParse(arg1, out iarg1))))
						)
						{
							if (arg2 == "[-1]")
								arg2 = "[-2]";
							line = line.Insert(
								match.Groups[1].Index,
								"[-1]"
							);
							switch (op)
							{
								case ("&&"):
									ret[0] += arg1 + ";\n" + arg2 + ";\n" + Inst("and") + "();\n";
									break;
							}
						}
						else
							if (
								((arg1[0] == '%' || arg1.EndsWith(".0f]")) && (arg2[0] == '%' || arg2.EndsWith(".0f]") || (arg2.EndsWith("f") && float.TryParse(STF(arg2), out farg2)))) ||
								((arg2[0] == '%' || arg2.EndsWith(".0f]")) && (arg1[0] == '%' || arg1.EndsWith(".0f]") || (arg1.EndsWith("f") && float.TryParse(STF(arg1), out farg1))))
							)
							{
								if (arg2 == "[-1.0f]")
									arg2 = "[-2.0f]";
								line = line.Insert(
									match.Groups[1].Index,
									"[-1.0f]"
								);
								switch (op)
								{
									case ("&&"):
										ret[0] += arg1 + ";\n0.0f;\n" + Inst("neqf") + "();\n" + arg2 + ";\n0.0f;\n" + Inst("neqf") + "();\n" + Inst("and") + "();\n";
										break;
								}
							}
							else
								throw new Exception("Invalid left- or right-hand operand for '" + op + "' near \"" + ECLScriptReader.CurLine + "\" (line " + (ECLScriptReader.CurLineNumber + 1) + ")");
					}
				}
			}

			ret[1] = line;
			return ret;
		}


		static string[] Prec12(string line)
		{
			var oldLine = line;
			var ret = new string[2] {"", ""};

			Match match;
			while ((match = REPrec12.Match(line)).Success)
			{
				var arg1 = match.Groups[2].Value;
				var op = match.Groups[3].Value;
				var arg2 = match.Groups[4].Value;

				line = line
					.Remove(
						match.Groups[1].Index,
						Math.Min(match.Groups[1].Length, line.Length - match.Groups[1].Index)
					);

				int iarg1, iarg2;
				if (int.TryParse(arg1, out iarg1) && int.TryParse(arg2, out iarg2))
				{

					switch (op)
					{
						case ("||"):
							line = line.Insert(
								match.Groups[1].Index,
								(iarg1 == 1 || iarg2 == 1 ? 1 : 0).ToString()
							);
							break;
					}
				}
				else
				{
					float farg1, farg2;
					if (float.TryParse(STF(arg1), out farg1) && float.TryParse(STF(arg2), out farg2))
					{

						switch (op)
						{
							case ("||"):
								line = line.Insert(
									match.Groups[1].Index,
									(farg1 != 0f || farg2 != 0f ? 1 : 0).ToString()
								);
								break;
						}
					}
					else
					{
						if (
							((arg1[0] == '$' || (!arg1.EndsWith(".0f]") && arg1.EndsWith("]"))) && (arg2[0] == '$' || (!arg2.EndsWith(".0f]") && arg2.EndsWith("]")) || (!arg2.EndsWith("f") && int.TryParse(arg2, out iarg2)))) ||
							((arg2[0] == '$' || (!arg2.EndsWith(".0f]") && arg2.EndsWith("]"))) && (arg1[0] == '$' || (!arg1.EndsWith(".0f]") && arg1.EndsWith("]")) || (!arg1.EndsWith("f") && int.TryParse(arg1, out iarg1))))
						)
						{
							if (arg2 == "[-1]")
								arg2 = "[-2]";
							line = line.Insert(
								match.Groups[1].Index,
								"[-1]"
							);
							switch (op)
							{
								case ("||"):
									ret[0] += arg1 + ";\n" + arg2 + ";\n" + Inst("or") + "();\n";
									break;
							}
						}
						else
							if (
								((arg1[0] == '%' || arg1.EndsWith(".0f]")) && (arg2[0] == '%' || arg2.EndsWith(".0f]") || (arg2.EndsWith("f") && float.TryParse(STF(arg2), out farg2)))) ||
								((arg2[0] == '%' || arg2.EndsWith(".0f]")) && (arg1[0] == '%' || arg1.EndsWith(".0f]") || (arg1.EndsWith("f") && float.TryParse(STF(arg1), out farg1))))
							)
							{
								if (arg2 == "[-1.0f]")
									arg2 = "[-2.0f]";
								line = line.Insert(
									match.Groups[1].Index,
									"[-1.0f]"
								);
								switch (op)
								{
									case ("||"):
										ret[0] += arg1 + ";\n0.0f;\n" + Inst("neqf") + "();\n" + arg2 + ";\n0.0f;\n" + Inst("neqf") + "();\n" + Inst("or") + "();\n";
										break;
								}
							}
							else
								throw new Exception("Invalid left- or right-hand operand for '" + op + "' near \"" + ECLScriptReader.CurLine + "\" (line " + (ECLScriptReader.CurLineNumber + 1) + ")");
					}
				}
			}

			ret[1] = line;
			return ret;
		}


		static string STF(string s)
		{
			if (s.EndsWith("f"))
				s = s.Remove(s.Length - 1);
			return s;
		}
		static string FTS(float f)
		{
			return f + "f";
		}
	}
}

