namespace app
{
    partial class FormMain
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
            this.btnUpdatePlaylists = new System.Windows.Forms.Button();
            this.btnAddPlaylists = new System.Windows.Forms.Button();
            this.btnSelectPath = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // btnUpdatePlaylists
            // 
            this.btnUpdatePlaylists.Location = new System.Drawing.Point(66, 115);
            this.btnUpdatePlaylists.Name = "btnUpdatePlaylists";
            this.btnUpdatePlaylists.Size = new System.Drawing.Size(91, 23);
            this.btnUpdatePlaylists.TabIndex = 1;
            this.btnUpdatePlaylists.Text = "Update Playlists";
            this.btnUpdatePlaylists.UseVisualStyleBackColor = true;
            this.btnUpdatePlaylists.Click += new System.EventHandler(this.btnUpdatePlaylists_Click);
            // 
            // btnAddPlaylists
            // 
            this.btnAddPlaylists.Location = new System.Drawing.Point(66, 184);
            this.btnAddPlaylists.Name = "btnAddPlaylists";
            this.btnAddPlaylists.Size = new System.Drawing.Size(91, 23);
            this.btnAddPlaylists.TabIndex = 2;
            this.btnAddPlaylists.Text = "Add Playlists";
            this.btnAddPlaylists.UseVisualStyleBackColor = true;
            this.btnAddPlaylists.Click += new System.EventHandler(this.btnAddPlaylists_Click);
            // 
            // btnSelectPath
            // 
            this.btnSelectPath.Location = new System.Drawing.Point(66, 49);
            this.btnSelectPath.Name = "btnSelectPath";
            this.btnSelectPath.Size = new System.Drawing.Size(91, 23);
            this.btnSelectPath.TabIndex = 3;
            this.btnSelectPath.Text = "Select Path";
            this.btnSelectPath.UseVisualStyleBackColor = true;
            this.btnSelectPath.Click += new System.EventHandler(this.btnSelectPath_Click);
            // 
            // FormMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ControlDarkDark;
            this.ClientSize = new System.Drawing.Size(224, 247);
            this.Controls.Add(this.btnSelectPath);
            this.Controls.Add(this.btnAddPlaylists);
            this.Controls.Add(this.btnUpdatePlaylists);
            this.Name = "FormMain";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Download Playlist";
            this.Load += new System.EventHandler(this.FormMain_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnUpdatePlaylists;
        private System.Windows.Forms.Button btnAddPlaylists;
        private System.Windows.Forms.Button btnSelectPath;
    }
}

