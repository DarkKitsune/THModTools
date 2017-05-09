using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace THModTools
{
	public static class ECLScriptReader
	{
		public static Dictionary<int, Dictionary<string, int>> Inst = new Dictionary<int, Dictionary<string, int>>();

		public static string GetInst(string name)
		{
			return "ins_" + Inst[THTK.Version][name];
		}

		static Dictionary<string, string> ECL10Constants = new Dictionary<string, string>()
		{
			{ "FLAG_NOHITBOX", "1"},
			{ "FLAG_NOKILLBOX", "2"},
			{ "FLAG_NOBOUNDS_H", "4"},
			{ "FLAG_NOBOUNDS_V", "8"},
			{ "FLAG_SURVIVAL", "16"},
			{ "FLAG_INVISIBLE", "32"},
			{ "FLAG_BOSS", "64"},
			{ "FLAG_PERSISTENTOBJECT", "128"},
			{ "FLAG_UNKNOWN_256", "256"},
			{ "FLAG_GRAZEABLE", "512"},
			{ "FLAG_UNKNOWN_1024", "1024"},
			{ "FLAG_BOMB", "2048"},
			{ "FLAG_UNKNOWN_4096", "4096"},
			{ "FLAG_NOTIMESCALE", "8192"},

			{ "TWEEN_LINEAR", "0" },
			{ "TWEEN_ACCELERATE", "1" },
			{ "TWEEN_DECELERATE", "4" },
			{ "TWEEN_SMOOTH", "9" },

			{ "TYPE_AIMED", "0" },
			{ "TYPE_NORMAL", "1" },
			{ "TYPE_CIRCLE2_AIMED", "4" },
			{ "TYPE_CIRCLE2", "5" },
			{ "TYPE_CIRCLE_AIMED", "2" },
			{ "TYPE_CIRCLE", "3" },
			{ "TYPE_RANDOM", "6" },
			{ "TYPE_TRIANGLE_CIRCLE_AIMED", "9" },
			{ "TYPE_TRIANGLE_CIRCLE", "10" },
			{ "TYPE_UP_DOWN", "11" },
			{ "TYPE_UNKNOWN_12", "12" },

			{ "PATTERN_BOOST", "1" },
			{ "PATTERN_EFFECT", "2" },
			{ "PATTERN_ACCELERATE_SPIN", "8" },
			{ "PATTERN_ACCELERATE", "4" },
			{ "PATTERN_STOP_CHANGE_MOTION", "16" },
			{ "PATTERN_UNKNOWN_32", "32" },
			{ "PATTERN_BOUNCE", "64" },
			{ "PATTERN_NODELETE", "128" },
			{ "PATTERN_UNKNOWN_256", "256" },
			{ "PATTERN_CHANGE_GRAPHIC", "512" },
			{ "PATTERN_DELETE", "1024" },
			{ "PATTERN_SOUND", "2048" },
			{ "PATTERN_WRAP", "4096" },
			{ "PATTERN_CREATE_CHILD", "8192" },
			{ "PATTERN_CHILD_GRAPHIC", "16384" },
			{ "PATTERN_ENABLE_DETECT", "32768" },
			{ "PATTERN_GOTO", "65536" },
			{ "PATTERN_UNKNOWN_131072", "131072" },
			{ "PATTERN_UNKNOWN_262144", "262144" },
			{ "PATTERN_UNKNOWN_524288", "524288" },
			{ "PATTERN_ADDITIVE", "1048576" },
			{ "PATTERN_CHANGE_MOTION", "2097152" },
			{ "PATTERN_CHANGE_SIZE", "4194304" },
			{ "PATTERN_UNKNOWN_8399608", "8399608" },
			{ "PATTERN_CREATE_OBJECT", "16777216" },
			{ "PATTERN_UNKNOWN_33554432", "33554432" },
			{ "PATTERN_DELAY", "67108864" },
			{ "PATTERN_UNKNOWN_134217728", "134217728" },
			{ "PATTERN_UNKNOWN_268435456", "268435456" },
			{ "PATTERN_UNKNOWN_536870912", "536870912" },
			{ "PATTERN_UNKNOWN_1073741824", "1073741824" },
			{ "PATTERN_WAIT", "-2147483648" },

			{ "WALL_UP", "1" },
			{ "WALL_DOWN", "2" },
			{ "WALL_LEFT", "4" },
			{ "WALL_RIGHT", "8" },

			{ "NULLF", "-999999.0f" },
			{ "NULL", "-999999" },
			{ "NO_CHANGE", "-999.0f" },
			{ "AIM_PLAYER", "999.0f" },

			{ "TRUE", "1" },
			{ "FALSE", "0" }
		};

		static Dictionary<int, Dictionary<string, Action<string[]>>> InstArgAction = new Dictionary<int, Dictionary<string, Action<string[]>>>()
		{
			{
				14,
				new Dictionary<string, Action<string[]>>()
				{
					{ "call", CallArgAction },
					{ "task", CallArgAction },
					{ "callID", CallIDArgAction },
					{ "setBossMode", SetBossModeArgAction },
					{ "setAnm", SetAnmArgAction }
				}
			},
			{
				15,
				new Dictionary<string, Action<string[]>>()
				{
					{ "call", CallArgAction },
					{ "task", CallArgAction },
					{ "callID", CallIDArgAction },
					{ "setBossMode", SetBossModeArgAction },
					{ "setAnm", SetAnmArgAction }
				}
			},
			{
				16,
				new Dictionary<string, Action<string[]>>()
				{
					{ "call", CallArgAction },
					{ "task", CallArgAction },
					{ "callID", CallIDArgAction },
					{ "setBossMode", SetBossModeArgAction },
					{ "setAnm", SetAnmArgAction }
				}
			}
		};

		static Dictionary<int, Dictionary<string, MacroFunction>> MacroFunctions = new Dictionary<int, Dictionary<string, MacroFunction>>()
		{
			{
				14,
				new Dictionary<string, MacroFunction>()
				{
					{ "rad", new MacroFunction() 
						{
							Args = new ECLType[] { ECLType.Expression },
							Function = (args) => {
								return args[0] + " * 0.0174533f";
							}
						}
					},
					{ "initAttack", new MacroFunction() 
						{
							Args = new ECLType[] { },
							Function = (args) => {
								return "setBossMode(TRUE);\n" +
									"system.misses = 0;\n" +
									"system.spellsUsed = 0;\n" +
									"system.failed = 1;\n" +
									"deleteChildrenObjects();\n" +
									"startAttack();\n" +
									"createObject(\"Ecl_EtBreak_ni\", 0.0f, 0.0f, 9999, 0, 0);\n" +
									"setCheckpoint(0);\n" +
									"setAnm(-1);\n" +
									"setSprite(1, 106)";
							}
						}
					}
				}
			},
			{
				15,
				new Dictionary<string, MacroFunction>()
				{
					{ "rad", new MacroFunction() 
						{
							Args = new ECLType[] { ECLType.Expression },
							Function = (args) => {
								return args[0] + " * 0.0174533f";
							}
						}
					},
					{ "rgb", new MacroFunction() 
						{
							Args = new ECLType[] { ECLType.Expression, ECLType.Expression, ECLType.Expression },
							Function = (args) => {
								return args[0] + " * 65536 + " + args[1] + " * 256 + " + args[2];
							}
						}
					},
					{ "initAttack", new MacroFunction() 
						{
							Args = new ECLType[] { },
							Function = (args) => {
								return "setBossMode(TRUE);\n" +
									"system.misses = 0;\n" +
									"system.spellsUsed = 0;\n" +
									"system.failed = 1;\n" +
									"deleteChildrenObjects();\n" +
									"startAttack();\n" +
									"createObject(\"Ecl_EtBreak_ni\", 0.0f, 0.0f, 9999, 0, 0)";
							}
						}
					}
				}
			},
			{
				16,
				new Dictionary<string, MacroFunction>()
				{
					{ "rad", new MacroFunction() 
						{
							Args = new ECLType[] { ECLType.Expression },
							Function = (args) => {
								return args[0] + " * 0.0174533f";
							}
						}
					},
					{ "rgb", new MacroFunction() 
						{
							Args = new ECLType[] { ECLType.Expression, ECLType.Expression, ECLType.Expression },
							Function = (args) => {
								return args[0] + " * 65536 + " + args[1] + " * 256 + " + args[2];
							}
						}
					},
					{ "initAttack", new MacroFunction() 
						{
							Args = new ECLType[] { },
							Function = (args) => {
								return "setBossMode(TRUE);\n" +
									"system.misses = 0;\n" +
									"system.spellsUsed = 0;\n" +
									"system.failed = 1;\n" +
									"deleteChildrenObjects();\n" +
									"startAttack();\n" +
									"createObject(\"Ecl_EtBreak_ni\", 0.0f, 0.0f, 9999, 0, 0)";
							}
						}
					}
				}
			}
		};

		static Dictionary<int, Dictionary<string, string>> Constants = new Dictionary<int, Dictionary<string, string>>()
		{
			{
				14,
				ECL10Constants
			},
			{
				15,
				ECL10Constants
			},
			{
				16,
				ECL10Constants
			}
		};

		static Dictionary<int, Dictionary<string, string>> Globals = new Dictionary<int, Dictionary<string, string>>();

		public static Dictionary<string, ECLType> CurVars;
		public static int CurLineNumber = -1;
		public static string CurLine = "";

		static Regex RegExInst = new Regex(@"\b([a-zA-Z_]\w*)\((.*?)\)", RegexOptions.Compiled);
		static Regex RegExUseVar = new Regex(@"\b([a-zA-Z_]\w*)\b(?!\s*\(|:)", RegexOptions.Compiled);
		static Regex RegExSub = new Regex(@"\bfunction ([a-zA-Z_]\w*)\((.*)\)", RegexOptions.Compiled);
		static Regex RegExDefVar = new Regex(@"((?:int|float))\s*([a-zA-Z_]\w*);", RegexOptions.Compiled);
		static Regex RegExDefVarMulti = new Regex(@"((?:int|float))\s*([A-Za-z0-9_,\s]*);", RegexOptions.Compiled);
		static Regex RegExDefVarSet = new Regex(@"((?:int|float))\s*([a-zA-Z_]\w*)\s*=\s*(.*);", RegexOptions.Compiled);
		static Regex RegExLbl = new Regex(@"(\w+):", RegexOptions.Compiled);
		static Regex RegGlobal = new Regex(@"([A-Za-z_]\w*\.[A-Za-z_]\w*)", RegexOptions.Compiled);
		static Regex RegComment = new Regex(@"\/\/.*", RegexOptions.Compiled);
		static Regex RegIf = new Regex(@"\bif\s*(.*)\s*then\b", RegexOptions.Compiled);
		static Regex RegWhile = new Regex(@"\bwhile\s*(.*)\s*do\b", RegexOptions.Compiled);
		static Regex RegConvFloat = new Regex(@"\bfloat\((.*)\)", RegexOptions.Compiled);

		static bool InQuotes(string line, int ind)
		{
			var inside = false;
			for (var i = 0; i < ind; i++)
			{
				if (line[i] == '"')
					inside = !inside;
			}

			return inside;
		}

		static bool IsKeyword(string str)
		{
			return (str == "anim" || str == "ecli" || str == "if" || str == "sub" || str == "function" ||
				str == "unless" || str == "var" || str == "goto" || str == "int" || str == "float" ||
				str == "string" || str == "_SS" || str == "_ff" || str == "_fS" || str == "_Sf" ||
				str == "then" || str == "while" || str == "do" || str == "end");
		}

		static ECLScriptReader()
		{
			Console.Write("Generating instruction conversion for ECLScriptReader...");
			foreach (var verKv in ECLScriptWriter.Inst)
			{
				Inst[verKv.Key] = new Dictionary<string, int>();
				foreach (var instKv in verKv.Value)
				{
					Inst[verKv.Key][instKv.Value] = instKv.Key; 
				}
			}
			Console.WriteLine(" Done");

			Console.Write("Generating global conversion for ECLScriptReader...");
			foreach (var verKv in ECLScriptWriter.Globals)
			{
				Globals[verKv.Key] = new Dictionary<string, string>();
				foreach (var globKv in verKv.Value)
				{
					Globals[verKv.Key][globKv.Value] = globKv.Key; 
				}
			}
			Console.WriteLine(" Done");
		}

		public static string[] ReadScript(string[] inputArr)
		{
			var input = new List<string>(inputArr);
			var labels = new Dictionary<string, int>();

			FirstPass(input);

			for (var lnum = 0; lnum < input.Count; lnum++)
			{
				Match match;
				if ((match = RegExLbl.Match(input[lnum])).Groups[1].Success)
				{
					labels[match.Groups[1].Value] = lnum;
				}
			}


			var output = new List<string>();
			var subLine = -1;
			var curSub = "";
			var curVars = new Dictionary<string, ECLType>();
			CurVars = curVars;

			for (var lnum = 0; lnum < input.Count; lnum++)
			{
				CurLineNumber = lnum;
				CurLine = input[lnum];
				var line = input[lnum];
				var oldLine = line;
				var trimmed = line.TrimStart();
				var prologue = "";

				line = RegComment.Replace(line, new MatchEvaluator((match) =>
				{
					return "";
				}));

				line = line.TrimEnd();

				line = RegConvFloat.Replace(line, new MatchEvaluator((match) =>
				{
					return "_f(" + match.Groups[1].Value + ")";
				}));

				foreach (var macro in MacroFunctions[THTK.Version])
				{
					var func = macro.Value;
					var pattern = macro.Key + @"\((";

					if (func.Args.Length != 0)
					{
						for (int i = 0; i < func.Args.Length - 1; i++)
							pattern += ".*,";
						pattern += ".*";
					}

					pattern += @")\)";

					line = Regex.Replace(line, pattern, new MatchEvaluator( (match) =>
					{
						var argstr = match.Groups[1].Value;
						while (argstr.Contains("  "))
							argstr.Replace("  ", " ");
						var args = argstr.Replace(", ", ",").Split(',');
						if (args.Length == 1 && args[0] == "")
							args = new string[] {};

						return func.Invoke(oldLine, args);
					}));
				}

				if (Globals.ContainsKey(THTK.Version))
				{
					line = RegGlobal.Replace(line, new MatchEvaluator((match) =>
					{
						foreach (var glob in Globals[THTK.Version])
						{
							if (match.Groups[1].Value == glob.Key)
								return glob.Value;
						}
						return match.Value;
					}));
				}

				foreach (var constant in Constants[THTK.Version])
				{
					line = line.Replace(constant.Key, constant.Value);
				}

				if (!trimmed.StartsWith("!") && !trimmed.StartsWith("goto ") && !trimmed.StartsWith("if ") && !trimmed.StartsWith("var "))
				{
					if (subLine != -1 && (trimmed.StartsWith("function ") || lnum == input.Count - 1))
					{
						var varLine = "    var";

						foreach (var vk in curVars)
						{
							varLine += " " + vk.Key;
						}

						output.Insert(subLine + 2, varLine + ";");
					}

					Match subMatch;
					if ((subMatch = RegExSub.Match(line)).Success)
					{
						subLine = output.Count;
						curSub = subMatch.Groups[1].Value;

						curVars.Clear();

						var argstr = subMatch.Groups[2].Value;
						while (argstr.Contains("  "))
							argstr = argstr.Replace("  ", " ");
						var curSubArgsA = subMatch.Groups[2].Value.Replace(", ", ",").Split(',');
						var args = new string[curSubArgsA.Length];
						var i = 0;
						foreach (var arg in curSubArgsA)
						{
							if (arg == "")
								continue;

							if (!arg.Contains(" "))
								throw new Exception("Argument \"" + arg + "\" missing type signature near \"" + CurLine + "\" (line " + (CurLineNumber + 1) + ")");

							var argParts = arg.Split(' ');

							args[i++] = argParts[1];

							if (argParts[0] == "int")
								curVars.Add(argParts[1], ECLType.Int);
							else if (argParts[0] == "float")
								curVars.Add(argParts[1], ECLType.Float);
							else
								throw new Exception("Argument \"" + arg + "\" has invalid type signature near \"" + CurLine + "\" (line " + (CurLineNumber + 1) + ")");
						}

						line = "sub " + curSub + "(" + String.Join(" ", args) + ")";
					}
					else
					{
						if (subLine != -1)
						{
							Match defMatch;
							if ((defMatch = RegExDefVarSet.Match(line)).Success)
							{
								if (!InQuotes(line, defMatch.Index))
								{
									var typeSig = defMatch.Groups[1].Value;
									var varName = defMatch.Groups[2].Value;
									var value = defMatch.Groups[3].Value;

									if (typeSig == "int")
										curVars.Add(varName, ECLType.Int);
									else if (typeSig == "float")
											curVars.Add(varName, ECLType.Float);
										else
										throw new Exception("Variable \"" + varName + "\" has invalid type signature near \"" + CurLine + "\" (line " + (CurLineNumber + 1) + ")");

									if (typeSig == "int")
									{
										if (value.Contains("%") || Regex.Match(value, @"\df").Success)
											throw new Exception("Value was not type \"int\" near \"" + CurLine + "\" (line " + (CurLineNumber + 1) + ")");
										line = varName + " = " + value + ";";
									}
									else if (typeSig == "float")
										{
										if (value.Contains("$") || Regex.Match(value, @"(?<!\.)\b[0-9]+\b(?!\.|f)").Success)
											throw new Exception("Value was not type \"float\" near \"" + CurLine + "\" (line " + (CurLineNumber + 1) + ")");
										line = varName + " = " + value + ";";
										}
								}
							}
							else
							{
								if ((defMatch = RegExDefVar.Match(line)).Success)
								{
									if (!InQuotes(line, defMatch.Index))
									{
										var typeSig = defMatch.Groups[1].Value;
										var varName = defMatch.Groups[2].Value;

										if (typeSig == "int")
											curVars.Add(varName, ECLType.Int);
										else if (typeSig == "float")
												curVars.Add(varName, ECLType.Float);
											else
											throw new Exception("Variable \"" + varName + "\" has invalid type signature near \"" + CurLine + "\" (line " + (CurLineNumber + 1) + ")");

										line = "";
									}
								}
								else
								{
									if ((defMatch = RegExDefVarMulti.Match(line)).Success)
									{
										if (!InQuotes(line, defMatch.Index))
										{
											var typeSig = defMatch.Groups[1].Value;
											var varNames = defMatch.Groups[2].Value.Replace(" ", "").Split(',');

											if (typeSig == "int")
												foreach (var varName in varNames)
													curVars.Add(varName, ECLType.Int);
											else if (typeSig == "float")
												foreach (var varName in varNames)
													curVars.Add(varName, ECLType.Float);
											else
												throw new Exception("Variables have invalid type signature near \"" + CurLine + "\" (line " + (CurLineNumber + 1) + ")");

											line = "";
										}
									}
								}
							}
							line = RegExUseVar.Replace(line, new MatchEvaluator((match) =>
							{
								if (!InQuotes(line, match.Index))
								{
									var varName = match.Groups[1].Value;

									if (IsKeyword(varName) || labels.ContainsKey(varName))
										return varName;

									if (!curVars.ContainsKey(varName))
										throw new Exception("Variable \"" + varName + "\" was never defined near \"" + CurLine + "\" (line " + (CurLineNumber + 1) + ")");

									var type = curVars[varName];
									if (type == ECLType.Float)
										return "%" + varName;
									return "$" + varName;
								}
								return match.Value;
							}
							));
						}
					}
				}

				var expr = Preprocessor.Expression(line);
				prologue += expr[0];
				line = expr[1];

				line = RegExInst.Replace(line, new MatchEvaluator((match) =>
				{
					var inst = match.Groups[1].Value;

					var argstr = match.Groups[2].Value;
					while (argstr.Contains("  "))
						argstr = argstr.Replace("  ", " ");
					argstr.Replace(", ", ",");

					var args = argstr.Split(',');
					if (InstArgAction[THTK.Version].ContainsKey(inst))
					{
						InstArgAction[THTK.Version][inst](args);
					}

					if (Inst[THTK.Version].ContainsKey(inst))
					{
						return GetInst(inst) + "(" + String.Join(", ", args) + ")";
					}
					return match.Groups[1].Value + "(" + String.Join(", ", args) + ")";
				}
				));



				line = prologue + line;
				var lines = line.Replace("\r", "").Split('\n');
				foreach (var ln  in lines)
					if (ln.Trim() != "")
						output.Add(ln);
			}

			FinalPass(output);
			return output.ToArray();
		}


		static void FirstPass(List<string> input)
		{
			for (var i = 0; i < input.Count; i++)
			{
				var line = input[i];

				Match match = RegIf.Match(line);
				if (match.Success)
				{
					int end;
					int depth = 1;
					for (end = i + 1; end < input.Count; end++)
					{
						if (RegIf.Match(input[end]).Success || RegWhile.Match(input[end]).Success)
							depth += 1;
						else if (input[end].Trim() == "end")
								depth -= 1;
						if (depth == 0)
							break;
					}
					if (end >= input.Count)
						throw new Exception("No matching \"end\" for \"if\" near \"" + line + "\"");

					input[end] = "lblif" + i + ":";
					input[i] = match.Groups[1].Value + ";";
					input.Insert(i + 1, "jumpIfNot(lblif" + i + ", 0);");
					i += 1;
				}
				else
				{
					match = RegWhile.Match(line);
					if (match.Success)
					{
						int end;
						int depth = 1;
						for (end = i + 1; end < input.Count; end++)
						{
							if (RegIf.Match(input[end]).Success || RegWhile.Match(input[end]).Success)
								depth += 1;
							else
								if (input[end].Trim() == "end")
									depth -= 1;
							if (depth == 0)
								break;
						}
						if (end >= input.Count)
							throw new Exception("No matching \"end\" for \"while\" near \"" + line + "\"");

						var labelName1 = "lblwhilestart" + i;
						var labelName2 = "lblwhileend" + i;
						input[end] = "goto " + labelName1 + " @ 0;";
						input.Insert(end + 1, labelName2 + ":");
						input[i] = labelName1 + ":";
						input.Insert(++i, match.Groups[1].Value + ";");
						input.Insert(++i, "jumpIfNot(" + labelName2 + ", 0);");
					}
				}
			}
		}

		static void FinalPass(List<string> input)
		{
			for (var i = 0; i < input.Count; i++)
			{
				var line = input[i];

				if (line.Trim().StartsWith("[-1]"))
					input.RemoveAt(i--);
			}
		}


		static void CallArgAction(string[] args)
		{
			for (var i = 1; i < args.Length; i++)
			{
				var arg = args[i];
				if (Regex.Match(arg, @"\d+f").Success || Regex.Match(arg, @"%[A-Za-z_]").Success)
					args[i] = "_ff " + arg;
				else
					args[i] = "_SS " + arg;
			}
		}

		static void CallIDArgAction(string[] args)
		{
			for (var i = 2; i < args.Length; i++)
			{
				var arg = args[i];
				if (Regex.Match(arg, @"\d+f").Success || Regex.Match(arg, @"%[A-Za-z_]").Success)
					args[i] = "_ff " + arg;
				else
					args[i] = "_SS " + arg;
			}
		}

		static void SetAnmArgAction(string[] args)
		{
			int iarg;
			if (int.TryParse(args[0], out iarg))
				args[0] = (iarg + 2).ToString();
		}

		static void SetBossModeArgAction(string[] args)
		{
			if (args[0] == "1")
				args[0] = "0";
			else
				if (args[0] == "0")
					args[0] = "-1";
		}
	}
}

