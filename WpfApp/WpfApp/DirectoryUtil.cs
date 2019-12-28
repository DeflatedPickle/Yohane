using System;
using System.IO;
using System.Windows;

namespace WpfApp
{
    public class DirectoryUtil
    {
        public static void RecurseDirectory(string directory, Action<string> function)
        {
            var fileList = Directory.GetFiles(directory);

            foreach (var file in fileList)
            {
                function.Invoke(file);
            }

            var directoryList = Directory.GetDirectories(directory);

            foreach (var subDirectory in directoryList)
            {
                RecurseDirectory(subDirectory, function);
            }
        }
    }
}