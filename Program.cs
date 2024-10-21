using CommandLine;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace InitProject
{
    /// <summary>
    /// Main program class for initializing project structure.
    /// </summary>
    internal class Program
    {
        /// <summary>
        /// Entry point of the application.
        /// </summary>
        /// <param name="args">Command line arguments.</param>
        /// <returns>An integer indicating the status of the operation.</returns>
        static int Main(string[] args)
        {
            return CommandLine.Parser.Default.ParseArguments<Options>(args)
                .MapResult((opts) => RunOptions(opts), (errs) => HandleParseError(errs));
        }

        /// <summary>
        /// Executes the main functionality based on parsed options.
        /// </summary>
        /// <param name="opts">Parsed command line options.</param>
        /// <returns>An integer indicating the status of the operation.</returns>
        static int RunOptions(Options opts)
        {
            // Check if the specified directory exists
            if (!Directory.Exists(opts.Directory))
            {
                Console.Error.WriteLine("Cannot access the specified directory.");
                return -1;
            }

            // Check if Git is initialized in the specified directory
            if (!Directory.Exists(Path.Combine(opts.Directory, ".git")))
            {
                Console.Error.WriteLine("Git is not set up in the specified directory.");
                return -1;
            }

            // Verify if the Git command is available
            if (!IsGitAvailable())
            {
                Console.Error.WriteLine("Git command is not working.");
                return -1;
            }

            // Display the operations that will be performed
            Console.WriteLine("The following operations will be performed:");
            Console.WriteLine();
            Console.WriteLine("  Target directory          : " + opts.Directory);
            Console.WriteLine("  Template name             : " + opts.TemplateName);
            Console.WriteLine("  Name after replacement    : " + opts.Name);
            Console.WriteLine("  Target extensions         : " + string.Join(" ", opts.Extensions));
            Console.WriteLine();
            Console.Write("Do you want to continue? (y/n): ");

            var input = Console.ReadLine();
            if (input.Trim().ToLower() != "y")
            {
                Console.WriteLine("The operation has been canceled.");
                return 0;
            }

            try
            {
                // Rename directories that contain the template name
                foreach (string dir in Directory.GetDirectories(opts.Directory, "*", SearchOption.AllDirectories))
                {
                    string folderName = Path.GetFileName(dir);
                    if (folderName.Contains(opts.TemplateName))
                    {
                        string newDirPath = Path.Combine(Path.GetDirectoryName(dir), folderName.Replace(opts.TemplateName, opts.Name));
                        GitMove(opts.Directory, dir, newDirPath);
                        Console.WriteLine($"Renamed folder: {dir} to {newDirPath}");
                    }
                }

                // Rename files and replace text within the files
                foreach (string dir in Directory.GetDirectories(opts.Directory, "*", SearchOption.AllDirectories))
                {
                    foreach (string file in opts.Extensions.SelectMany(ext => Directory.GetFiles(dir, ext, SearchOption.TopDirectoryOnly)))
                    {
                        string newFilePath = file;
                        if (file.Contains(opts.TemplateName))
                        {
                            newFilePath = Path.Combine(Path.GetDirectoryName(dir), file.Replace(opts.TemplateName, opts.Name));
                            GitMove(opts.Directory, file, newFilePath);
                            Console.WriteLine($"Renamed file: {file} to {newFilePath}");
                        }

                        // Read the file content and replace the template name with the new name
                        string content = File.ReadAllText(newFilePath);

                        if (content.Contains(opts.TemplateName))
                        {
                            string newContent = content.Replace(opts.TemplateName, opts.Name);
                            File.WriteAllText(newFilePath, newContent);
                            Console.WriteLine($"Replaced text in file: {newFilePath}");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine("An unexpected error has occurred.");
                Console.Error.WriteLine(ex.ToString());
                return -1;
            }

            return 0;
        }

        /// <summary>
        /// Handles errors from command line parsing.
        /// </summary>
        /// <param name="errs">A collection of parsing errors.</param>
        /// <returns>An integer indicating the status of the operation.</returns>
        static int HandleParseError(IEnumerable<Error> errs)
        {
            Console.Error.WriteLine("An error occurred. Please check the parameters.");
            return -1;
        }

        /// <summary>
        /// Checks if the Git command is available.
        /// </summary>
        /// <returns>A boolean indicating whether Git is available.</returns>
        static bool IsGitAvailable()
        {
            string command = "git";
            string args = "--version";

            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                FileName = command,
                Arguments = args,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using (Process process = new Process { StartInfo = startInfo })
            {
                try
                {
                    process.Start();
                    process.WaitForExit();
                    return process.ExitCode == 0;
                }
                catch (Exception)
                {
                    return false;
                }
            }
        }

        /// <summary>
        /// Moves files or directories using Git.
        /// </summary>
        /// <param name="workingDirectory">The working directory where the command will be executed.</param>
        /// <param name="oldName">The current name of the file or directory.</param>
        /// <param name="newName">The new name for the file or directory.</param>
        static void GitMove(string workingDirectory, string oldName, string newName)
        {
            string command = "git";
            string args = $"mv \"{oldName}\" \"{newName}\"";

            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                WorkingDirectory = workingDirectory,
                FileName = command,
                Arguments = args,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using (Process process = new Process { StartInfo = startInfo })
            {
                process.Start();
                process.WaitForExit();
            }
        }
    }

    /// <summary>
    /// Defines command line options for the application.
    /// </summary>
    internal class Options
    {
        /// <summary>
        /// Gets or sets the target directory.
        /// </summary>
        [Option('d', "dir", Required = true, HelpText = "Specify the target directory.")]
        public string Directory { get; set; }

        /// <summary>
        /// Gets or sets the name after replacement.
        /// </summary>
        [Option('n', "name", Required = true, HelpText = "Specify the name after replacement.")]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the template name to replace.
        /// </summary>
        [Option('t', "template", Required = false, HelpText = "Specify the template name to replace.", Default = "__template__")]
        public string TemplateName { get; set; }

        /// <summary>
        /// Gets or sets the file extensions to be processed.
        /// </summary>
        [Option('e', "ext", Required = false, HelpText = "Specify the file extension.", Default = new string[] { "*.cs", "*.csproj", "*.vb", "*.vbproj", "*.config", "*.json", "*.aspx", "*.css", "*.js", "*.html", "*.sln" })]
        public IEnumerable<string> Extensions { get; set; }
    }
}
