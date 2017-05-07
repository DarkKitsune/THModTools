using System;

namespace THModTools
{
	public struct MacroFunction
	{
		public ECLType[] Args;
		public Func<object[], string> Function;

		public string Invoke(string line, params string[] args)
		{
			if (args.Length != Args.Length)
				throw new Exception("Argument count mismatch near \"" + line + "\"");

			var convArgs = new object[args.Length];
			var i = 0;
			foreach (var arg in args)
			{
				int iarg;
				float farg;
				switch (Args[i])
				{
					case (ECLType.Int):
						if (int.TryParse(arg, out iarg))
							convArgs[i] = iarg;
						else
							throw new Exception("Argument \"" + arg + "\" was not type \"int\" in macro function near \"" + line + "\"");
						break;
					case (ECLType.Float):
						if (arg.EndsWith("f") && float.TryParse(arg.Replace("f",""), out farg))
							convArgs[i] = farg;
						else
							throw new Exception("Argument \"" + arg + "\" was not type \"float\" in macro function near \"" + line + "\"");
						break;
					case (ECLType.Expression):
						convArgs[i] = arg;
						break;
				}
				i++;
			}

			return Function(convArgs);
		}
	}
}

