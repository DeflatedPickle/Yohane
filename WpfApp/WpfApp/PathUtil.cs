using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Win32;

namespace WpfApp
{
    public class PathUtil
    {

        public static Dictionary<string, string> PathDict = new Dictionary<string, string>();
        
        public static List<string> DiscoverPaths()
        {
            var paths = new List<string>();
            
            DirectoryUtil.RecurseDirectory(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                s =>
                {
                    var path = FileUtil.GetShortcutTarget(s);
                    if (path != null)
                    {
                        var cleanPath = s.Split('\\').Last().Split('.').First();
                        PathDict[path] = cleanPath;
                        paths.Add(cleanPath);
                    }
                });

            using (var key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall"))
            {
                foreach (var subKeyName in key.GetSubKeyNames())
                {
                    using (var subKey = key.OpenSubKey(subKeyName))
                    {
                        var displayName = subKey.GetValue("DisplayName") as string;
                        var installLocation = subKey.GetValue("InstallLocation") as string;

                        if (displayName != null && installLocation != null && !PathDict.ContainsKey(displayName))
                        {
                            PathDict[installLocation] = displayName;
                            paths.Add(displayName);
                        }
                    }
                }
            }

            foreach (var path in Environment.GetEnvironmentVariable("Path").Split(';'))
            {
                if (path.EndsWith(@"\"))
                {
                    if (Directory.Exists(path))
                    {
                        DirectoryUtil.RecurseDirectory(path, (s) =>
                        {
                            var name = s.Split('\\').Last();

                            if (!PathDict.ContainsKey(name))
                            {
                                PathDict[path] = name;
                                paths.Add(name);
                            }
                        });
                    }
                }
                else
                {
                    var name = path.Split('\\').Last();

                    if (!PathDict.ContainsKey(name))
                    {
                        PathDict[path] = name;
                        paths.Add(name);
                    }
                }
            }

            return paths;
        }
    }
}