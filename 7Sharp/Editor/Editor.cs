using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace _7Sharp.Editor
{
	internal partial class Editor : Form
	{
		public string Code
		{
			get => codeBox.Text;
			set => codeBox.Text = value ?? string.Empty;
		}

		public Editor() => InitializeComponent();

		private void SaveAndExitButton_Click(object sender, EventArgs e) => Close();

		private void Editor_Resize(object sender, EventArgs e) => saveAndExitButton.Size = new Size(Size.Width, (int)(Size.Height * 0.1625));

		private void CodeBox_KeyDown(object sender, KeyEventArgs e)
		{
			if (!e.Control)
			{
				return;
			}
			int delta = 0;
			switch (e.KeyCode)
			{
				case Keys.Oemplus:
					delta = 1;
					break;
				case Keys.OemMinus:
					delta = -1;
					break;
			}
			if (codeBox.Font.SizeInPoints + delta > 0)
			{
				codeBox.Font = new Font(codeBox.Font.FontFamily, codeBox.Font.SizeInPoints + delta, GraphicsUnit.Point);
			}
		}
	}
}
