using System;
using System.IO;
using System.Collections.Generic;
using IniParser;
using IniParser.Parser;
using IniParser.Model;

namespace THModTools
{
	public static class Project
	{
		public static string DirBuildFiles;
		public static string DirBuild;
		public static string DirSource;
		public static Dictionary<string, string> Settings = new Dictionary<string, string>();

		public static string IniPath;

		static int[] Versions = new int[] {
			6, 7, 8, 9, 95, 10, 105, 11, 12, 123, 125, 128, 13, 14, 143, 15, 16
		};

		public static string[] BuildFiles
		{
			get
			{
				return Directory.GetFiles(DirBuildFiles);
			}
		}

		public static string[] SourceFiles
		{
			get
			{
				return Directory.GetFiles(DirSource);
			}
		}

		static Project()
		{
			DirBuildFiles = Info.ProgramDirectory + "Project/BuildFiles/";
			DirBuild = Info.ProgramDirectory + "Project/Build/";
			DirSource = Info.ProgramDirectory + "Project/Source/";

			Settings["version"] = "15";
		}

		public static void SetVersionFromArchive(string path)
		{
			var name = new FileInfo(path).Name;

			foreach (var version in Versions)
			{
				var vname = version.ToString();
				if (vname.Length == 1)
					vname = "0" + vname;
				if (name.Contains(vname))
				{
					THTK.Version = version;
					Settings["version"] = version.ToString();
					break;
				}
			}
		}

		public static IniData NewIni()
		{
			var ini = new IniData();

			return ini;
		}

		public static void ReadSettings()
		{
			Console.WriteLine("Reading settings");

			if (!File.Exists(IniPath))
			{
				WriteSettings();
				return;
			}

			var parser = new FileIniDataParser();
			var ini = parser.ReadFile(IniPath);
			var section = ini.Sections["settings"];

			foreach (var key in section)
			{
				Settings[key.KeyName] = key.Value;
			}

			int parseInt;
			if (int.TryParse(Settings["version"], out parseInt))
				THTK.Version = parseInt;
			else
				throw new Exception("Invalid version");
		}

		public static void WriteSettings()
		{
			Console.WriteLine("Writing settings");

			var parser = new FileIniDataParser();
			IniData ini;
			KeyDataCollection section;

			if (File.Exists(IniPath))
			{
				ini = parser.ReadFile(IniPath);
				section = ini.Sections["settings"];
				section.RemoveAllKeys();
			}
			else
			{
				ini = NewIni();
				ini.Sections.AddSection("settings");
				section = ini.Sections["settings"];
			}

			foreach (var kv in Settings)
			{
				section.AddKey(kv.Key, kv.Value);
			}

			parser.WriteFile(IniPath, ini, System.Text.Encoding.ASCII);
		}
	}
}

