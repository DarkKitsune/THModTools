using System;
using System.Text.RegularExpressions;
using System.Collections.Generic;

namespace THModTools
{
	public static class ANMScriptReader
	{
		static Dictionary<int, Dictionary<string, int>> Inst = new Dictionary<int, Dictionary<string, int>>();

		static Regex REInst = new Regex(@"(Instruction:\s*\d+\s+\d+)\s+(\w+)\s*(.*)", RegexOptions.Compiled);

		static ANMScriptReader()
		{
			Console.Write("Generating instruction conversion for ANMScriptReader...");
			foreach (var verKv in ANMScriptWriter.Inst)
			{
				Inst[verKv.Key] = new Dictionary<string, int>();
				foreach (var instKv in verKv.Value)
				{
					Inst[verKv.Key][instKv.Value] = instKv.Key; 
				}
			}
			Console.WriteLine(" Done");
		}

		public static string[] ReadScript(string[] input)
		{
			var output = new List<string>();

			for (int lnum = 0; lnum < input.Length; lnum++)
			{
				var line = input[lnum];

				if (Inst.ContainsKey(THTK.Version))
				{
					line = REInst.Replace(line, new MatchEvaluator( (match) =>
					{
						var instName = match.Groups[2].Value;

						if (Inst[THTK.Version].ContainsKey(instName))
						{
							return match.Groups[1] + " " + Inst[THTK.Version][instName] + " " + match.Groups[3];
						}

						if (instName.StartsWith("ins_"))
						{
							int instID;
							if (int.TryParse(instName.Replace("ins_", ""), out instID))
							{
								return match.Groups[1] + " " + instID + " " + match.Groups[3];
							}
						}

						throw new Exception("Unknown ANM instruction \"" + instName + "\" near \"" + line + "\" at line " + (lnum + 1));
					}));
				}

				output.Add(line);
			}

			return output.ToArray();
		}
	}
}

