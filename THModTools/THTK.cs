using System;
using System.IO;
using System.Collections.Generic;
using System.Diagnostics;

namespace THModTools
{
	public static class THTK
	{
		const string TOOLPATH = "thtk/";

		static THTK()
		{
			
		}

		public static int Version = 15;

		static string Run(string prgm, string directory, string arg)
		{
			if (!Directory.Exists(Info.ProgramDirectory + "/" + TOOLPATH))
			{
				System.Windows.Forms.MessageBox.Show("THTK binaries were not found in " + new DirectoryInfo(TOOLPATH).FullName);
				return "Could not finish.";
			}

			var path = TOOLPATH + prgm;

			switch (Platform.ID)
			{
				case (Platform.Windows):
					path += ".exe";
					break;
			}

			var prgmPath = Info.ProgramDirectory + "/" + path;

			var info = new ProcessStartInfo(prgmPath, arg);
			info.WorkingDirectory = directory;
			info.UseShellExecute = false;
			info.RedirectStandardOutput = true;
			info.RedirectStandardError = true;

			string ret;
			using (var proc = Process.Start(info))
			{
				using (var def = new StringWriter())
				{
					while (!proc.HasExited)
					{
						def.Write(proc.StandardOutput.ReadToEnd());
						var err = proc.StandardError.ReadToEnd();
						if (err.Length > 0)
							def.Write("\nERROR: " + err + "\n");
					}
					ret = def.ToString();
				}
			}

			return ret;
		}

		static Process RunGetProcess(string prgm, string directory, string arg)
		{
			if (!Directory.Exists(Info.ProgramDirectory + "/" + TOOLPATH))
			{
				System.Windows.Forms.MessageBox.Show("THTK binaries were not found in " + new DirectoryInfo(TOOLPATH).FullName);
				return null;
			}

			var path = TOOLPATH + prgm;

			switch (Platform.ID)
			{
				case (Platform.Windows):
					path += ".exe";
					break;
			}

			var prgmPath = Info.ProgramDirectory + "/" + path;

			var info = new ProcessStartInfo(prgmPath, arg);
			info.WorkingDirectory = directory;
			info.UseShellExecute = false;
			info.RedirectStandardOutput = true;
			info.RedirectStandardError = true;

			var proc = Process.Start(info);

			return proc;
		}

		static string ChooseDatName()
		{
			var vername = Version.ToString();
			if (vername.Length == 1)
				vername = "0" + vername;
			if (vername == "16")
				vername += "tr";

			return "th" + vername + ".dat";
		}

		static void PrintOutput(Process proc)
		{
			if (proc == null)
				return;
			Console.WriteLine("*****THTK Output:*****");
			while (!proc.HasExited)
			{
				var read = proc.StandardOutput.ReadToEnd();
				if (read.Length > 0)
					Console.WriteLine(read);
				var err = proc.StandardError.ReadToEnd();
				if (err.Length > 0)
					Console.WriteLine("\nERROR: " + err + "\n");
			}
			proc.Dispose();
			Console.WriteLine("**********************");
		}


		public static class THDAT
		{
			public static void Extract(string path)
			{
				Console.WriteLine(File.Exists(path));

				if (File.Exists(path))
				{
					Console.WriteLine("Extracting DAT archive...");
					PrintOutput(RunGetProcess("thdat", Project.DirBuildFiles, "x" +  (Version == 16 ? 15 : Version) + " " + path));
					Console.WriteLine("Done");
				}
				else
				{
					if (path == "")
						throw new Exception("Archive not set");
					else
						throw new Exception("Archive '" + path + "' doesn't exist");
				}
			}

			public static void Create()
			{
				Console.WriteLine("Creating DAT archive...");
				PrintOutput(RunGetProcess("thdat",
					Project.DirBuild,
					"c" + (Version == 16 ? 15 : Version) + " " + Project.DirBuild + ChooseDatName() +
					" " + String.Join(" ", Project.BuildFiles)
				));
				Console.WriteLine("Done");
			}
		}

		public static class THANM
		{
			public static string SourceDir = Project.DirSource + "ANM/";

