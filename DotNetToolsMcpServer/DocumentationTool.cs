using ModelContextProtocol.Server;
using System.ComponentModel;

namespace DotNetToolsMcpServer
{
    [McpServerToolType]
    public class DocumentationTool
    {
        private static readonly HttpClient httpClient = new HttpClient();
        private static readonly LibraryConfigurationManager _configManager = new LibraryConfigurationManager(new DefaultConfigFilePathProvider());

        [McpServerTool, Description("Lists all available documentation files by fetching and parsing the documentation index.")]
        public static string ListAvailableDocumentationFiles()
        {
            try
            {
                List<LibraryInfo> libraries = _configManager.GetLibraries();
                
                if (libraries.Count == 0)
                {
                    return "No documentation files found.";
                }

                List<string> libraryNames = libraries.Select(lib => lib.Name).ToList();
                return $"Available documentation files:\n{string.Join("\n", libraryNames)}";
            }
            catch (Exception ex)
            {
                return $"Error fetching documentation index: {ex.Message}";
            }
        }

        [McpServerTool, Description("Gets documentation for a specific library by fetching the README file for the given NuGet package name.")]
        public static string GetDocumentationForLibrary(string libraryNugetPackageName)
        {
            try
            {
                string documentationUrl = _configManager.GetLibraryUrl(libraryNugetPackageName);
                
                if (string.IsNullOrEmpty(documentationUrl))
                {
                    return $"Library '{libraryNugetPackageName}' not found in documentation index.";
                }

                string documentationContent = httpClient.GetStringAsync(documentationUrl).Result;
                
                return $"Documentation for {libraryNugetPackageName}:\n\n{documentationContent}";
            }
            catch (Exception ex)
            {
                return $"Error fetching documentation for '{libraryNugetPackageName}': {ex.Message}";
            }
        }
    }
} 