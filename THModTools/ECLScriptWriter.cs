using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace THModTools
{
	public static class ECLScriptWriter
	{
		static Dictionary<string, string> ECL10Globals = new Dictionary<string, string>() {
			{ "[-9906]", "this.flags" },
			{ "[-9907]", "spell.index" },
			{ "[-9921.0f]", "global.float0" },
			{ "[-9922.0f]", "global.float1" },
			{ "[-9925]", "global.int0" },
			{ "[-9926]", "global.int1" },
			{ "[-9935.0f]", "global.float1" },
			{ "[-9945]", "player.type" },
			{ "[-9946]", "system.objectCount" },
			{ "[-9947]", "system.failed" },
			{ "[-9948]", "system.spellsUsed" },
			{ "[-9949]", "system.misses" },
			{ "[-9954]", "boss.life" },
			{ "[-9958.0f]", "this.angle" },
			{ "[-9959]", "system.difficulty" },
			{ "[-9962.0f]", "this.y" },
			{ "[-9963.0f]", "this.x" },
			{ "[-9964.0f]", "player.y" },
			{ "[-9965.0f]", "player.x" },
			{ "[-9971.0f]", "this.lastAngle" },
			{ "[-9978.0f]", "this.shotDetectRadius" },
			{ "[-9979.0f]", "local.unknownFloat9979" },
			{ "[-9980.0f]", "this.shotPattern14Radius" },
			{ "[-9981.0f]", "local.unknownFloat9981" },
			{ "[-9982]", "local.int0" },
			{ "[-9983]", "local.int1" },
			{ "[-9984]", "local.int2" },
			{ "[-9985]", "this.shotDetected" },
			{ "[-9986]", "system.timedOut" },
			{ "[-9987.0f]", "random.quarterCircle" },
			{ "[-9988]", "system.time" },
			{ "[-9989.0f]", "this.angleToPlayer" },
			{ "[-9990.0f]", "player.y2" },
			{ "[-9991.0f]", "player.y3" },
			{ "[-9994.0f]", "this.y2" },
			{ "[-9995.0f]", "this.x2" },
			{ "[-9996.0f]", "this.y3" },
			{ "[-9997.0f]", "this.x3" },
			{ "[-9998.0f]", "random.halfCircle" },
			{ "[-9999.0f]", "random.quarterPi" },
			{ "[-10000]", "random.int" },
		};

		public static Dictionary<int, Dictionary<int, string>> Inst = new Dictionary<int, Dictionary<int, string>>()
		{
			{
				14,
				new Dictionary<int, string>()
				{
					{ 1, "delete" },
					{ 10, "return" },
					{ 11, "call" },
					{ 12, "jump" },
					{ 13, "jumpIfNot" },
					{ 14, "jumpIf" },
					{ 15, "task" },
					{ 22, "callID" },
					{ 23, "wait" },

					{ 42, "push" },
					{ 43, "pop" },
					{ 44, "pushf" },
					{ 45, "popf" },

					{ 50, "add" },
					{ 51, "addf" },
					{ 52, "sub" },
					{ 53, "subf" },
					{ 54, "mul" },
					{ 55, "mulf" },
					{ 56, "div" },
					{ 57, "divf" },
					{ 58, "mod" },
					{ 59, "eq" },
					{ 60, "eqf" },
					{ 61, "neq" },
					{ 62, "neqf" },
					{ 63, "lt" },
					{ 64, "ltf" },
					{ 65, "lteq" },
					{ 66, "lteqf" },
					{ 67, "gt" },
					{ 68, "gtf" },
					{ 69, "gteq" },
					{ 70, "gteqf" },
					{ 73, "or" },
					{ 74, "and" },

					{ 78, "dec" },

					{ 81, "cosSin" },

					{ 83, "invSub" },
					{ 84, "invSubf" },

					{ 300, "createObject" },
					{ 301, "createObjectAt" },
					{ 304, "createObjectMirrored" },
					{ 309, "createObject2" },
					{ 311, "createObject3" },

					{ 302, "setAnm" },
					{ 303, "setSprite" },
					{ 306, "setSpriteEnemy" },
					{ 307, "effect" },
					{ 308, "effect2" },
					{ 325, "setSpriteColor" },
					{ 328, "setSpriteAlpha" },
					{ 329, "setSpriteScale" },
					{ 330, "changeSpriteScale" },
					{ 334, "attachEffect" },
					{ 336, "attachEffect2" },

					{ 400, "setPosition" },
					{ 401, "move" },
					{ 402, "setPosition2" },
					{ 404, "setMotion" },
					{ 405, "changeMotion" },
					{ 412, "moveRandom" },
					{ 426, "moveCurve" },
					{ 441, "changeAngle" },
					{ 445, "changeSpeed" },

					{ 500, "setHitbox" },
					{ 501, "setKillbox" },
					{ 502, "setFlags" },
					{ 503, "clearFlags" },
					{ 504, "setMovementArea" },
					{ 506, "clearDrops" },
					{ 507, "addDrop" },
					{ 508, "setDropArea" },
					{ 509, "drop" },
					{ 510, "setItem" },
					{ 511, "setLife" },
					{ 512, "setBossMode" },
					{ 513, "startAttack" },
					{ 514, "addAttack" },
					{ 515, "setInvulnerableTime" },
					{ 516, "playSound" },

					{ 518, "startMsg" },
					{ 519, "waitMsgEnd" },
					{ 520, "waitBossEnd" },

					{ 524, "setCheckpoint" },
					{ 525, "deleteChildrenObjects" },

					{ 535, "setByDifficulty" },
					{ 536, "setByDifficultyf" },
					{ 547, "setTimeScale" },
					{ 548, "waitByDifficulty" },

					{ 556, "callOnDeath" },

					{ 600, "createShot" },
					{ 601, "fireShot" },
					{ 602, "setShotGraphic" },
					{ 603, "setShotOffset" },
					{ 604, "setShotAngle" },
					{ 605, "setShotSpeed" },
					{ 606, "setShotCount" },
					{ 607, "setShotType" },
					{ 608, "setShotSound" },
					{ 609, "setShotPattern" },
					{ 610, "setShotPattern2" },
					{ 611, "setShotPattern3" },
					{ 612, "setShotPattern4" },
					{ 613, "deleteShots" },
					{ 615, "itemizeShotsCircle" },
					{ 616, "deleteShotsCircle" },
					{ 624, "setShotSpeedByDifficulty" },
					{ 625, "setShotCountByDifficulty" },
					{ 626, "setShotOffsetPolar" },
					{ 627, "setShotOffsetCircle" },
					{ 640, "setShotCreateObject" },

					{ 629, "setAura" },

					{ 632, "setShotDetectMode" },
					{ 637, "unknown_637" },
				}
			},
			{
				15,
				new Dictionary<int, string>()
				{
					{ 1, "delete" },
					{ 10, "return" },
					{ 11, "call" },
					{ 12, "jump" },
					{ 13, "jumpIfNot" },
					{ 14, "jumpIf" },
					{ 15, "task" },
					{ 22, "callID" },
					{ 23, "wait" },

					{ 42, "push" },
					{ 43, "pop" },
					{ 44, "pushf" },
					{ 45, "popf" },

					{ 50, "add" },
					{ 51, "addf" },
					{ 52, "sub" },
					{ 53, "subf" },
					{ 54, "mul" },
					{ 55, "mulf" },
					{ 56, "div" },
					{ 57, "divf" },
					{ 58, "mod" },
					{ 59, "eq" },
					{ 60, "eqf" },
					{ 61, "neq" },
					{ 62, "neqf" },
					{ 63, "lt" },
					{ 64, "ltf" },
					{ 65, "lteq" },
					{ 66, "lteqf" },
					{ 67, "gt" },
					{ 68, "gtf" },
					{ 69, "gteq" },
					{ 70, "gteqf" },
					{ 73, "or" },
					{ 74, "and" },

					{ 78, "dec" },

					{ 81, "cosSin" },

					{ 83, "invSub" },
					{ 84, "invSubf" },

					{ 300, "createObject" },
					{ 301, "createObjectAt" },
					{ 304, "createObjectMirrored" },
					{ 309, "createObject2" },
					{ 311, "createObject3" },

					{ 302, "setAnm" },
					{ 303, "setSprite" },
					{ 306, "setSpriteEnemy" },
					{ 307, "effect" },
					{ 308, "effect2" },
					{ 325, "setSpriteColor" },
					{ 328, "setSpriteAlpha" },
					{ 329, "setSpriteScale" },
					{ 330, "changeSpriteScale" },
					{ 334, "attachEffect" },
					{ 336, "attachEffect2" },

					{ 400, "setPosition" },
					{ 401, "move" },
					{ 402, "setPosition2" },
					{ 404, "setMotion" },
					{ 405, "changeMotion" },
					{ 412, "moveRandom" },
					{ 426, "moveCurve" },
					{ 441, "changeAngle" },
					{ 445, "changeSpeed" },

					{ 500, "setHitbox" },
					{ 501, "setKillbox" },
					{ 502, "setFlags" },
					{ 503, "clearFlags" },
					{ 504, "setMovementArea" },
					{ 506, "clearDrops" },
					{ 507, "addDrop" },
					{ 508, "setDropArea" },
					{ 509, "drop" },
					{ 510, "setItem" },
					{ 511, "setLife" },
					{ 512, "setBossMode" },
					{ 513, "startAttack" },
					{ 514, "addAttack" },
					{ 515, "setInvulnerableTime" },
					{ 516, "playSound" },

					{ 518, "startMsg" },
					{ 519, "waitMsgEnd" },
					{ 520, "waitBossEnd" },

					{ 524, "setCheckpoint" },
					{ 525, "deleteChildrenObjects" },
					{ 527, "setLifebarRegion" },

					{ 535, "setByDifficulty" },
					{ 536, "setByDifficultyf" },
					{ 547, "setTimeScale" },
					{ 548, "waitByDifficulty" },

					{ 556, "callOnDeath" },

					{ 600, "createShot" },
					{ 601, "fireShot" },
					{ 602, "setShotGraphic" },
					{ 603, "setShotOffset" },
					{ 604, "setShotAngle" },
					{ 605, "setShotSpeed" },
					{ 606, "setShotCount" },
					{ 607, "setShotType" },
					{ 608, "setShotSound" },
					{ 609, "setShotPattern" },
					{ 610, "setShotPattern2" },
					{ 611, "setShotPattern3" },
					{ 612, "setShotPattern4" },
					{ 613, "deleteShots" },
					{ 615, "itemizeShotsCircle" },
					{ 616, "deleteShotsCircle" },
					{ 624, "setShotSpeedByDifficulty" },
					{ 625, "setShotCountByDifficulty" },
					{ 626, "setShotOffsetPolar" },
					{ 627, "setShotOffsetCircle" },
					{ 640, "setShotCreateObject" },

					{ 629, "setAura" },

					{ 632, "setShotDetectMode" },
					{ 637, "unknown_637" },
				}
			}
		};

		static Dictionary<int, Dictionary<int, Action<string[]>>> InstArgAction = new Dictionary<int, Dictionary<int, Action<string[]>>>()
		{
			{
				14,
				new Dictionary<int, Action<string[]>>()
				{
					{ 11, CallArgAction },
					{ 15, CallArgAction },
					{ 22, CallIDArgAction },
					{ 302, SetAnmArgAction },
					{ 330, TweenArg2Action },
					{ 401, TweenArg1Action },
					{ 405, TweenArg1Action },
					{ 412, TweenArg1Action },
					{ 441, TweenArg1Action },
					{ 445, TweenArg1Action },
					{ 502, ECL10Flags },
					{ 503, ECL10Flags },
					{ 512, BossModeAction },
					{ 607, ShotTypeAction },
					{ 609, ShotPattern0Action },
					{ 610, ShotPattern1Action },
					{ 611, ShotPattern2Action },
					{ 612, ShotPattern3Action }
				}
			},
			{
				15,
				new Dictionary<int, Action<string[]>>()
				{
					{ 11, CallArgAction },
					{ 15, CallArgAction },
					{ 22, CallIDArgAction },
					{ 302, SetAnmArgAction },
					{ 330, TweenArg2Action },
					{ 401, TweenArg1Action },
					{ 405, TweenArg1Action },
					{ 412, TweenArg1Action },
					{ 441, TweenArg1Action },
					{ 445, TweenArg1Action },
					{ 502, ECL10Flags },
					{ 503, ECL10Flags },
					{ 512, BossModeAction },
					{ 607, ShotTypeAction },
					{ 609, ShotPattern0Action },
					{ 610, ShotPattern1Action },
					{ 611, ShotPattern2Action },
					{ 612, ShotPattern3Action }
				}
			}
		};

		public static Dictionary<int, Dictionary<string, string>> Globals = new Dictionary<int, Dictionary<string, string>>()
		{
			{ 14, ECL10Globals },
			{ 15, ECL10Globals }
		};

		static Regex RegExInst = new Regex(@"\bins_(\d+)\((.*)\);", RegexOptions.Compiled);
		static Regex RegExSub = new Regex(@"\bsub (\w+)\((.*)\)", RegexOptions.Compiled);
		static Regex RegExVar = new Regex(@"\bvar\s*((?:\w+\s*)*);", RegexOptions.Compiled);
		static Regex RegUseVar = new Regex(@"([$%])(\w+)", RegexOptions.Compiled);
		static Regex RegGlobal = new Regex(@"(\[-.*\])", RegexOptions.Compiled);
		static Regex RegConvFloat = new Regex(@"\b_f\((.*)\)", RegexOptions.Compiled);

		public static string[] WriteScript(string[] input)
		{
			var output = new List<string>();

			string curSub = "";
			var curSubArgs = new Dictionary<string, ECLType>();
			var subLine = -1;
			var curVars = new Dictionary<string, ECLType>();
			var varLine = -1;

			for (var lnum = 0; lnum < input.Length; lnum++)
			{
				var line = input[lnum];

				line = RegConvFloat.Replace(line, new MatchEvaluator((match) =>
				{
					return "float(" + match.Groups[1].Value + ")";
				}));

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


				if (line.TrimStart().StartsWith("sub ") || lnum == input.Length - 1)
				{
					if (subLine != -1)
					{
						if (varLine != -1)
							output.RemoveAt(varLine);
						output[subLine] = "function " + curSub + "(";

						bool first = true;
						foreach (var kv in curSubArgs)
						{
							if (kv.Value != ECLType.Int && kv.Value != ECLType.Float)
								continue;

							if (!first)
								output[subLine] += ", ";

							if (kv.Value == ECLType.Int)
								output[subLine] += "int " + kv.Key;
							else if (kv.Value == ECLType.Float)
								output[subLine] += "float " + kv.Key;

							first = false;
						}

						output[subLine] += ")";
					}
				}

				Match subMatch;
				if ((subMatch = RegExSub.Match(line)).Success)
				{
					subLine = output.Count;
					curSub = subMatch.Groups[1].Value;
					curSubArgs.Clear();
					var curSubArgsA = subMatch.Groups[2].Value.Split(' ');
					foreach (var arg in curSubArgsA)
					{
						curSubArgs.Add(arg, ECLType.Undefined);
					}
					curVars.Clear();
				}

				Match varMatch;
				if ((varMatch = RegExVar.Match(line)).Success)
				{
					varLine = output.Count;
					var vars = varMatch.Groups[1].Value.Split(' ');

					foreach (var varName in vars)
						curVars.Add(varName, ECLType.Undefined);
					

				}

				line = RegUseVar.Replace(line, new MatchEvaluator((match) =>
				{
					if (InQuotes(line, match.Groups[2].Index))
						return match.Value;
					var typeSig = match.Groups[1].Value;
					var varName = match.Groups[2].Value;

					if (!curSubArgs.ContainsKey(varName))
					{
						if (typeSig == "$" && curVars[varName] == ECLType.Undefined)
						{
							output.Insert(varLine + 1, "    int " + varName + ";");
							curVars[varName] = ECLType.Int;
						}
						else
							if (typeSig == "%" && curVars[varName] == ECLType.Undefined)
							{
								output.Insert(varLine + 1, "    float " + varName + ";");
								curVars[varName] = ECLType.Float;
							}
					}
					else
					{
						if (typeSig == "$" && curSubArgs[varName] == ECLType.Undefined)
						{
							curSubArgs[varName] = ECLType.Int;
						}
						else
							if (typeSig == "%" && curSubArgs[varName] == ECLType.Undefined)
							{
								curSubArgs[varName] = ECLType.Float;
							}
					}

					return varName;
				}
				));

				line = RegExInst.Replace(line, new MatchEvaluator((match) =>
				{
					var inst = int.Parse(match.Groups[1].Value);

					var argstr = match.Groups[2].Value;
					while (argstr.Contains("  "))
						argstr = argstr.Replace("  ", " ");
					argstr = argstr.Replace(", ", ",");

					var args = argstr.Split(',');
					if (InstArgAction[THTK.Version].ContainsKey(inst))
					{
						InstArgAction[THTK.Version][inst](args);
					}
						
					if (Inst[THTK.Version].ContainsKey(inst))
						return Inst[THTK.Version][inst] + "(" + String.Join(", ", args) + ");";
					return "ins_" + match.Groups[1].Value + "(" + String.Join(", ", args) + ");";
				}
				));

				output.Add(line);
			}

			return output.ToArray();
		}


		static string TweenType(string id)
		{
			var ret = id;

			switch (id)
			{
				case ("0"):
					ret = "TWEEN_LINEAR";
					break;
				case ("1"):
					ret = "TWEEN_ACCELERATE";
					break;
				case ("4"):
					ret = "TWEEN_DECELERATE";
					break;
				case ("9"):
					ret = "TWEEN_SMOOTH";
					break;
			}

			return ret;
		}


		static void ECL10Flags(string[] args)
		{
			var flagint = int.Parse(args[0]);
			var flags = new List<string>();

			if ((flagint & 1) > 0)
				flags.Add("FLAG_NOHITBOX");
			if ((flagint & 2) > 0)
				flags.Add("FLAG_NOKILLBOX");
			if ((flagint & 4) > 0)
				flags.Add("FLAG_NOBOUNDS_H");
			if ((flagint & 8) > 0)
				flags.Add("FLAG_NOBOUNDS_V");
			if ((flagint & 16) > 0)
				flags.Add("FLAG_SURVIVAL");
			if ((flagint & 32) > 0)
				flags.Add("FLAG_INVISIBLE");
			if ((flagint & 64) > 0)
				flags.Add("FLAG_BOSS");
			if ((flagint & 128) > 0)
				flags.Add("FLAG_PERSISTENTOBJECT");
			if ((flagint & 256) > 0)
				flags.Add("FLAG_UNKNOWN_256");
			if ((flagint & 512) > 0)
				flags.Add("FLAG_GRAZEABLE");
			if ((flagint & 1024) > 0)
				flags.Add("FLAG_UNKNOWN_1024");
			if ((flagint & 2048) > 0)
				flags.Add("FLAG_BOMB");
			if ((flagint & 4096) > 0)
				flags.Add("FLAG_UNKNOWN_4096");
			if ((flagint & 8192) > 0)
				flags.Add("FLAG_NOTIMESCALE");

			if (flags.Count == 0)
				args[0] = "0";
			else
				args[0] = String.Join(" | ", flags);
		}

		static void CallArgAction(string[] args)
		{
			for (var i = 1; i < args.Length; i++)
			{
				var arg = args[i];
				args[i] = arg.Split(' ')[1];
			}
		}

		static void CallIDArgAction(string[] args)
		{
			for (var i = 2; i < args.Length; i++)
			{
				var arg = args[i];
				args[i] = arg.Split(' ')[1];
			}
		}

		static void SetAnmArgAction(string[] args)
		{
			int iarg;
			if (int.TryParse(args[0], out iarg))
				args[0] = (iarg - 2).ToString();
		}

		static void TweenArg1Action(string[] args)
		{
			args[1] = TweenType(args[1]);
		}

		static void TweenArg2Action(string[] args)
		{
			args[2] = TweenType(args[2]);
		}

		static void BossModeAction(string[] args)
		{
			switch (args[0])
			{
				case ("0"):
					args[0] = "TRUE";
					break;
				case ("-1"):
					args[0] = "FALSE";
					break;
			}
		}

		static void ShotTypeAction(string[] args)
		{
			switch (args[1])
			{
				case ("0"):
					args[1] = "TYPE_AIMED";
					break;
				case ("1"):
					args[1] = "TYPE_NORMAL";
					break;
				case ("2"):
					args[1] = "TYPE_CIRCLE_AIMED";
					break;
				case ("3"):
					args[1] = "TYPE_CIRCLE";
					break;
				case ("4"):
					args[1] = "TYPE_CIRCLE2_AIMED";
					break;
				case ("5"):
					args[1] = "TYPE_CIRCLE2";
					break;
				case ("6"):
					args[1] = "TYPE_RANDOM";
					break;
				case ("9"):
					args[1] = "TYPE_TRIANGLE_CIRCLE_AIMED";
					break;
				case ("10"):
					args[1] = "TYPE_TRIANGLE_CIRCLE";
					break;
				case ("11"):
					args[1] = "TYPE_UP_DOWN";
					break;
				case ("12"):
					args[1] = "TYPE_UNKNOWN_12";
					break;
			}
		}

		static string[] ShotPatternNames(string id, string a, string b, string c, string d, string r, string s, string m, string n)
		{
			var ret = new string[] { id, a, b, c, d, r, s, m, n };
			switch (id)
			{
				case ("1"):
					ret[0] = "PATTERN_BOOST";
					break;
				case ("2"):
					ret[0] = "PATTERN_EFFECT";
					break;
				case ("4"):
					ret[0] = "PATTERN_ACCELERATE";
					break;
				case ("8"):
					ret[0] = "PATTERN_ACCELERATE_SPIN";
					break;
				case ("16"):
					ret[0] = "PATTERN_STOP_CHANGE_MOTION";
					break;
				case ("32"):
					ret[0] = "PATTERN_UNKNOWN_32";
					break;
				case ("64"):
					ret[0] = "PATTERN_BOUNCE";
					int bouncei;
					if (ret[2] != "0" && int.TryParse(ret[2], out bouncei))
					{
						List<string> bouncef = new List<string>();
						if ((bouncei & 1) > 0)
							bouncef.Add("WALL_UP");
						if ((bouncei & 2) > 0)
							bouncef.Add("WALL_DOWN");
						if ((bouncei & 4) > 0)
							bouncef.Add("WALL_LEFT");
						if ((bouncei & 8) > 0)
							bouncef.Add("WALL_RIGHT");
						ret[2] = string.Join(" | ", bouncef);
					}
					break;
				case ("128"):
					ret[0] = "PATTERN_NODELETE";
					break;
				case ("256"):
					ret[0] = "PATTERN_UNKNOWN_256";
					break;
				case ("512"):
					ret[0] = "PATTERN_CHANGE_GRAPHIC";
					break;
				case ("1024"):
					ret[0] = "PATTERN_DELETE";
					break;
				case ("2048"):
					ret[0] = "PATTERN_SOUND";
					break;
				case ("4096"):
					ret[0] = "PATTERN_WRAP";
					if (ret[2] != "0" && int.TryParse(ret[2], out bouncei))
					{
						List<string> bouncef = new List<string>();
						if ((bouncei & 1) > 0)
							bouncef.Add("WALL_UP");
						if ((bouncei & 2) > 0)
							bouncef.Add("WALL_DOWN");
						if ((bouncei & 4) > 0)
							bouncef.Add("WALL_LEFT");
						if ((bouncei & 8) > 0)
							bouncef.Add("WALL_RIGHT");
						ret[2] = string.Join(" | ", bouncef);
					}
					break;
				case ("8192"):
					ret[0] = "PATTERN_CREATE_CHILD";
					break;
				case ("16384"):
					ret[0] = "PATTERN_CHILD_GRAPHIC";
					break;
				case ("32768"):
					ret[0] = "PATTERN_ENABLE_DETECT";
					break;
				case ("65536"):
					ret[0] = "PATTERN_GOTO";
					break;
				case ("131072"):
					ret[0] = "PATTERN_UNKNOWN_131072";
					break;
				case ("262144"):
					ret[0] = "PATTERN_UNKNOWN_262144";
					break;
				case ("524288"):
					ret[0] = "PATTERN_UNKNOWN_524288";
					break;
				case ("1048576"):
					ret[0] = "PATTERN_ADDITIVE";
					break;
				case ("2097152"):
					ret[0] = "PATTERN_CHANGE_MOTION";
					break;
				case ("4194304"):
					ret[0] = "PATTERN_CHANGE_SIZE";
					ret[2] = TweenType(ret[2]);
					break;
				case ("8399608"):
					ret[0] = "PATTERN_UNKNOWN_8399608";
					break;
				case ("16777216"):
					ret[0] = "PATTERN_CREATE_OBJECT";
					break;
				case ("33554432"):
					ret[0] = "PATTERN_UNKNOWN_33554432";
					break;
				case ("67108864"):
					ret[0] = "PATTERN_DELAY";
					break;
				case ("134217728"):
					ret[0] = "PATTERN_UNKNOWN_134217728";
					break;
				case ("268435456"):
					ret[0] = "PATTERN_UNKNOWN_268435456";
					break;
				case ("536870912"):
					ret[0] = "PATTERN_UNKNOWN_536870912";
					break;
				case ("1073741824"):
					ret[0] = "PATTERN_UNKNOWN_1073741824";
					break;
				case ("-2147483648"):
					ret[0] = "PATTERN_WAIT";
					break;
			}
			return ret;
		}

		static void ShotPattern0Action(string[] args)
		{
			var names = ShotPatternNames(args[3], args[4], args[5], "", "", args[6], args[7], "", "");
			args[3] = names[0];
			args[4] = names[1];
			args[5] = names[2];
			args[6] = names[5];
			args[7] = names[6];

			for (var i = 4; i < args.Length; i++)
			{
				switch (args[i])
				{
					case ("-999999"):
						args[i] = "NULL";
						break;
					case ("-999999.0f"):
						args[i] = "NULLF";
						break;
					case ("-999.0f"):
						args[i] = "NO_CHANGE";
						break;
					case ("999.0f"):
						args[i] = "AIM_PLAYER";
						break;
				}
			}
		}

		static void ShotPattern1Action(string[] args)
		{
			var names = ShotPatternNames(args[3], args[4], args[5], args[6], args[7], args[8], args[9], args[10], args[11]);
			args[3] = names[0];
			args[4] = names[1];
			args[5] = names[2];
			args[6] = names[3];
			args[7] = names[4];
			args[8] = names[5];
			args[9] = names[6];
			args[10] = names[7];
			args[11] = names[8];

			for (var i = 4; i < args.Length; i++)
			{
				switch (args[i])
				{
					case ("-999999"):
						args[i] = "NULL";
						break;
					case ("-999999.0f"):
						args[i] = "NULLF";
						break;
					case ("-999.0f"):
						args[i] = "NO_CHANGE";
						break;
					case ("999.0f"):
						args[i] = "AIM_PLAYER";
						break;
				}
			}
		}

		static void ShotPattern2Action(string[] args)
		{
			var names = ShotPatternNames(args[2], args[3], args[4], "", "", args[5], args[6], "", "");
			args[2] = names[0];
			args[3] = names[1];
			args[4] = names[2];
			args[5] = names[5];
			args[6] = names[6];

			for (var i = 3; i < args.Length; i++)
			{
				switch (args[i])
				{
					case ("-999999"):
						args[i] = "NULL";
						break;
					case ("-999999.0f"):
						args[i] = "NULLF";
						break;
					case ("-999.0f"):
						args[i] = "NO_CHANGE";
						break;
					case ("999.0f"):
						args[i] = "AIM_PLAYER";
						break;
				}
			}
		}

		static void ShotPattern3Action(string[] args)
		{
			var names = ShotPatternNames(args[2], args[3], args[4], args[5], args[6], args[7], args[8], args[9], args[10]);
			args[2] = names[0];
			args[3] = names[1];
			args[4] = names[2];
			args[5] = names[3];
			args[6] = names[4];
			args[7] = names[5];
			args[8] = names[6];
			args[9] = names[7];
			args[10] = names[8];

			for (var i = 3; i < args.Length; i++)
			{
				switch (args[i])
				{
					case ("-999999"):
						args[i] = "NULL";
						break;
					case ("-999999.0f"):
						args[i] = "NULLF";
						break;
					case ("-999.0f"):
						args[i] = "NO_CHANGE";
						break;
					case ("999.0f"):
						args[i] = "AIM_PLAYER";
						break;
				}
			}
		}

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
	}
}