			public static void Extract()
			{
				Directory.CreateDirectory(SourceDir);

				var files = Project.BuildFiles;

				foreach (var file in files)
				{
					if (file.EndsWith(".anm"))
					{
						Console.WriteLine("Extracting " + file);
						var name = new FileInfo(file).Name.Replace(".anm", ".anm.txt");
						File.WriteAllLines(SourceDir + name, ANMScriptWriter.WriteScript(
                            Run("thanm", Project.DirSource, "l" + " " + file).Replace("\r", "").Split('\n')
						));
						PrintOutput(RunGetProcess("thanm", SourceDir, "x" + " " + file));
					}
				}
				Console.WriteLine("Done");
			}

			public static void Build()
			{
				var files = Directory.GetFiles(SourceDir);
				foreach (var file in files)
				{
					if (file.EndsWith(".anm.txt"))
					{
						Build(new FileInfo(file).Name);
					}
				}
				Console.WriteLine("Done");
			}

			public static void Build(string file)
			{
				Directory.CreateDirectory(SourceDir);

				var name = file.Replace(".anm.txt", ".anm");
				var anmfile = Project.DirBuildFiles + name;

				Console.WriteLine("Building " + name);

				file = SourceDir + file;
				var tempfile = file.Replace(".anm.txt", ".temp");
				File.WriteAllLines(tempfile, ANMScriptReader.ReadScript(File.ReadAllLines(file)));

				PrintOutput(RunGetProcess("thanm", SourceDir, "c" + " " + anmfile + " " + tempfile));

				File.Delete(tempfile);
				Console.WriteLine("Done");
			}
		}


		public static class THECL
		{
			public static string SourceDir = Project.DirSource + "ECL/";

			public static void Extract()
			{
				Directory.CreateDirectory(SourceDir);

				var files = Project.BuildFiles;

				foreach (var file in files)
				{
					if (file.EndsWith(".ecl"))
					{
						Console.WriteLine("Extracting " + file);
						var name = new FileInfo(file).Name.Replace(".ecl", ".ecl.txt");
						File.WriteAllLines(SourceDir + name, ECLScriptWriter.WriteScript(
							Run("thecl", Project.DirSource, "d" + Version + " " + file).Replace("\r", "").Split('\n')
						));
					}
				}
				Console.WriteLine("Done");
			}

			public static void Build()
			{
				var files = Directory.GetFiles(SourceDir);
				foreach (var file in files)
				{
					if (file.EndsWith(".ecl.txt"))
					{
						Build(new FileInfo(file).Name);
					}
				}
				Console.WriteLine("Done");
			}

			public static void Build(string file)
			{
				Directory.CreateDirectory(SourceDir);

				var name = file.Replace(".ecl.txt", ".ecl");
				var eclfile = Project.DirBuildFiles + name;

				Console.WriteLine("Building " + name);

				file = SourceDir + file;
				var tempfile = file.Replace(".ecl.txt", ".temp");
				var lines = ECLScriptReader.ReadScript(File.ReadAllLines(file));
				Console.WriteLine(lines.Length);
				File.WriteAllLines(tempfile, lines);

				PrintOutput(RunGetProcess("thecl", SourceDir, "c" + Version + " " + tempfile + " " + eclfile));

				//File.Delete(tempfile);
				Console.WriteLine("Done");
			}
		}

		public static class THSTD
		{
			public static string SourceDir = Project.DirSource + "STD/";

			public static void Extract()
			{
				Directory.CreateDirectory(SourceDir);

				var files = Project.BuildFiles;

				foreach (var file in files)
				{
					if (file.EndsWith(".std"))
					{
						Console.WriteLine("Extracting " + file);
						var name = new FileInfo(file).Name.Replace(".std", ".std.txt");
						File.WriteAllLines(SourceDir + name, STD.Read(file));
					}
				}
				Console.WriteLine("Done");
			}

			public static void Build()
			{
				var files = Directory.GetFiles(SourceDir);
				foreach (var file in files)
				{
					if (file.EndsWith(".std.txt"))
					{
						Build(new FileInfo(file).Name);
					}
				}
				Console.WriteLine("Done");
			}

			public static void Build(string file)
			{
				Directory.CreateDirectory(SourceDir);

				var name = file.Replace(".std.txt", ".std");
				var stdfile = Project.DirBuildFiles + name;

				Console.WriteLine("Building " + name);

				file = SourceDir + file;
				var lines = File.ReadAllLines(file);
				STD.Write(stdfile, lines);

				Console.WriteLine("Done");;
			}
		}
	}
}

