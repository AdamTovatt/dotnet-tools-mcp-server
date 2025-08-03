using System.Text.RegularExpressions;

namespace DotNetToolsMcpServer
{
    public class LibraryConfigurationManager
    {
        private readonly IConfigFilePathProvider _configFilePathProvider;

        public LibraryConfigurationManager(IConfigFilePathProvider configFilePathProvider)
        {
            _configFilePathProvider = configFilePathProvider;
        }

        private string ConfigFilePath => _configFilePathProvider.GetConfigFilePath();
        private string ConfigDirectory => Path.GetDirectoryName(ConfigFilePath) ?? string.Empty;

        public string GetConfigFilePath()
        {
            return _configFilePathProvider.GetConfigFilePath();
        }

        public void EnsureConfigDirectoryExists()
        {
            if (!Directory.Exists(ConfigDirectory))
            {
                Directory.CreateDirectory(ConfigDirectory);
            }
        }

        public List<LibraryInfo> GetLibraries()
        {
            List<LibraryInfo> libraries = new List<LibraryInfo>();
            
            if (!File.Exists(ConfigFilePath))
            {
                return libraries;
            }

            string[] lines = File.ReadAllLines(ConfigFilePath);
            string pattern = @"- \[([^\]]+)\]\(([^)]+)\)";
            
            foreach (string line in lines)
            {
                Match match = Regex.Match(line.Trim(), pattern);
                if (match.Success && match.Groups.Count > 2)
                {
                    string name = match.Groups[1].Value;
                    string url = match.Groups[2].Value;
                    libraries.Add(new LibraryInfo { Name = name, Url = url });
                }
            }

            return libraries;
        }

        public void AddLibrary(string name, string url)
        {
            EnsureConfigDirectoryExists();
            
            List<LibraryInfo> existingLibraries = GetLibraries();
            
            // Check if library already exists
            if (existingLibraries.Any(lib => lib.Name.Equals(name, StringComparison.OrdinalIgnoreCase)))
            {
                throw new InvalidOperationException($"Library '{name}' already exists in configuration.");
            }

            string newEntry = $"- [{name}]({url})";
            File.AppendAllText(ConfigFilePath, newEntry + Environment.NewLine);
        }

        public void RemoveLibrary(string name)
        {
            if (!File.Exists(ConfigFilePath))
            {
                throw new InvalidOperationException($"Library '{name}' not found in configuration.");
            }

            string[] lines = File.ReadAllLines(ConfigFilePath);
            List<string> updatedLines = new List<string>();
            bool found = false;
            string pattern = $@"- \[{Regex.Escape(name)}\]\([^)]+\)";

            foreach (string line in lines)
            {
                if (!Regex.IsMatch(line.Trim(), pattern))
                {
                    updatedLines.Add(line);
                }
                else
                {
                    found = true;
                }
            }

            if (!found)
            {
                throw new InvalidOperationException($"Library '{name}' not found in configuration.");
            }

            File.WriteAllLines(ConfigFilePath, updatedLines);
        }

        public string GetLibraryUrl(string name)
        {
            List<LibraryInfo> libraries = GetLibraries();
            LibraryInfo? library = libraries.FirstOrDefault(lib => 
                lib.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
            
            return library?.Url ?? string.Empty;
        }

        public string ListLibraries()
        {
            List<LibraryInfo> libraries = GetLibraries();
            
            if (libraries.Count == 0)
            {
                return "No libraries configured.";
            }

            return $"Configured libraries:\n{string.Join("\n", libraries.Select(lib => $"- {lib.Name}"))}";
        }
    }

    public class LibraryInfo
    {
        public string Name { get; set; } = string.Empty;
        public string Url { get; set; } = string.Empty;
    }
} 