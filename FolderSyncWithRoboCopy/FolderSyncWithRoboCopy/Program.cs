using System;
using System.IO;
using System.Reflection;
using System.Windows.Forms;

namespace FolderSyncWithRoboCopy
{
    static class Program
    {
        // Source and destination folder
        public static string sourceFolder = string.Empty;
        public static string destinationFolder = string.Empty;

        // The last used directories will be saved in this text file
        public static string directories = Path.Combine(Path.GetTempPath(), "FolderSync_Directories.txt");

        // Errors will be logged in this text file
        public static string errors = Path.Combine(Path.GetTempPath(), "FolderSync_Errors.log");

        // Journal file
        public static string journal = Path.Combine(Path.GetTempPath(), "FolderSync_Journal.txt");

        /// <summary>
        /// Der Haupteinstiegspunkt für die Anwendung.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            if (File.Exists(errors)) { File.Delete(errors); }

            if (File.Exists(journal)) { File.Delete(journal); }

            Application.Run(new Form1(directories, errors, journal));
        }
    }
}