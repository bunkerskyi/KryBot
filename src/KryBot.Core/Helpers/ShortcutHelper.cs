/*
* This is a personal academic project. Dear PVS-Studio, please check it.
* PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com
*/
using System;
using IWshRuntimeLibrary;

namespace KryBot.Core.Helpers
{
    public static class ShortcutHelper
    {
        public static void Create()
        {
            var shortcut = new WshShell()
                    .CreateShortcut(
                        $"{Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory)}\\KryBot.lnk")
                as IWshShortcut;
            if (shortcut != null)
            {
                shortcut.TargetPath = $"{Environment.CurrentDirectory}\\KryBot.exe";
                shortcut.WorkingDirectory = Environment.CurrentDirectory;
                shortcut.Save();
            }
        }
    }
}