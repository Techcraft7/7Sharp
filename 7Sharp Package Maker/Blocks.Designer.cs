namespace _7Sharp_Package_Maker
{
	partial class Blocks
	{
		/// <summary> 
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary> 
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Component Designer generated code

		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			this.Text = new System.Windows.Forms.Label();
			this.UpdateClock = new System.Windows.Forms.Timer(this.components);
			this.SuspendLayout();
			// 
			// Text
			// 
			this.Text.AutoSize = true;
			this.Text.Dock = System.Windows.Forms.DockStyle.Fill;
			this.Text.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel, ((byte)(0)));
			this.Text.Location = new System.Drawing.Point(0, 0);
			this.Text.Name = "Text";
			this.Text.Size = new System.Drawing.Size(32, 13);
			this.Text.TabIndex = 0;
			this.Text.Text = "Block";
			this.Text.Click += new System.EventHandler(this.Text_Click);
			// 
			// UpdateClock
			// 
			this.UpdateClock.Tick += new System.EventHandler(this.Update_Tick);
			// 
			// Blocks
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.Color.DarkGray;
			this.Controls.Add(this.Text);
			this.Name = "Blocks";
			this.Size = new System.Drawing.Size(150, 59);
			this.Load += new System.EventHandler(this.Blocks_Load);
			this.Click += new System.EventHandler(this.Blocks_Click);
			this.MouseClick += new System.Windows.Forms.MouseEventHandler(this.Blocks_MouseClick);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		public System.Windows.Forms.Label Text;
		private System.Windows.Forms.Timer UpdateClock;
	}
}
