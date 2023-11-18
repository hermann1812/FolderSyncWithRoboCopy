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
        string directories = Path.Combine(Path.GetTempPath(), "FolderSync_Directories.txt");

        // Errors will be logged in this text file (currently not used)
        string errors = Path.Combine(Path.GetTempPath(), "FolderSync_Errors.log");

        // Journal file
        string journal = Path.Combine(Path.GetTempPath(), "FolderSync_Journal.txt");

        bool showJournal = true;

        int filesInSourceFolder = -1;
        int filesInDesinationFolder = -1;

        public Form1()
        {
            // Caption for form
            Text = caption;

            // If available read the paths of last used directories
            if (File.Exists(directories))
            {
                string[] lines = File.ReadAllLines(directories);

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
            filesInSourceFolder = CountFiles(sourceFolder);
            filesInDesinationFolder = CountFiles(destinationFolder);

            label_SourceFolder.Text = "Source Folder = " + sourceFolder;
            label_DestinationFolder.Text = "Destination Folder = " + destinationFolder;

            label_SourceAvailable.Text = filesInSourceFolder + " files currently available";
            label_DestinationAvailable.Text = filesInDesinationFolder + " files currently available";

            if (filesInDesinationFolder > filesInSourceFolder)
            {
                label_Attention.Visible = true;
            }
            else
            {
                label_Attention.Visible = false;
            }

            if (filesInSourceFolder == -1 || filesInDesinationFolder == -1)
            {
                button_StartSyncing.Enabled = false;
            }
            else
            {
                button_StartSyncing.Enabled = true;
            }

            if (sourceFolder == destinationFolder)
            {
                button_StartSyncing.Enabled = false;
                MessageBox.Show("Please select 2 different folder!", caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Method to count the files in a directory and its subdirectories
        /// </summary>
        /// <param name="folder"></param>
        /// <returns></returns>
        private int CountFiles(string folder)
        {
            try
            {
                return Directory.GetFiles(folder, "*.*", SearchOption.AllDirectories).Length;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
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
            new Thread(StartCopy).Start();
        }

        private void StartCopy()
        {
            // Start Progressbar
            Invoke((MethodInvoker)delegate () { progressBar1.Style = ProgressBarStyle.Marquee; });
            Invoke((MethodInvoker)delegate () { progressBar1.MarqueeAnimationSpeed = 100; });

            Invoke((MethodInvoker)delegate () { textBox1.Clear(); });

            // Save the last used folder paths
            if (File.Exists(directories)) { File.Delete(directories); }
            File.WriteAllText(directories, sourceFolder + "\r\n" + destinationFolder);

            // Start syncing by using the Windows command "Robocopy"
            Process process = new Process();
            process.StartInfo.FileName = "robocopy";
            process.StartInfo.Arguments = " " + sourceFolder + " " + destinationFolder + " /mir /tee /unilog:" + journal;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.CreateNoWindow = true;
            process.Start();
            process.WaitForExit();

            // Output to TextBox
            Invoke((MethodInvoker)delegate () { textBox1.Text = File.ReadAllText(journal); });
            Invoke((MethodInvoker)delegate () { textBox1.Focus(); });
            Invoke((MethodInvoker)delegate () { textBox1.ScrollToCaret(); });

            // Show Log File
            if (showJournal)
            {
                Process.Start(journal);
            }

            // Stop Progressbar
            Invoke((MethodInvoker)delegate () { progressBar1.Style = ProgressBarStyle.Blocks; });
            Invoke((MethodInvoker)delegate () { progressBar1.Value = 0; });

            Invoke((MethodInvoker)delegate () { Form1_Load(null, null); });
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked)
            {
                showJournal = true;
            }
            else
            {
                showJournal = false;
            }
        }
    }
}