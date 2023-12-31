using System.Windows.Forms;

namespace Translator.Explorer.OpenCL
{
	partial class DeviceSelection
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
			if (disposing && (components is not null))
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
			this.OurCancelButton = new System.Windows.Forms.Button();
			this.deviceList = new System.Windows.Forms.ListBox();
			this.SubmitButton = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// OurCancelButton
			// 
			this.OurCancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.OurCancelButton.Location = new System.Drawing.Point(347, 146);
			this.OurCancelButton.Name = "OurCancelButton";
			this.OurCancelButton.Size = new System.Drawing.Size(75, 23);
			this.OurCancelButton.TabIndex = 0;
			this.OurCancelButton.Text = "Cancel";
			this.OurCancelButton.UseVisualStyleBackColor = true;
			this.OurCancelButton.Click += new System.EventHandler(this.OurCancelButton_Click);
			// 
			// deviceList
			// 
			this.deviceList.FormattingEnabled = true;
			this.deviceList.ItemHeight = 15;
			this.deviceList.Location = new System.Drawing.Point(12, 12);
			this.deviceList.Name = "deviceList";
			this.deviceList.Size = new System.Drawing.Size(410, 109);
			this.deviceList.TabIndex = 1;
			// 
			// SubmitButton
			// 
			this.SubmitButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.SubmitButton.Location = new System.Drawing.Point(266, 146);
			this.SubmitButton.Name = "SubmitButton";
			this.SubmitButton.Size = new System.Drawing.Size(75, 23);
			this.SubmitButton.TabIndex = 2;
			this.SubmitButton.Text = "Submit";
			this.SubmitButton.UseVisualStyleBackColor = true;
			this.SubmitButton.Click += new System.EventHandler(this.SubmitButton_Click);
			// 
			// DeviceSelection
			// 
			this.AcceptButton = this.SubmitButton;
			this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(434, 181);
			this.Controls.Add(this.SubmitButton);
			this.Controls.Add(this.deviceList);
			this.Controls.Add(this.OurCancelButton);
			this.MaximizeBox = false;
			this.MaximumSize = new System.Drawing.Size(450, 220);
			this.MinimizeBox = false;
			this.MinimumSize = new System.Drawing.Size(450, 220);
			this.Name = "DeviceSelection";
			this.ShowIcon = false;
			this.ShowInTaskbar = false;
			this.Text = "Please select the device you want to use";
			this.UseWaitCursor = false;
			this.FormBorderStyle = FormBorderStyle.FixedDialog;
			this.ResumeLayout(false);

		}

		#endregion

		private Button OurCancelButton;
		private ListBox deviceList;
		private Button SubmitButton;
	}
}