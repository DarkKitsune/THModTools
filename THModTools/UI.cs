using System;
using System.Drawing;
using System.Collections.Generic;
using System.Windows.Forms;

namespace THModTools
{
	public class UI : Form
	{
		public Dictionary<string, Control> ControlDict = new Dictionary<string, Control>();
		public int CurrentX = 8;
		public int CurrentY = 8;
		int NextCX = 0;
		int NextCY = 0;
		public int MarginX = 8;
		public int MarginY = 8;
		public bool Vertical = false;

		public Control this[string name]
		{
			get
			{
				return ControlDict[name];
			}
			set
			{
				ControlDict[name] = value;
			}
		}

		public UI(int w, int h, string title, bool resizable)
		{
			Size = new Size(w, h);
			Text = title;
			if (!resizable)
				FormBorderStyle = FormBorderStyle.FixedSingle;
		}

		public void Start()
		{
			Application.Run(this);
		}

		void AddControl(string name, string label, Control control)
		{
			if (control is Label || control is Button)
			{
				control.Parent = this;
				control.Location = new Point(CurrentX, CurrentY);

				ControlDict.Add(name, control);
			}
			else
			{
				var labelControl = new Label() { Location = new Point(CurrentX, CurrentY), Text = label, Parent = this };
				ControlDict.Add(name + ".label", labelControl);

				control.Parent = this;
				control.Location = new Point(CurrentX, labelControl.Bottom);

				ControlDict.Add(name, control);
			}

			NextCX = control.Right + MarginX;
			NextCY = control.Bottom + MarginY;
		}

		public void AddButton(string name, string text, Action onClick)
		{
			var control = new Button() { Text = text };
			control.Click += new EventHandler(delegate(object sender, EventArgs e) {
				onClick();
			});
			control.AutoSize = true;
			AddControl(name, "", control);
		}

		public void AddLabel(string name, string text)
		{
			var control = new Label() { Text = text };
			control.AutoSize = true;
			AddControl(name, "", control);
		}

		public void AddListBox(string name, string label, int height, object[] items)
		{
			var control = new ListBox() { BackColor = Color.White };
			control.Items.AddRange(items);
			control.Width = control.PreferredSize.Width;
			control.Height = height;
			AddControl(name, label, control);
		}

		public void AddDividerH(string name)
		{
			CurrentY -= 2;
			var control = new Label() {
				AutoSize = false, Text = "",
				BorderStyle = BorderStyle.Fixed3D,
				Width = ClientSize.Width - 4,
				Height = 2
			};
			AddControl(name, "", control);
			control.Location = new Point(2, control.Location.Y);
		}


		public void SetPosition(int x, int y)
		{
			CurrentX = x;
			CurrentY = y;
		}

		public void Shift()
		{
			CurrentX = NextCX;
		}

		public void Drop()
		{
			CurrentY = NextCY;
		}

		public void MeetLeft()
		{
			CurrentX = 12;
		}

		public void MeetTop()
		{
			CurrentY = 12;
		}


		public string GetFile(string filter, string title)
		{
			var open = new OpenFileDialog();
			open.Title = title;
			open.Filter = filter;

			var res = open.ShowDialog();
			if (res == DialogResult.OK)
				return open.FileName;
			else
				return null;
		}
	}
}

