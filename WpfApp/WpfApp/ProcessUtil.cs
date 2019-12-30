using System.Diagnostics;
using System.IO;

namespace WpfApp
{
    public class ProcessUtil
    {
        public static void OpenProcess(string path)
        {
            using (var process = new Process())
            {
                process.StartInfo.FileName = path;
                process.StartInfo.WorkingDirectory = Path.GetDirectoryName(path);
                process.StartInfo.ErrorDialog = true;
                process.Start();
            }
        }
    }
}