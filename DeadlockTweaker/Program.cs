using Microsoft.Win32;
using Spectre.Console;
using System.Text.RegularExpressions;

namespace DeadlockTweaker;

class Program
{
    static async Task Main(string[] args)
    {
        Console.Title = "Deadlock Tweaker";

        StarterInfo();
        string deadlockPath = @"C:\Program Files (x86)\Steam\steamapps\common\Deadlockk";
        bool deadlockExists = true;
        string[] actions = ["1", "2"];
        string[] language_actions = ["1", "2"];
        Console.ForegroundColor = ConsoleColor.Red;

        string registryPath = @"Software\DeadlockTweaker"; // раздел в реестре
        string rootPath_KEY_name = "DeadlockPath";              // ключ root_path внутри раздела
        string language_KEY_name = "DeadlockTweakerLanguage";   // ключ language внутри раздела

        RegistryKey registry_key = Registry.CurrentUser.CreateSubKey(registryPath);
        LangHelper.ChangeLanguage(registry_key.GetValue(language_KEY_name)?.ToString() ?? "en");

        while (string.IsNullOrEmpty(registry_key.GetValue(language_KEY_name)?.ToString()))
        {
            LanguageChoose(language_actions, language_KEY_name, registry_key);
        }

        if (!Directory.Exists(deadlockPath))
        {
            // Читаем данные
            var key = Registry.CurrentUser.OpenSubKey(registryPath);
            if (key != null)
            {
                deadlockPath = key.GetValue(rootPath_KEY_name)?.ToString() ?? deadlockPath;
            }
        }

        do
        {
            Console.Clear();
            StarterInfo();
            Console.ForegroundColor = ConsoleColor.Red;
            if (!Directory.Exists(deadlockPath))
            {
                Console.WriteLine(LangHelper.GetString("Directory_Error") + $" {deadlockPath}");
                deadlockExists = false;

                Console.WriteLine(LangHelper.GetString("Enter_Root_Path"));
                Console.ForegroundColor = ConsoleColor.Gray;
                string path = Console.ReadLine().ToString();

                while (string.IsNullOrEmpty(path))
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine(LangHelper.GetString("Empty_Input_Error"));
                    Console.ForegroundColor = ConsoleColor.Gray;
                    path = Console.ReadLine().ToString();
                }

                if (path != null)
                {
                    // Сохраняем данные
                    registry_key.SetValue(rootPath_KEY_name, path);
                    deadlockPath = path;
                }
            }
            else
            {
                deadlockExists = true;
            }
        }
        while (!deadlockExists);

        while (true)
        {
            Console.Clear();
            StarterInfo();
            ActionChoose();
            string actionNum = Console.ReadLine().ToString();

            while (!actions.Contains(actionNum))
            {
                Console.Clear();
                StarterInfo();
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(LangHelper.GetString("Incorrect_Input"));
                ActionChoose();
                actionNum = Console.ReadLine().ToString();
            }

            Console.Clear();
            StarterInfo();

            if (actionNum == "1")
            {
                string gameinfoPath = deadlockPath + @"\game\citadel\gameinfo.gi";
                Console.Clear();
                StarterInfo();
                try
                {
                    int value;

                    while (true)
                    {
                        Console.ForegroundColor = ConsoleColor.Magenta;
                        Console.Write(LangHelper.GetString("Enter_Fov_Number"));
                        string input = Console.ReadLine();

                        if (int.TryParse(input, out value))
                            break;

                        Console.Clear();
                        StarterInfo();

                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine(LangHelper.GetString("Incorrect_Input_TryAgain"));
                        Console.ResetColor();
                    }

                    var fovValue = 0.03708 * value - 1.207;
                    string text = await File.ReadAllTextAsync(gameinfoPath);

                    var fovContains = text.Contains("r_aspectratio");

                    if (fovContains)
                    {
                        text = Regex.Replace(text, "\\r\\n\\t\\t\"r_aspectratio\"\\s+\"[^\"]*\"", $"\r\n\t\t\"r_aspectratio\" \"{Math.Round(fovValue, 2)}\" ");
                    }
                    else
                    {
                        text = text.Replace("\tConVars\r\n\t{\t", $"\tConVars\r\n\t{{\t\r\n\t\t\"r_aspectratio\" \"{Math.Round(fovValue, 2)}\" ");
                    }
                    await File.WriteAllTextAsync(gameinfoPath, text);
                    Console.Clear();
                    StarterInfo();
                    await ProgressBar();
                    Console.Clear();
                    StarterInfo();
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine(" " + LangHelper.GetString("Successful") + "\n");
                    Console.ResetColor();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(LangHelper.GetString("Error") + $"{ex.Message}");
                }
                break;
            }
            else if (actionNum == "2")
            {
                Console.Clear();
                StarterInfo();
                LanguageChoose(language_actions, language_KEY_name, registry_key);
            }
        }
        

