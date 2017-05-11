using System;
using System.Text.RegularExpressions;
using System.Collections.Generic;

namespace THModTools
{
	public static class ANMScriptWriter
	{
		public static Dictionary<int, Dictionary<int, string>> Inst = new Dictionary<int, Dictionary<int, string>>()
		{
			{
				15,
				new Dictionary<int, string>()
				{
					{ 1, "end" },
					{ 2, "return" },
					{ 200, "jump" },
					{ 300, "setSprite" },
					{ 301, "setSpriteRandom" },
					{ 302, "setSpriteMode" },
					{ 303, "setBlendMode" },
					{ 304, "setLayer" },
					{ 308, "flip" },
					{ 400, "setPosition" },
					{ 401, "setAngle" },
					{ 402, "setScale" },
					{ 404, "setColor" },
					{ 407, "move" },
					{ 408, "changeColor" },
					{ 409, "changeAlpha" },
					{ 410, "changeAngle" },
					{ 412, "changeScale" },
                    { 421, "setCenter" },
					{ 425, "setScrollY" },
					{ 426, "setScrollX" }
				}
			},
			{
				16,
				new Dictionary<int, string>()
				{
					{ 1, "end" },
					{ 2, "return" },
					{ 200, "jump" },
					{ 300, "setSprite" },
					{ 301, "setSpriteRandom" },
					{ 302, "setSpriteMode" },
					{ 303, "setBlendMode" },
					{ 304, "setLayer" },
					{ 308, "flip" },
					{ 400, "setPosition" },
					{ 401, "setAngle" },
					{ 402, "setScale" },
					{ 404, "setColor" },
					{ 407, "move" },
					{ 408, "changeColor" },
					{ 409, "changeAlpha" },
					{ 410, "changeAngle" },
					{ 412, "changeScale" },
                    { 421, "setCenter" },
					{ 425, "setScrollY" },
					{ 426, "setScrollX" }
				}
			}
		};

		static Regex REInst = new Regex(@"(Instruction:\s*\d+\s+\d+)\s+(\d+)\s*(.*)", RegexOptions.Compiled);

		public static string[] WriteScript(string[] input)
		{
			var output = new List<string>();

			for (int lnum = 0; lnum < input.Length; lnum++)
			{
				var line = input[lnum];

				if (Inst.ContainsKey(THTK.Version))
				{
					line = REInst.Replace(line, new MatchEvaluator((match) =>
					{
						int instID;
						if (int.TryParse(match.Groups[2].Value, out instID))
						{
							if (Inst[THTK.Version].ContainsKey(instID))
							{
								return match.Groups[1].Value + " " + Inst[THTK.Version][instID] +
									" " + match.Groups[3];
							}

							return match.Groups[1].Value + " ins_" + instID +
								" " + match.Groups[3];
						}
						
						return match.Value;
					}));
				}

				output.Add(line);
			}

			return output.ToArray();
		}
	}
}

