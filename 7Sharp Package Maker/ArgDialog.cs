using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace _7Sharp_Package_Maker
{
	public partial class ArgDialog : Form
	{
		public ArgDialog(Blocks b)
		{
			InitializeComponent();
			set = b;
			DialogResult = DialogResult.None;
		}
		private Blocks set;
		public Blocks LastSet;
		private void ArgDialog_Resize(object sender, EventArgs e)
		{

		}

		private void OK_Click(object sender, EventArgs e)
		{
			set.com = a.Text;
			set.a1 = b.Text.Replace(' ', '~');
			set.a2 = c.Text.Replace(' ', '~');
			set.a3 = d.Text.Replace(' ', '~');
			set.a4 = this.e.Text.Replace(' ', '~');
			LastSet = set;
			Console.WriteLine(set.Command);
			Console.WriteLine(LastSet.Command);
			DialogResult = DialogResult.OK;
			Close();
		}

		private void ArgDialog_Load(object sender, EventArgs e)
		{
			a.Text = set.com;
			b.Text = set.a1;
			c.Text = set.a2;
			d.Text = set.a3;
			this.e.Text = set.a4;

		}

		private void a_TextChanged(object sender, EventArgs e)
		{
			if (a.Text.Contains(' '))
			{
				try
				{
					a.Text = "NoSpaces!";
					System.Media.SoundPlayer sp = new System.Media.SoundPlayer("C:\\Windows\\media\\Windows Background.wav");
					sp.Play();
					a.Text = "";
				}
				catch (Exception error)
				{
					Console.WriteLine(error.Message);
				}
			}
		}
	}
}