        Console.Write(LangHelper.GetString("Closing_In"));
        await Task.Delay(333);
        Console.Write("3");
        await Task.Delay(333);
        Console.Write(".");
        await Task.Delay(333);
        Console.Write(".");
        await Task.Delay(333);
        Console.Write(".");
        await Task.Delay(333);
        Console.Write("2");
        await Task.Delay(333);
        Console.Write(".");
        await Task.Delay(333);
        Console.Write(".");
        await Task.Delay(333);
        Console.Write(".");
        await Task.Delay(333);
        Console.Write("1");
        await Task.Delay(333);
        Console.Write(".");
        await Task.Delay(333);
        Console.Write(".");
        await Task.Delay(333);
        Console.Write(".");

        Environment.Exit(0);
    }

    private static void LanguageChoose(string[] language_actions, string language_KEY_name, RegistryKey registry_key)
    {
        Console.ForegroundColor = ConsoleColor.White;
        Console.Write("1. Русский \n" +
            "2. English \n" +
            "Выберите язык / Select language (1-2): ");
        Console.ResetColor();

        string languageNum = Console.ReadLine().ToString();

        while (!language_actions.Contains(languageNum))
        {
            Console.Clear();
            StarterInfo();
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(LangHelper.GetString("Incorrect_Input"));
            ActionChoose();
            languageNum = Console.ReadLine().ToString();
        }

        if (languageNum == "1") // ru
        {
            registry_key.SetValue(language_KEY_name, "ru");
        }
        else if (languageNum == "2") // en
        {
            registry_key.SetValue(language_KEY_name, "en");
        }

        LangHelper.ChangeLanguage(registry_key.GetValue(language_KEY_name)?.ToString() ?? "en");
    }

    static void StarterInfo()
    {
        Console.ForegroundColor = ConsoleColor.Blue;
        Console.WriteLine("▓█████▄ ▓█████ ▄▄▄      ▓█████▄  ██▓     ▒█████   ▄████▄   ██ ▄█▀\r\n▒██▀ ██▌▓█   ▀▒████▄    ▒██▀ ██▌▓██▒    ▒██▒  ██▒▒██▀ ▀█   ██▄█▒ \r\n░██   █▌▒███  ▒██  ▀█▄  ░██   █▌▒██░    ▒██░  ██▒▒▓█    ▄ ▓███▄░ \r\n░▓█▄   ▌▒▓█  ▄░██▄▄▄▄██ ░▓█▄   ▌▒██░    ▒██   ██░▒▓▓▄ ▄██▒▓██ █▄ \r\n░▒████▓ ░▒████▒▓█   ▓██▒░▒████▓ ░██████▒░ ████▓▒░▒ ▓███▀ ░▒██▒ █▄\r\n ▒▒▓  ▒ ░░ ▒░ ░▒▒   ▓▒█░ ▒▒▓  ▒ ░ ▒░▓  ░░ ▒░▒░▒░ ░ ░▒ ▒  ░▒ ▒▒ ▓▒\r\n ░ ▒  ▒  ░ ░  ░ ▒   ▒▒ ░ ░ ▒  ▒ ░ ░ ▒  ░  ░ ▒ ▒░   ░  ▒   ░ ░▒ ▒░\r\n ░ ░  ░    ░    ░   ▒    ░ ░  ░   ░ ░   ░ ░ ░ ▒  ░        ░ ░░ ░ \r\n   ░       ░  ░     ░  ░   ░        ░  ░    ░ ░  ░ ░      ░  ░   \r\n ░                       ░                       ░               ");
        Console.ResetColor();

        Console.ForegroundColor = ConsoleColor.Magenta;
        Console.WriteLine("  _____                    _             \r\n |_   _|_      _____  __ _| | _____ _ __ \r\n   | | \\ \\ /\\ / / _ \\/ _` | |/ / _ \\ '__|\r\n   | |  \\ V  V /  __/ (_| |   <  __/ |   \r\n   |_|   \\_/\\_/ \\___|\\__,_|_|\\_\\___|_|   \r\n                                         ");
        Console.WriteLine(" twitch.tv/megicullla \n");
        Console.ResetColor();
    }

    static void ActionChoose()
    {
        Console.ForegroundColor = ConsoleColor.Magenta;
        Console.Write("1. "+ LangHelper.GetString("Change_Fov") + "\n" +
            "2. Изменить язык / Language \n" +
            LangHelper.GetString("Enter_Number") + " (1-2): ");
        Console.ResetColor();
    }

    static async Task ProgressBar()
    {
        await AnsiConsole.Progress().StartAsync(async ctx =>
        {
            var gettingReadyTask = ctx.AddTask("[magenta] " + LangHelper.GetString("Applying_Changes") + "[/]");

            while (!ctx.IsFinished)
            {
                await Task.Delay(300);
                gettingReadyTask.Increment(10.5);
            }
        });
    }
}