/*
* This is a personal academic project. Dear PVS-Studio, please check it.
* PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com
*/
using CefSharp;

namespace KryBot.Gui.WinFormsGui
{
    public static class CefTools
    {
        public static string GetUserAgent()
        {
            return
                $"Mozilla/5.0 (Windows NT 10.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/{Cef.ChromiumVersion} Safari/537.36";
        }
    }
}