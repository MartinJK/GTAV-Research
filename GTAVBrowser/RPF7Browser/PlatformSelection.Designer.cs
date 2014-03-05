/*
 
    GTANETWORKV - Viewer/Editor for RAGE Package File version 7
    Copyright (C) 2013  koolk <koolkdev at gmail.com>
   
    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.
  
    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.
   
    You should have received a copy of the GNU General Public License
    along with this program.  If not, see <http://www.gnu.org/licenses/>.
 
 */

namespace GTANETWORKV
{
    partial class PlatformSelection
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
            this.xboxSelect = new System.Windows.Forms.Button();
            this.playstationSelect = new System.Windows.Forms.Button();
            this.closeButton = new System.Windows.Forms.Button();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.process1 = new System.Diagnostics.Process();
            this.label1 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // xboxSelect
            // 
            this.xboxSelect.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(177)));
            this.xboxSelect.Location = new System.Drawing.Point(12, 197);
            this.xboxSelect.Name = "xboxSelect";
            this.xboxSelect.Size = new System.Drawing.Size(150, 50);
            this.xboxSelect.TabIndex = 0;
            this.xboxSelect.Text = "Xbox 360";
            this.xboxSelect.UseVisualStyleBackColor = true;
            this.xboxSelect.Click += new System.EventHandler(this.xboxSelect_Click);
            // 
            // playstationSelect
            // 
            this.playstationSelect.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(177)));
            this.playstationSelect.Location = new System.Drawing.Point(536, 197);
            this.playstationSelect.Name = "playstationSelect";
            this.playstationSelect.Size = new System.Drawing.Size(150, 50);
            this.playstationSelect.TabIndex = 1;
            this.playstationSelect.Text = "PlayStation 3";
            this.playstationSelect.UseVisualStyleBackColor = true;
            this.playstationSelect.Click += new System.EventHandler(this.playstationSelect_Click);
            // 
            // closeButton
            // 
            this.closeButton.Location = new System.Drawing.Point(611, 12);
            this.closeButton.Name = "closeButton";
            this.closeButton.Size = new System.Drawing.Size(75, 23);
            this.closeButton.TabIndex = 2;
            this.closeButton.Text = "Exit";
            this.closeButton.UseVisualStyleBackColor = true;
            this.closeButton.Click += new System.EventHandler(this.closeButton_Click);
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = global::GTANETWORKV.Properties.Resources.logo;
            this.pictureBox1.Location = new System.Drawing.Point(12, 12);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(471, 90);
            this.pictureBox1.TabIndex = 3;
            this.pictureBox1.TabStop = false;
            // 
            // process1
            // 
            this.process1.StartInfo.Domain = "";
            this.process1.StartInfo.LoadUserProfile = false;
            this.process1.StartInfo.Password = null;
            this.process1.StartInfo.StandardErrorEncoding = null;
            this.process1.StartInfo.StandardOutputEncoding = null;
            this.process1.StartInfo.UserName = "";
            this.process1.SynchronizingObject = this;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 20.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(266, 94);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(351, 31);
            this.label1.TabIndex = 4;
            this.label1.Text = "GTA 5 RPF Tools - Browser";
            // 
            // PlatformSelection
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(698, 259);
            this.ControlBox = false;
            this.Controls.Add(this.label1);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.closeButton);
            this.Controls.Add(this.playstationSelect);
            this.Controls.Add(this.xboxSelect);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "PlatformSelection";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Welcome - Please Select Your Platform";
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button xboxSelect;
        private System.Windows.Forms.Button playstationSelect;
        private System.Windows.Forms.Button closeButton;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Diagnostics.Process process1;
        private System.Windows.Forms.Label label1;
    }
}