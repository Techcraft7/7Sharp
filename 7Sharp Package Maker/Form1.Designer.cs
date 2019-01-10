namespace _7Sharp_Package_Maker
{
	partial class Form1
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

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
			this.CommandSpace = new System.Windows.Forms.FlowLayoutPanel();
			this.flowLayoutPanel2 = new System.Windows.Forms.FlowLayoutPanel();
			this.SaveButton = new System.Windows.Forms.Button();
			this.LoadButton = new System.Windows.Forms.Button();
			this.AddBlock = new System.Windows.Forms.Button();
			this.UpdateClock = new System.Windows.Forms.Timer(this.components);
			this.OFD = new System.Windows.Forms.OpenFileDialog();
			this.SFD = new System.Windows.Forms.SaveFileDialog();
			this.CmdName = new System.Windows.Forms.TextBox();
			this.flowLayoutPanel2.SuspendLayout();
			this.SuspendLayout();
			// 
			// CommandSpace
			// 
			this.CommandSpace.AllowDrop = true;
			this.CommandSpace.AutoScroll = true;
			this.CommandSpace.BackColor = System.Drawing.SystemColors.ButtonHighlight;
			this.CommandSpace.Dock = System.Windows.Forms.DockStyle.Left;
			this.CommandSpace.Location = new System.Drawing.Point(0, 0);
			this.CommandSpace.Name = "CommandSpace";
			this.CommandSpace.Size = new System.Drawing.Size(273, 381);
			this.CommandSpace.TabIndex = 0;
			this.CommandSpace.DragDrop += new System.Windows.Forms.DragEventHandler(this.CommandSpace_DragDrop);
			// 
			// flowLayoutPanel2
			// 
			this.flowLayoutPanel2.Controls.Add(this.SaveButton);
			this.flowLayoutPanel2.Controls.Add(this.LoadButton);
			this.flowLayoutPanel2.Controls.Add(this.AddBlock);
			this.flowLayoutPanel2.Controls.Add(this.CmdName);
			this.flowLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Right;
			this.flowLayoutPanel2.Location = new System.Drawing.Point(279, 0);
			this.flowLayoutPanel2.Name = "flowLayoutPanel2";
			this.flowLayoutPanel2.Padding = new System.Windows.Forms.Padding(5);
			this.flowLayoutPanel2.Size = new System.Drawing.Size(125, 381);
			this.flowLayoutPanel2.TabIndex = 1;
			// 
			// SaveButton
			// 
			this.SaveButton.Location = new System.Drawing.Point(5, 5);
			this.SaveButton.Margin = new System.Windows.Forms.Padding(0);
			this.SaveButton.Name = "SaveButton";
			this.SaveButton.Size = new System.Drawing.Size(110, 46);
			this.SaveButton.TabIndex = 0;
			this.SaveButton.Text = "save";
			this.SaveButton.UseVisualStyleBackColor = true;
			this.SaveButton.Click += new System.EventHandler(this.SaveButton_Click);
			// 
			// LoadButton
			// 
			this.LoadButton.Location = new System.Drawing.Point(5, 51);
			this.LoadButton.Margin = new System.Windows.Forms.Padding(0);
			this.LoadButton.Name = "LoadButton";
			this.LoadButton.Size = new System.Drawing.Size(110, 46);
			this.LoadButton.TabIndex = 1;
			this.LoadButton.Text = "load";
			this.LoadButton.UseVisualStyleBackColor = true;
			this.LoadButton.Click += new System.EventHandler(this.LoadButton_Click);
			// 
			// AddBlock
			// 
			this.AddBlock.Location = new System.Drawing.Point(5, 97);
			this.AddBlock.Margin = new System.Windows.Forms.Padding(0);
			this.AddBlock.Name = "AddBlock";
			this.AddBlock.Size = new System.Drawing.Size(110, 46);
			this.AddBlock.TabIndex = 2;
			this.AddBlock.Text = "Add Block";
			this.AddBlock.UseVisualStyleBackColor = true;
			this.AddBlock.Click += new System.EventHandler(this.AddBlock_Click);
			// 
			// UpdateClock
			// 
			this.UpdateClock.Enabled = true;
			this.UpdateClock.Interval = 1;
			this.UpdateClock.Tick += new System.EventHandler(this.UpdateClock_Tick);
			// 
			// OFD
			// 
			this.OFD.Filter = "7Sharp Packages (*.7spkg)|*.7spkg";
			this.OFD.Title = "Open Package";
			// 
			// SFD
			// 
			this.SFD.FileName = "My Package.7spkg";
			this.SFD.Filter = "7Sharp Packages (*.7spkg)|*.7spkg";
			this.SFD.Title = "Save Package";
			// 
			// CmdName
			// 
			this.CmdName.Location = new System.Drawing.Point(8, 146);
			this.CmdName.Name = "CmdName";
			this.CmdName.Size = new System.Drawing.Size(107, 20);
			this.CmdName.TabIndex = 3;
			this.CmdName.TextChanged += new System.EventHandler(this.CmdName_TextChanged);
			// 
			// Form1
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(404, 381);
			this.Controls.Add(this.flowLayoutPanel2);
			this.Controls.Add(this.CommandSpace);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.Name = "Form1";
			this.Text = "7Sharp Package Maker";
			this.flowLayoutPanel2.ResumeLayout(false);
			this.flowLayoutPanel2.PerformLayout();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.FlowLayoutPanel CommandSpace;
		private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel2;
		private System.Windows.Forms.Button SaveButton;
		private System.Windows.Forms.Button LoadButton;
		private System.Windows.Forms.Button AddBlock;
		private System.Windows.Forms.Timer UpdateClock;
		private System.Windows.Forms.OpenFileDialog OFD;
		private System.Windows.Forms.SaveFileDialog SFD;
		private System.Windows.Forms.TextBox CmdName;
	}
}

