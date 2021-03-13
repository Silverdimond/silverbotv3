using SIlverCraftBot.Modules;
using System;
using System.Diagnostics;
using System.IO;
using System.Net.Http;

namespace SIlverCraftBot
{
    internal static class Version
    {
        public const string vnumber = ThisAssembly.Git.Commit + "-" + ThisAssembly.Git.Branch + "-" + ThisAssembly.Git.CommitDate;
        public const double config_ver = 0.02;

        public static async void Checkforupdates()
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Running on " + System.Environment.OSVersion.VersionString);
            Console.ResetColor();
            HttpClient client = Webclient.Get();
            HttpResponseMessage rm = await client.GetAsync("https://silverdimond.tk/silvercraftbot/version-info.txt");
            string _content = await rm.Content.ReadAsStringAsync();
            string[] strings = _content.Split("\n", StringSplitOptions.RemoveEmptyEntries);
            bool uptodate = true;
            if (strings.Length != 3)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Oh oh someone made an oopsie making the strings not 3. they are curently " + strings.Length);
                Console.ResetColor();
            }
            if (strings[0] != vnumber)
            {
                Console.ForegroundColor = ConsoleColor.DarkBlue;
                Console.WriteLine("You are currently running {0} while the latest version is {1}", vnumber, strings[0]);
                Console.ResetColor();
                uptodate = false;
            }
            if (uptodate)
            {
                Console.ForegroundColor = ConsoleColor.DarkBlue;
                Console.WriteLine("You are currently running {0} which is the latest version according to silverdimond.tk", vnumber);
                Console.ResetColor();
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("You should go to {0} to download a new version of SilverCraftBot", strings[2]);
                Console.ResetColor();
            }
#if DEBUG
            if (!uptodate && (Environment.UserDomainName == null || Environment.UserDomainName == "DESKTOP-QK1H9BG"))
            {
                Console.WriteLine(Environment.UserDomainName);
                using (StreamWriter sw = new StreamWriter("version-info.txt"))
                {
                    sw.WriteLine(vnumber);
                    sw.WriteLine("codname here");
                    sw.WriteLine(ThisAssembly.Git.RepositoryUrl);
                }
                Process.Start("notepad", "version-info.txt");
            }

#endif
        }
    }
}