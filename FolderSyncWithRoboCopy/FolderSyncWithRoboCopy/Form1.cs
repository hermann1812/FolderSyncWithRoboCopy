using System;
using System.Diagnostics;
using System.Drawing.Text;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Windows.Forms;

namespace FolderSyncWithRoboCopy
{
    public partial class Form1 : Form
    {
        // Caption for MessageBox
        public static readonly string caption = "Folder Sync With RoboCopy - " + Assembly.GetEntryAssembly().GetName().Version;

        string sourceFolder = string.Empty;
        string destinationFolder = string.Empty;

        bool showJournal = false;

        int filesInSourceFolder = -1;
        int filesInDesinationFolder = -1;

        public Form1(string x_directories, string x_errors, string x_journal)
        {
            // Caption for form
            Text = caption;

            directories = x_directories;
            errors = x_errors;
            journal = x_journal;

            // If available read the paths of last used directories
            if (File.Exists(directories))
            {
                string[] lines = File.ReadAllLines(directories);

                sourceFolder = lines[0];
                destinationFolder = lines[1];
            }

            InitializeComponent();

        }
        private static string directories;
        private static string errors;
        private static string journal;


        /// <summary>
        /// Things to be done when form is loaded
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form1_Load(object sender, EventArgs e)
        {
            filesInSourceFolder = CountFiles(sourceFolder, errors);
            filesInDesinationFolder = CountFiles(destinationFolder, errors);

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
            }
        }

        /// <summary>
        /// Method to count the files in a directory and its subdirectories
        /// </summary>
        /// <param name="folder"></param>
        /// <returns></returns>
        private int CountFiles(string folder, string errors)
        {
            try
            {
                return Directory.GetFiles(folder, "*.*", SearchOption.AllDirectories).Length;
            }
            catch (Exception ex)
            {
                File.WriteAllText(errors, ex.Message);
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
            button_StartSyncing.Enabled = false;

            // Save the last used folder paths
            if (File.Exists(directories)) { File.Delete(directories); }
            File.WriteAllText(directories, sourceFolder + "\r\n" + destinationFolder);

            new Thread(StartCopy).Start();
        }

        private void StartCopy()
        {
            // Start Progressbar
            Invoke((MethodInvoker)delegate () { progressBar1.Style = ProgressBarStyle.Marquee; });
            Invoke((MethodInvoker)delegate () { progressBar1.MarqueeAnimationSpeed = 100; });

            Invoke((MethodInvoker)delegate () { textBox1.Clear(); });

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

            Form1_Load(null, null);
        }
    }
}