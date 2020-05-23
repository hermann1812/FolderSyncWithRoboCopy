namespace FolderSyncWithRoboCopy
{
    partial class Form1
    {
        /// <summary>
        /// Erforderliche Designervariable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Verwendete Ressourcen bereinigen.
        /// </summary>
        /// <param name="disposing">True, wenn verwaltete Ressourcen gelöscht werden sollen; andernfalls False.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Vom Windows Form-Designer generierter Code

        /// <summary>
        /// Erforderliche Methode für die Designerunterstützung.
        /// Der Inhalt der Methode darf nicht mit dem Code-Editor geändert werden.
        /// </summary>
        private void InitializeComponent()
        {
            this.label_SourceFolder = new System.Windows.Forms.Label();
            this.button_ChangeSourceFolder = new System.Windows.Forms.Button();
            this.button_ChangeDestinationFolder = new System.Windows.Forms.Button();
            this.label_DestinationFolder = new System.Windows.Forms.Label();
            this.button_StartSyncing = new System.Windows.Forms.Button();
            this.label_SourceAvailable = new System.Windows.Forms.Label();
            this.label_DestinationAvailable = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.label_Attention = new System.Windows.Forms.Label();
            this.backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
            this.SuspendLayout();
            // 
            // label_SourceFolder
            // 
            this.label_SourceFolder.AutoSize = true;
            this.label_SourceFolder.Location = new System.Drawing.Point(12, 9);
            this.label_SourceFolder.Name = "label_SourceFolder";
            this.label_SourceFolder.Size = new System.Drawing.Size(85, 13);
            this.label_SourceFolder.TabIndex = 0;
            this.label_SourceFolder.Text = "Source Folder = ";
            // 
            // button_ChangeSourceFolder
            // 
            this.button_ChangeSourceFolder.Location = new System.Drawing.Point(12, 25);
            this.button_ChangeSourceFolder.Name = "button_ChangeSourceFolder";
            this.button_ChangeSourceFolder.Size = new System.Drawing.Size(143, 23);
            this.button_ChangeSourceFolder.TabIndex = 1;
            this.button_ChangeSourceFolder.Text = "Change Source Folder";
            this.button_ChangeSourceFolder.UseVisualStyleBackColor = true;
            this.button_ChangeSourceFolder.Click += new System.EventHandler(this.button_ChangeSourceFolder_Click);
            // 
            // button_ChangeDestinationFolder
            // 
            this.button_ChangeDestinationFolder.Location = new System.Drawing.Point(12, 83);
            this.button_ChangeDestinationFolder.Name = "button_ChangeDestinationFolder";
            this.button_ChangeDestinationFolder.Size = new System.Drawing.Size(143, 23);
            this.button_ChangeDestinationFolder.TabIndex = 3;
            this.button_ChangeDestinationFolder.Text = "Change Destination Folder";
            this.button_ChangeDestinationFolder.UseVisualStyleBackColor = true;
            this.button_ChangeDestinationFolder.Click += new System.EventHandler(this.button_ChangeDestinationFolder_Click);
            // 
            // label_DestinationFolder
            // 
            this.label_DestinationFolder.AutoSize = true;
            this.label_DestinationFolder.Location = new System.Drawing.Point(12, 67);
            this.label_DestinationFolder.Name = "label_DestinationFolder";
            this.label_DestinationFolder.Size = new System.Drawing.Size(104, 13);
            this.label_DestinationFolder.TabIndex = 2;
            this.label_DestinationFolder.Text = "Destination Folder = ";
            // 
            // button_StartSyncing
            // 
            this.button_StartSyncing.Location = new System.Drawing.Point(12, 129);
            this.button_StartSyncing.Name = "button_StartSyncing";
            this.button_StartSyncing.Size = new System.Drawing.Size(80, 23);
            this.button_StartSyncing.TabIndex = 4;
            this.button_StartSyncing.Text = "Start Syncing";
            this.button_StartSyncing.UseVisualStyleBackColor = true;
            this.button_StartSyncing.Click += new System.EventHandler(this.button_StartSyncing_Click);
            // 
            // label_SourceAvailable
            // 
            this.label_SourceAvailable.AutoSize = true;
            this.label_SourceAvailable.Location = new System.Drawing.Point(161, 30);
            this.label_SourceAvailable.Name = "label_SourceAvailable";
            this.label_SourceAvailable.Size = new System.Drawing.Size(126, 13);
            this.label_SourceAvailable.TabIndex = 5;
            this.label_SourceAvailable.Text = "xx files currently available";
            // 
            // label_DestinationAvailable
            // 
            this.label_DestinationAvailable.AutoSize = true;
            this.label_DestinationAvailable.Location = new System.Drawing.Point(161, 88);
            this.label_DestinationAvailable.Name = "label_DestinationAvailable";
            this.label_DestinationAvailable.Size = new System.Drawing.Size(126, 13);
            this.label_DestinationAvailable.TabIndex = 6;
            this.label_DestinationAvailable.Text = "xx files currently available";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(98, 134);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(218, 13);
            this.label1.TabIndex = 7;
            this.label1.Text = "Files may be deleted in the destination folder.";
            // 
            // label_Attention
            // 
            this.label_Attention.AutoSize = true;
            this.label_Attention.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label_Attention.ForeColor = System.Drawing.Color.Red;
            this.label_Attention.Location = new System.Drawing.Point(322, 99);
            this.label_Attention.Name = "label_Attention";
            this.label_Attention.Size = new System.Drawing.Size(230, 48);
            this.label_Attention.TabIndex = 8;
            this.label_Attention.Text = "Attention!\r\nThere are more files in the destination\r\nfolder than in the source fo" +
    "lder!";
            this.label_Attention.Visible = false;
            // 
            // backgroundWorker1
            // 
            this.backgroundWorker1.WorkerReportsProgress = true;
            this.backgroundWorker1.WorkerSupportsCancellation = true;
            this.backgroundWorker1.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundWorker1_DoWork);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(560, 163);
            this.Controls.Add(this.label_Attention);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.label_DestinationAvailable);
            this.Controls.Add(this.label_SourceAvailable);
            this.Controls.Add(this.button_StartSyncing);
            this.Controls.Add(this.button_ChangeDestinationFolder);
            this.Controls.Add(this.label_DestinationFolder);
            this.Controls.Add(this.button_ChangeSourceFolder);
            this.Controls.Add(this.label_SourceFolder);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Folder Sync With RoboCopy";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label_SourceFolder;
        private System.Windows.Forms.Button button_ChangeSourceFolder;
        private System.Windows.Forms.Button button_ChangeDestinationFolder;
        private System.Windows.Forms.Label label_DestinationFolder;
        private System.Windows.Forms.Button button_StartSyncing;
        private System.Windows.Forms.Label label_SourceAvailable;
        private System.Windows.Forms.Label label_DestinationAvailable;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label_Attention;
        private System.ComponentModel.BackgroundWorker backgroundWorker1;
    }
}

