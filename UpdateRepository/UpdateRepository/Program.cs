using System;
using System.Threading.Tasks;
using Spectre.Console;
using System.IO;
using System.Diagnostics;

namespace UpdateRepository
{
    class Program
    {
        static async Task Main()
        {
            Console.Title = "Repository Updater";
            string baseDirectory = AppDomain.CurrentDomain.BaseDirectory; // Path where the App runs = Path where the repository is 
            AnsiConsole.MarkupLine("Repository Updater");
            AnsiConsole.MarkupLine($"Checking if [yellow]'{baseDirectory}'[/] is a Git repository");
            if (!IsGitRepository(baseDirectory))
            {
                AnsiConsole.MarkupLine("[red]This isn't a git repository[/]\n\n");
                AnsiConsole.MarkupLine("Press any key to exit...");
                Console.ReadKey();
                return;
            }
            AnsiConsole.MarkupLine("[green]Success[/]");
            if (!AnsiConsole.Confirm("Do you want to update this repository?")) 
            {
                AnsiConsole.MarkupLine("\n\nPress any key to exit...");
                Console.ReadKey();
                return;
            }
            var spinner = Spinner.Known.Aesthetic;
            var progress = new SpinnerColumn(spinner);
            await AnsiConsole.Progress().Columns(new ProgressColumn[]
            {
                new TaskDescriptionColumn(),
                progress,
            }).StartAsync(async ctx =>
            {
                var task = ctx.AddTask("Downloading.");
                await UpdateRepositoryAsync(baseDirectory);
                await Task.Delay(5000);
                task.Increment(1.0);
            });
            AnsiConsole.MarkupLine("[green]Update successfully completed![/]\n\n");
            AnsiConsole.MarkupLine("Press any key to exit...");
            Console.ReadKey();
        }
        static bool IsGitRepository(string dir)
        {
            string gitDirectory = Path.Combine(dir, ".git");
            return Directory.Exists(gitDirectory);
        }

        static bool UpdateRepository(string baseDir)
        {
            try
            {
                Directory.SetCurrentDirectory(baseDir);
                ExecuteCommand("git", "pull");

                return true;
            }
            catch (Exception e)
            {
                AnsiConsole.WriteException(e);
                return false;
            }
        }
        static async Task UpdateRepositoryAsync(string path)
        {
            try
            {
                //AnsiConsole.Write("Updating");
                await Task.Delay(5000);
                Directory.SetCurrentDirectory(path);
                await ExecuteCommand("git", "pull");
            }
            catch (Exception e)
            {
                AnsiConsole.WriteException(e);
            }
        }
        static Task ExecuteCommand(string command, string arguments)
        {
            return Task.Run(() =>
            {
                using (Process process = new Process())
                {
                    process.StartInfo.FileName = command;
                    process.StartInfo.Arguments = arguments;
                    process.StartInfo.RedirectStandardOutput = true;
                    process.StartInfo.UseShellExecute = false;
                    process.StartInfo.CreateNoWindow = true;

                    process.Start();
                    process.WaitForExit();
                }
            });
        }
    }
}
