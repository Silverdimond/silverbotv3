using SIlverCraftBot;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Text.Json;

namespace dotnetcorebot.Modules.infoclasses
{
    public class Config
    {
        public double? config_ver { get; set; } = null;

        public string Gtoken { get; set; } = "Giphy_Token_Here";
        public string Prefix { get; set; } = "sd!";
        public string Token { get; set; } = "Discord_Token_Here";
        public string Fortnite_Api_Token { get; set; } = "Fartnite_Token_Here";
        public string Location_to_java { get; set; } = "C:\\Program Files\\Java\\jdk-13\\bin\\java.exe";
        public ulong Log_channel { get; set; } = 717166388044628018;
        public ulong Suggestion_channel { get; set; } = 768558720086835250;
        public ulong Server_id { get; set; } = 679353407667961877;
        public ulong Bot_admin_role_id { get; set; } = 746821906602131506;
        public List<ulong> Botowners { get; set; } = new List<ulong> { 264081339316305920 };
        //public string YtToken { get; set; } //currently unused

        public string SentryURL { get; set; } = "https://something.ingest.sentry.io/someid";
        public string OpenWeatherMap { get; set; } = "Your_OpenWeatherMap.org_Api_Key_Here";

        public static Config Get()
        {
            try
            {
                using Stream stream = File.OpenRead("silverbot.json");
                StreamReader reader = new StreamReader(stream);
                string content = reader.ReadToEnd();
                reader.Dispose();
                Config asdf = JsonSerializer.Deserialize<Config>(content);
                if (asdf.config_ver == null || asdf.config_ver < new Config().config_ver)
                {
                    var e = new JsonSerializerOptions();
                    e.WriteIndented = true;
                    using (StreamWriter streamWriter = new StreamWriter("silverbot.json", false))
                    {
                        var config = new Config { config_ver = SIlverCraftBot.Version.config_ver };

                        streamWriter.Write(JsonSerializer.Serialize(config, e));
                    }
                    using (StreamWriter streamWriter = new StreamWriter("silverbotold.json", false))
                    {
                        streamWriter.Write(content);
                    }
                    if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                    {
                        Process.Start("notepad.exe", "silverbot.json");
                        Process.Start("notepad.exe", "silverbotold.json");
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("silverbot.json and silverbotold.json  should have opened in notepad, edit them, save silverbot.json and restart silverbot");
                        Console.WriteLine("Press any key to continue...");
                        Console.ReadKey();
                        Environment.Exit(420);
                        Environment.Exit(420);
                        Environment.Exit(420);
                        Environment.Exit(420);
                        Environment.Exit(420);
                        Environment.Exit(420);
                        Environment.Exit(420);
                        Environment.Exit(420);
                        Environment.Exit(420);
                        Environment.Exit(420);
                        Environment.Exit(420);
                        Environment.Exit(420);
                    }
                }
                return asdf;
            }
            catch (FileNotFoundException e)
            {
                using (StreamWriter streamWriter = new StreamWriter("silverbot.json"))
                {
                    var o = new JsonSerializerOptions();
                    o.WriteIndented = true;
                    var config = new Config { config_ver = SIlverCraftBot.Version.config_ver };
                    streamWriter.Write(JsonSerializer.Serialize(config, o));
                }
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    Process.Start("notepad.exe", "silverbot.json");
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("silverbot.json should have opened in notepad, edit it, save it and restart silverbot");
                    Console.WriteLine("Press any key to continue...");
                    Console.WriteLine(e);
                    Console.ReadKey();
                }
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("silverbot.json should open in nano, edit it, save it, restart silverbot");
                    Console.WriteLine("Press any key to continue...");
                    Console.WriteLine(e);
                    Console.ReadKey();
                    Process.Start("nano", "silverbot.json");
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("silverbot.json should exist, edit it, save it and restart silverbot");
                    Console.WriteLine("Press any key to continue...");
                    Console.WriteLine(e);
                    Console.ReadKey();
                    Process.Start("nano", "silverbot.json");
                }
                Environment.Exit(420);
                Environment.Exit(420);
                Environment.Exit(420);
                Environment.Exit(420);
                Environment.Exit(420);
                Environment.Exit(420);
                Environment.Exit(420);
                Environment.Exit(420);
                Environment.Exit(420);
                Environment.Exit(420);
                Environment.Exit(420);
                Environment.Exit(420);
                Config asdf = new Config();
                return asdf;
            }
        }
    }
}