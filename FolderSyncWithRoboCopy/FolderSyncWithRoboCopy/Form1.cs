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

        // OOOOOOOO
        Thread oThreadUpdateLabel;
        Thread oThreadStartCopy;

        public Form1()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Things to be done when form is loaded
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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

            // Starts thread which shows the number of files in the directories
            oThreadUpdateLabel = new Thread(new ThreadStart(ThreadUpdateLabels));
            oThreadUpdateLabel.Start();
        }

        /// <summary>
        /// Thread which updates the labels showing the number of files in the source and destination folder
        /// </summary>
        private void ThreadUpdateLabels()
        {
            while (true)
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
                    oThreadUpdateLabel.Abort();
                    MessageBox.Show("Exception = " + ex);
                }

                return fCount;
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
                    oThreadStartCopy = new Thread(new ThreadStart(ThreadStartCopy));
                    oThreadStartCopy.Start();
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

        private void ThreadStartCopy()
        {
            button_StartSyncing.Invoke((MethodInvoker)(() => button_StartSyncing.Enabled = false));

            SetProgressBarAnimationSpeed(30);

            // Save the last used folder paths
            if (File.Exists(txtFile)) { File.Delete(txtFile); }
            File.WriteAllText(txtFile, sourceFolder + "\r\n" + destinationFolder);

            // Start syncing by using the Windows command "Robocopy"
            Process process = new Process();
            process.StartInfo.FileName = "robocopy";
            process.StartInfo.Arguments = " /MIR \"" + sourceFolder + "\" \"" + destinationFolder + "\"";

            process.StartInfo.UseShellExecute = false;
            process.StartInfo.CreateNoWindow = true;

            process.Start();
            process.WaitForExit();

            SetProgressBarAnimationSpeed(0);

            button_StartSyncing.Invoke((MethodInvoker)(() => button_StartSyncing.Enabled = true));
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

        private delegate void SetProgressBarAnimationSpeedDelegate(int animationSpeed);

        private void SetProgressBarAnimationSpeed(int animationSpeed)
        {
            if (progressBar1.InvokeRequired)
            {
                progressBar1.Invoke(new SetProgressBarAnimationSpeedDelegate(SetProgressBarAnimationSpeed), animationSpeed);
            }
            else
            {
                //0000
                progressBar1.MarqueeAnimationSpeed = animationSpeed;

                if (animationSpeed == 0)
                {
                    progressBar1.Value = 0;
                    progressBar1.Style = ProgressBarStyle.Blocks;
                }
                else
                {
                    progressBar1.Style = ProgressBarStyle.Marquee;
                }
            }
        }

        /// <summary>
        /// Method to stop all running threads
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (oThreadUpdateLabel != null) { oThreadUpdateLabel.Abort(); }

            if (oThreadStartCopy != null) { oThreadStartCopy.Abort(); }
        }
    }
}