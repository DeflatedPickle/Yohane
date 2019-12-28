using System.Windows;
using IWshRuntimeLibrary;

namespace WpfApp
{
    public class FileUtil
    {
        public static WshShell shell = new WshShell();
        
        public static string GetShortcutTarget(string shortcut)
        {
            if (System.IO.File.Exists(shortcut) && shortcut != "" && shortcut.EndsWith(".lnk"))
            {
                var link = (IWshShortcut) shell.CreateShortcut(shortcut);

                return link.TargetPath;
            }

            return null;
        }
    }
}