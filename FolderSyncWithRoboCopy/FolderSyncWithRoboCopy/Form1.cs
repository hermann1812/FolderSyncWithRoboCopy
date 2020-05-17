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

        // Errors will be logged in this text file
        string logFile = Path.Combine(Path.GetTempPath(), "FolderSyncWithRoboCopy.log");

        // Bool to stop the BackgroundWorker
        bool weitermachen = true;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
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

            // Start BackgroundWorker
            backgroundWorker1.RunWorkerAsync();
        }

        private void UpdateFileCount()
        {
            // Shows the number of files in the source and destination folder
            label_SourceFolder.Invoke((MethodInvoker)(() => label_SourceFolder.Text = "Source Folder = " + sourceFolder));
            label_DestinationFolder.Invoke((MethodInvoker)(() => label_DestinationFolder.Text = "Destination Folder = " + destinationFolder));

            label_SourceAvailable.Invoke((MethodInvoker)(() => label_SourceAvailable.Text = CountFiles(sourceFolder) + " files currently available"));
            label_DestinationAvailable.Invoke((MethodInvoker)(() => label_DestinationAvailable.Text = CountFiles(destinationFolder) + " files currently available"));

            // Shows a warning message if there are already more files in the destination folder than in the source folder
            if (CountFiles(destinationFolder) > CountFiles(sourceFolder))
            {
                label_Attention.Invoke((MethodInvoker)(() => label_Attention.Visible = true));
            }
            else
            {
                label_Attention.Invoke((MethodInvoker)(() => label_Attention.Visible = false));
            }
        }

        /// <summary>
        /// Method to count the files in a directory and its subdirectories
        /// </summary>
        /// <param name="folder"></param>
        /// <returns></returns>
        private int CountFiles(string folder)
        {
            int fCount = 0;

            if (Directory.Exists(folder))
            {
                try
                {
                    fCount = Directory.GetFiles(folder, "*", SearchOption.AllDirectories).Length;
                }
                catch (Exception ex)
                {
                    WriteToLogFile(logFile, ex);
                }
            }
            return fCount;
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
        }

        /// <summary>
        /// Method to start the syncing process
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button_StartSyncing_Click(object sender, EventArgs e)
        {
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

                    //process.StartInfo.UseShellExecute = false;
                    //process.StartInfo.CreateNoWindow = true;

                    process.Start();
                    process.WaitForExit();
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
        }

        /// <summary>
        /// Method to write to log-file
        /// </summary>
        /// <param name="logFile"></param>
        /// <param name="ex"></param>
        private void WriteToLogFile(string logFile, Exception ex)
        {
            MessageBox.Show(ex.ToString(), caption, MessageBoxButtons.OK, MessageBoxIcon.Error);

            if (File.Exists(logFile))
            {
                File.AppendAllText(logFile, "\r\n=========> " + DateTime.Now + " <=========\r\n" + ex.ToString());
            }
            else
            {
                File.WriteAllText(logFile, "\r\n=========> " + DateTime.Now + " <=========\r\n" + ex.ToString());
            }
        }

        /// <summary>
        /// BackgroundWorker
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void backgroundWorker1_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            while (weitermachen)
            {
                UpdateFileCount();
                Thread.Sleep(1000);
            }
        }

        /// <summary>
        /// Program exit
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            // Stop BackgroundWorker
            if (backgroundWorker1.IsBusy)
            {
                weitermachen = false;
                backgroundWorker1.CancelAsync();
            }
        }
    }
}