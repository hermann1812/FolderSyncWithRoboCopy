using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
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

        public Form1()
        {
            InitializeComponent();

            Timer timer1 = new Timer { Interval = 1000 };
            timer1.Enabled = true;
            timer1.Tick += new EventHandler(OnTimerEvent);
        }

        private void OnTimerEvent(object sender, EventArgs e)
        {
            UpdateFileCount();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Text = caption;

            // If available read the paths of last used directories
            if (File.Exists(txtFile))
            {
                string[] lines = File.ReadAllLines(txtFile);

                sourceFolder = lines[0];
                destinationFolder = lines[1];
            }
        }

        private void UpdateFileCount()
        {
            label_SourceFolder.Text = "Source Folder = " + sourceFolder;
            label_DestinationFolder.Text = "Destination Folder = " + destinationFolder;

            label_SourceAvailable.Text = CountFiles(sourceFolder) + " files currently available";
            label_DestinationAvailable.Text = CountFiles(destinationFolder) + " files currently available";

            if (CountFiles(sourceFolder) != CountFiles(destinationFolder))
            {
                button_StartSyncing.Enabled = true;
            }
            else
            {
                button_StartSyncing.Enabled = false;
            }

            if (CountFiles(destinationFolder) > CountFiles(sourceFolder))
            {
                label_Attention.Visible = true;
            }
            else
            {
                label_Attention.Visible = false;
            }
        }

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

        private void button_StartSyncing_Click(object sender, EventArgs e)
        {
            if (Directory.Exists(sourceFolder) && Directory.Exists(destinationFolder))
            {
                if (sourceFolder != destinationFolder)
                {
                    // During syncing button will be disabled
                    //button_StartSyncing.Enabled = false;

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
                MessageBox.Show("Please select valid directories!", caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

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
    }
}