using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows.Forms;

namespace _7Sharp_Package_Maker
{
	public partial class Form1 : Form
	{
		public Form1()
		{
			InitializeComponent();
			MessageBox.Show("This is deprecated... please use the 7Sharp API!");
		}

		private void CommandSpace_DragDrop(object sender, DragEventArgs e)
		{

		}

		private void AddBlock_Click(object sender, EventArgs e)
		{
			CommandSpace.Controls.Add(new Blocks());
			CommandSpace.Update();
		}

		private void UpdateClock_Tick(object sender, EventArgs e)
		{
			CommandSpace.Update();
		}

		private void SaveButton_Click(object sender, EventArgs e)
		{
			if (SFD.ShowDialog() == DialogResult.OK)
			{
				StreamWriter sw = new StreamWriter(SFD.FileName);
				sw.WriteLine(CmdName.Text);
				Console.WriteLine(CmdName.Text);
				foreach (Blocks i in CommandSpace.Controls)
				{
					string foo = string.Format("{0} {1} {2} {3} {4}", i.com, i.a1, i.a2, i.a3, i.a4);
					Console.WriteLine(foo);
					sw.WriteLine(foo);
				}
				sw.Close();
				sw.Dispose();
			}
		}

		private void LoadButton_Click(object sender, EventArgs e)
		{
			Console.WriteLine("HI");
			if (OFD.ShowDialog() == DialogResult.OK)
			{
				Console.WriteLine("HI");
				StreamReader sr = new StreamReader(OFD.FileName);
				CmdName.Text = sr.ReadLine(); //command name
				while (sr.EndOfStream == false)
				{
					string[] split = sr.ReadLine().Split(' ');
					foreach (string i in split)
					{
						i.Replace(' ', '~');
						i.Replace("~", "\\~");
					}
					Blocks b = new Blocks();
					b.com = split[0];
					b.a1 = split[1];
					b.a2 = split[2];
					b.a3 = split[3];
					b.a4 = split[4];
					CommandSpace.Controls.Add(b);
					Console.WriteLine("YES!");
					CommandSpace.Update();
				}
				sr.Close();
				sr.Dispose();
			}
		}

		private void CmdName_TextChanged(object sender, EventArgs e)
		{
			if (CmdName.Text.Contains(' '))
			{
				try
				{
					System.Media.SoundPlayer sp = new System.Media.SoundPlayer("C:\\Windows\\media\\Windows Background.wav");
					sp.Play();
					CmdName.Text = "";
				}
				catch (Exception error)
				{
					Console.WriteLine(error.Message);
				}
			}
		}
	}
}
