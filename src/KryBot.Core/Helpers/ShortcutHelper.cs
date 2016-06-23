using System;
using IWshRuntimeLibrary;

namespace KryBot.Core.Helpers
{
    public static class ShortcutHelper
    {
        public static void Create()
        {
            IWshShortcut shortcut = new WshShell()
                .CreateShortcut($"{Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory)}\\KryBot.lnk") 
                as IWshShortcut;
            if (shortcut != null)
            {
                shortcut.TargetPath = $"{Environment.CurrentDirectory}\\KryBot.Gui.WinFormsGui.exe";
                shortcut.WorkingDirectory = Environment.CurrentDirectory;
                shortcut.Save();
            }
        }
    }
}
