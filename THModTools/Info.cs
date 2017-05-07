using System;

namespace THModTools
{
	public static class Info
	{
		public static readonly string ProgramDirectory;

		static Info()
		{
			ProgramDirectory = new System.IO.FileInfo(
				System.Reflection.Assembly.GetExecutingAssembly().Location
			).DirectoryName + "/";
		}
	}
}

