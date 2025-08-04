using System.Diagnostics;

namespace DotNetToolsMcpServer
{
    public static class CliHandler
    {
        private static readonly LibraryConfigurationManager _configManager = new LibraryConfigurationManager(new DefaultConfigFilePathProvider());

        public static int HandleCli(string[] args)
        {
            return HandleCli(args, _configManager);
        }

        public static int HandleCli(string[] args, LibraryConfigurationManager configManager)
        {
            if (args.Length == 0)
            {
                PrintUsage();
                return 1;
            }

            string command = args[0].ToLowerInvariant();

            // Check for help flags
            if (command == "-help" || command == "--help" || command == "-h")
            {
                PrintUsage();
                return 0;
            }

            switch (command)
            {
                case "add-library":
                    return HandleAddLibrary(args, configManager);
                case "remove-library":
                    return HandleRemoveLibrary(args, configManager);
                case "list-libraries":
                    return HandleListLibraries(configManager);
                case "config-path":
                    return HandleConfigPath(configManager);
                case "open-config":
                    return HandleOpenConfig(configManager);
                default:
                    Console.WriteLine($"Unknown command: {command}");
                    Console.WriteLine("Use -help, --help, or -h for usage information.");
                    return 1;
            }
        }

        private static void PrintUsage()
        {
            Console.WriteLine("DotNetToolsMcpServer - MCP Server and CLI Tool for .NET Tools");
            Console.WriteLine();
            Console.WriteLine("Usage:");
            Console.WriteLine("  DotNetToolsMcpServer [command] [options]");
            Console.WriteLine();
            Console.WriteLine("Commands:");
            Console.WriteLine("  add-library --name=\"LibraryName\" --url=\"https://...\"");
            Console.WriteLine("    Add a library documentation URL to the configuration");
            Console.WriteLine();
            Console.WriteLine("  remove-library --name=\"LibraryName\"");
            Console.WriteLine("    Remove a library from the configuration");
            Console.WriteLine();
            Console.WriteLine("  list-libraries");
            Console.WriteLine("    List all configured libraries");
            Console.WriteLine();
            Console.WriteLine("  config-path");
            Console.WriteLine("    Display the path to the configuration file");
            Console.WriteLine();
            Console.WriteLine("  open-config");
            Console.WriteLine("    Open the configuration file in the default editor");
            Console.WriteLine();
            Console.WriteLine("Help:");
            Console.WriteLine("  -h, -help, --help");
            Console.WriteLine("    Show this help message");
            Console.WriteLine();
            Console.WriteLine("Examples:");
            Console.WriteLine("  DotNetToolsMcpServer add-library --name=\"EasyReasy\" --url=\"https://example.com/readme.md\"");
            Console.WriteLine("  DotNetToolsMcpServer list-libraries");
            Console.WriteLine("  DotNetToolsMcpServer config-path");
            Console.WriteLine();
            Console.WriteLine("Note: When run without arguments, the server operates as an MCP server for Cursor integration.");
        }

        private static int HandleAddLibrary(string[] args, LibraryConfigurationManager configManager)
        {
            string? name = null;
            string? url = null;

            for (int i = 1; i < args.Length; i++)
            {
                string arg = args[i];
                if (arg.StartsWith("--name="))
                {
                    name = arg.Substring("--name=".Length).Trim('"');
                }
                else if (arg.StartsWith("--url="))
                {
                    url = arg.Substring("--url=".Length).Trim('"');
                }
            }

            if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(url))
            {
                Console.WriteLine("Error: Both --name and --url are required for add-library command.");
                return 1;
            }

            try
            {
                configManager.AddLibrary(name, url);
                Console.WriteLine($"Successfully added library '{name}' with URL '{url}'");
                return 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error adding library: {ex.Message}");
                return 1;
            }
        }

        private static int HandleRemoveLibrary(string[] args, LibraryConfigurationManager configManager)
        {
            string? name = null;

            for (int i = 1; i < args.Length; i++)
            {
                string arg = args[i];
                if (arg.StartsWith("--name="))
                {
                    name = arg.Substring("--name=".Length).Trim('"');
                    break;
                }
            }

            if (string.IsNullOrEmpty(name))
            {
                Console.WriteLine("Error: --name is required for remove-library command.");
                return 1;
            }

            try
            {
                configManager.RemoveLibrary(name);
                Console.WriteLine($"Successfully removed library '{name}'");
                return 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error removing library: {ex.Message}");
                return 1;
            }
        }

        private static int HandleListLibraries(LibraryConfigurationManager configManager)
        {
            try
            {
                string result = configManager.ListLibraries();
                Console.WriteLine(result);
                return 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error listing libraries: {ex.Message}");
                return 1;
            }
        }

        private static int HandleConfigPath(LibraryConfigurationManager configManager)
        {
            try
            {
                string configPath = configManager.GetConfigFilePath();
                Console.WriteLine(configPath);
                return 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting config path: {ex.Message}");
                return 1;
            }
        }

        private static int HandleOpenConfig(LibraryConfigurationManager configManager)
        {
            try
            {
                string configPath = configManager.GetConfigFilePath();
                
                // Ensure the config directory exists
                configManager.EnsureConfigDirectoryExists();
                
                // Create the file if it doesn't exist
                if (!File.Exists(configPath))
                {
                    File.WriteAllText(configPath, "# Library Documentation Configuration\n\n");
                }
                
                // Open the file with the default application
                Process.Start(new ProcessStartInfo
                {
                    FileName = configPath,
                    UseShellExecute = true
                });
                
                Console.WriteLine($"Opened configuration file: {configPath}");
                return 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error opening config file: {ex.Message}");
                return 1;
            }
        }
    }
} 