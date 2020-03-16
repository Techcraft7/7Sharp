namespace _7Sharp.Editor
{
	partial class Editor
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Editor));
			this.codeBox = new System.Windows.Forms.TextBox();
			this.saveAndExitButton = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// codeBox
			// 
			this.codeBox.AcceptsReturn = true;
			this.codeBox.AcceptsTab = true;
			this.codeBox.Dock = System.Windows.Forms.DockStyle.Fill;
			this.codeBox.Font = new System.Drawing.Font("Consolas", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.codeBox.Location = new System.Drawing.Point(0, 0);
			this.codeBox.MaxLength = 2147483647;
			this.codeBox.Multiline = true;
			this.codeBox.Name = "codeBox";
			this.codeBox.Size = new System.Drawing.Size(384, 361);
			this.codeBox.TabIndex = 0;
			this.codeBox.TabStop = false;
			this.codeBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.CodeBox_KeyDown);
			// 
			// saveAndExitButton
			// 
			this.saveAndExitButton.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.saveAndExitButton.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.saveAndExitButton.Location = new System.Drawing.Point(0, 296);
			this.saveAndExitButton.Name = "saveAndExitButton";
			this.saveAndExitButton.Size = new System.Drawing.Size(384, 65);
			this.saveAndExitButton.TabIndex = 1;
			this.saveAndExitButton.TabStop = false;
			this.saveAndExitButton.Text = "Save and exit";
			this.saveAndExitButton.UseVisualStyleBackColor = true;
			this.saveAndExitButton.Click += new System.EventHandler(this.SaveAndExitButton_Click);
			// 
			// Editor
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(384, 361);
			this.Controls.Add(this.saveAndExitButton);
			this.Controls.Add(this.codeBox);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "Editor";
			this.Text = "Editor";
			this.Resize += new System.EventHandler(this.Editor_Resize);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.TextBox codeBox;
		private System.Windows.Forms.Button saveAndExitButton;
	}
}