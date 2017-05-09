using System;
using System.IO;
using System.Windows.Forms;
using System.Collections.Generic;

namespace THModTools
{
	class MainClass
	{
		static UI MainUI;

		[STAThread]
		public static void Main(string[] args)
		{
			Directory.CreateDirectory(Project.DirBuildFiles);
			Directory.CreateDirectory(Project.DirBuild);
			Directory.CreateDirectory(Project.DirSource);
			Directory.CreateDirectory(THTK.THANM.SourceDir);
			Directory.CreateDirectory(THTK.THECL.SourceDir);
			Directory.CreateDirectory(THTK.THSTD.SourceDir);

			Project.IniPath = Info.ProgramDirectory + "Project/project.ini";
			Project.ReadSettings();

			MainUI = new UI(800, 600, "TH Mod Tools", false);
			MainUI.AddLabel("thver", "Project Version = " + THTK.Version);

			MainUI.Drop();

			MainUI.AddButton("extract", "Create Project From Archive", () => {
				var open = MainUI.GetFile("DAT file (*.dat)|*.dat", "Choose DAT File To Extract");
				if (open != null)
				{
					Project.SetVersionFromArchive(open);
					MainUI["thver"].Text = "Project Version = " + THTK.Version;
					THTK.THDAT.Extract(open);
				}
			});
			MainUI.Shift();
			MainUI.AddButton("build", "Build Archive", () => {
				THTK.THDAT.Create();
			});

			MainUI.Drop();
			MainUI.AddDividerH("middle div");

			//ANM
			MainUI.SetPosition(8, 70);

			MainUI.AddButton("extract anm", "Extract ANMs", () => {
				THTK.THANM.Extract();
				RefreshANM();
			});
			MainUI.Drop();
			MainUI.AddButton("build anms", "Build All ANMs", () => {
				THTK.THANM.Build();
			});
			MainUI.Drop();
			MainUI.AddListBox("anm list", "ANM Sources", 200, new object[] {});
			MainUI.Drop();
			MainUI.AddButton("refresh anm list", "Refresh", () => {
				RefreshANM();
			});
			MainUI.Drop();
			MainUI.AddButton("build anm", "Build", () => {
				THTK.THANM.Build((MainUI["anm list"] as ListBox).SelectedItem as string);
			});

			//ECL
			MainUI.SetPosition(158, 70);

			MainUI.AddButton("extract ecl", "Extract ECLs", () => {
				THTK.THECL.Extract();
				RefreshECL();
			});
			MainUI.Drop();
			MainUI.AddButton("build ecls", "Build All ECLs", () => {
				THTK.THECL.Build();
			});
			MainUI.Drop();
			MainUI.AddListBox("ecl list", "ECL Sources", 200, new object[] {});
			MainUI.Drop();
			MainUI.AddButton("refresh ecl list", "Refresh", () => {
				RefreshECL();
			});
			MainUI.Drop();
			MainUI.AddButton("build ecl", "Build", () => {
				THTK.THECL.Build((MainUI["ecl list"] as ListBox).SelectedItem as string);
			});

			//STD
			MainUI.SetPosition(308, 70);

			MainUI.AddButton("extract std", "Extract STDs", () => {
				THTK.THSTD.Extract();
				RefreshSTD();
			});
			MainUI.Drop();
			MainUI.AddButton("build stds", "Build All STDs", () => {
				THTK.THSTD.Build();
			});
			MainUI.Drop();
			MainUI.AddListBox("std list", "STD Sources", 200, new object[] {});
			MainUI.Drop();
			MainUI.AddButton("refresh std list", "Refresh", () => {
				RefreshSTD();
			});
			MainUI.Drop();
			MainUI.AddButton("build std", "Build", () => {
				THTK.THSTD.Build((MainUI["std list"] as ListBox).SelectedItem as string);
			});


			RefreshANM();
			RefreshECL();
			RefreshSTD();

			//THTK.THSTD.Extract(Project.DirBuildFiles + "st01.std");
			//THTK.THSTD.Build("st01.std.txt");
				
			MainUI.Start();

			Project.WriteSettings();
		}

		static void RefreshANM()
		{
			var list = new List<string>();
			foreach (var file in Directory.GetFiles(THTK.THANM.SourceDir))
				list.Add(new FileInfo(file).Name);
			var lb = MainUI["anm list"] as ListBox;
			lb.Items.Clear();
			lb.Items.AddRange(list.ToArray());
		}

		static void RefreshECL()
		{
			var list = new List<string>();
			foreach (var file in Directory.GetFiles(THTK.THECL.SourceDir))
				list.Add(new FileInfo(file).Name);
			var lb = MainUI["ecl list"] as ListBox;
			lb.Items.Clear();
			lb.Items.AddRange(list.ToArray());
		}

		static void RefreshSTD()
		{
			var list = new List<string>();
			foreach (var file in Directory.GetFiles(THTK.THSTD.SourceDir))
				list.Add(new FileInfo(file).Name);
			var lb = MainUI["std list"] as ListBox;
			lb.Items.Clear();
			lb.Items.AddRange(list.ToArray());
		}
	}
}
