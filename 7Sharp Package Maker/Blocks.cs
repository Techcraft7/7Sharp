using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace _7Sharp_Package_Maker
{
	public partial class Blocks : UserControl
	{
		public Blocks()
		{
			InitializeComponent();
			com = "Block";
		}
		public string com, a1, a2, a3, a4 = "";
		public string Command
		{
			get
			{
				return com;
			}
			set
			{
				if (value.Length == 0)
				{
					value = "Block";
				}
			}
		}
		public string Arg1
		{
			get
			{
				return a1;
			}
			set
			{

			}
		}
		public string Arg2
		{
			get
			{
				return a2;
			}
			set
			{

			}
		}
		public string Arg3
		{
			get
			{
				return a3;
			}
			set
			{

			}
		}
		public string Arg4
		{
			get
			{
				return a4;
			}
			set
			{

			}
		}

		private void Text_Click(object sender, EventArgs e)
		{
			Blocks_MouseClick(sender, (MouseEventArgs)e);
		}

		private void Blocks_Click(object sender, EventArgs e)
		{
			Blocks_MouseClick(sender, (MouseEventArgs)e);
		}

		private void Blocks_MouseClick(object sender, MouseEventArgs e)
		{
			if (e.Button == MouseButtons.Left)
			{
				ArgDialog dia = new ArgDialog(this);
				if (dia.ShowDialog() == DialogResult.OK)
				{
					Console.WriteLine("OK");
					Command = dia.LastSet.Command;
					Arg1 = dia.LastSet.Arg1;
					Arg2 = dia.LastSet.Arg2;
					Arg3 = dia.LastSet.Arg3;
					Arg4 = dia.LastSet.Arg4;
				}
			}
			else if (e.Button == MouseButtons.Right)
			{
				Dispose();
			}
		}

		private Color c = Color.FromArgb(100, 0, 0, 0);
		public Color Color
		{
			get
			{
				return c;
			}
			set
			{
				if (value == null)
				{
					value = Color.FromArgb(100, 0, 0, 0);
				}
			}
		}
		private void Update_Tick(object sender, EventArgs e)
		{
			if (BlockText.Text.Length != 0)
			{
				BlockText.Size = new Size(Size.Width, Size.Height);
				BlockText.Font = new Font(FontFamily.GenericMonospace, Size.Width / BlockText.Text.Length, FontStyle.Regular);
				BackColor = Color;
				BlockText.Text = Command;
				Size = new Size(Size.Width, BlockText.Font.Height);
				BlockText.BackColor = Color.FromArgb(0, 0, 0, 0);
			}
			else
			{
				BlockText.Text = "Error";
			}
		}

		private void Blocks_Load(object sender, EventArgs e)
		{
			UpdateClock.Start();
		}
	}
}
