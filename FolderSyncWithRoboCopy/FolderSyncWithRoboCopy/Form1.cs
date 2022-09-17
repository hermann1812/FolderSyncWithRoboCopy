using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Windows.Forms;

namespace FolderSyncWithRoboCopy
{
    public partial class Form1 : Form
    {
        // Caption for MessageBox
        readonly string caption = "Folder Sync With RoboCopy - " + Assembly.GetEntryAssembly().GetName().Version;

        // Source and destination folder
        string sourceFolder = string.Empty;
        string destinationFolder = string.Empty;

        // The last used directories will be saved in this text file
        string txtFile = Path.Combine(Path.GetTempPath(), "FolderSyncWithRoboCopy.txt");

        // Errors will be logged in this text file (currently not used)
        string logFile = Path.Combine(Path.GetTempPath(), "FolderSyncWithRoboCopy.log");

        public Form1()
        {
            // Caption for form
            Text = caption;

            // If available read the paths of last used directories
            if (File.Exists(txtFile))
            {
                string[] lines = File.ReadAllLines(txtFile);

                sourceFolder = lines[0];
                destinationFolder = lines[1];
            }

            InitializeComponent();
        }

        /// <summary>
        /// Things to be done when form is loaded
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form1_Load(object sender, EventArgs e)
        {
            label_SourceFolder.Text = "Source Folder = " + sourceFolder;
            label_DestinationFolder.Text = "Destination Folder = " + destinationFolder;

            label_SourceAvailable.Text = CountFiles(sourceFolder) + " files currently available";
            label_DestinationAvailable.Text = CountFiles(destinationFolder) + " files currently available";

            if (CountFiles(destinationFolder) > CountFiles(sourceFolder))
            {
                label_Attention.Visible = true;
            }
            else
            {
                label_Attention.Visible = false;
            }
        }

        /// <summary>
        /// Method to count the files in a directory and its subdirectories
        /// </summary>
        /// <param name="folder"></param>
        /// <returns></returns>
        private int CountFiles(string folder)
        {
            if (Directory.Exists(folder))
            {
                return Directory.GetFiles(folder, "*", SearchOption.AllDirectories).Length;
            }
            else
            {
                return -1;
            }
        }

        /// <summary>
        /// Method to change the source folder
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button_ChangeSourceFolder_Click(object sender, EventArgs e)
        {
            var fbd = new FolderBrowserDialog();

            DialogResult result = fbd.ShowDialog();

            if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(fbd.SelectedPath))
            {
                sourceFolder = fbd.SelectedPath;

                label_SourceFolder.Text = "Source Folder = " + sourceFolder;
            }

            Form1_Load(null, null);
        }

        /// <summary>
        /// Method to change the destination folder
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button_ChangeDestinationFolder_Click(object sender, EventArgs e)
        {
            var fbd = new FolderBrowserDialog();

            DialogResult result = fbd.ShowDialog();

            if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(fbd.SelectedPath))
            {
                destinationFolder = fbd.SelectedPath;

                label_DestinationFolder.Text = "Destination Folder = " + destinationFolder;
            }

            Form1_Load(null, null);
        }

        /// <summary>
        /// Method to start the syncing process
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button_StartSyncing_Click(object sender, EventArgs e)
        {
            textBox1.Clear();

            if (Directory.Exists(sourceFolder) && Directory.Exists(destinationFolder))
            {
                if (sourceFolder != destinationFolder)
                {
                    // Save the last used folder paths
                    if (File.Exists(txtFile)) { File.Delete(txtFile); }
                    File.WriteAllText(txtFile, sourceFolder + "\r\n" + destinationFolder);

                    // Start syncing by using the Windows command "Robocopy"
                    Process process = new Process();
                    process.StartInfo.FileName = "robocopy";
                    process.StartInfo.Arguments = " /MIR \"" + sourceFolder + "\" \"" + destinationFolder + "\"";

                    process.StartInfo.UseShellExecute = false;
                    process.StartInfo.CreateNoWindow = true;

                    process.StartInfo.RedirectStandardOutput = true;
                    process.StartInfo.RedirectStandardError = true;
                    process.StartInfo.RedirectStandardInput = true;

                    process.Start();

                    string stdOutput = process.StandardOutput.ReadToEnd();
                    string stdError = process.StandardError.ReadToEnd();

                    process.WaitForExit();

                    textBox1.AppendText(stdOutput);
                    textBox1.AppendText(stdError);
                    textBox1.ScrollToCaret();
                }
                else
                {
                    MessageBox.Show("Please select 2 different folder!", caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                // Source folder cannot be accessed
                if (!Directory.Exists(sourceFolder))
                {
                    MessageBox.Show("Please select valid source folder!", caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

                // Destination folder cannot be accessed
                if (!Directory.Exists(destinationFolder))
                {
                    MessageBox.Show("Please select valid destination folder!", caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            Form1_Load(null, null);
        }
    }
}