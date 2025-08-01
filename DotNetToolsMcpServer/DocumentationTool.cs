using ModelContextProtocol.Server;
using System.ComponentModel;
using System.Text.RegularExpressions;

namespace DotNetToolsMcpServer
{
    [McpServerToolType]
    public class DocumentationTool
    {
        private static readonly HttpClient httpClient = new HttpClient();
        private static readonly string documentationIndexUrl = "https://raw.githubusercontent.com/AdamTovatt/library-documentation-files/refs/heads/main/README.md";

        [McpServerTool, Description("Lists all available documentation files by fetching and parsing the documentation index.")]
        public static string ListAvailableDocumentationFiles()
        {
            try
            {
                string indexContent = httpClient.GetStringAsync(documentationIndexUrl).Result;
                
                // Parse markdown links to extract library names
                List<string> libraryNames = new List<string>();
                string pattern = @"- \[([^\]]+)\]\([^)]+\)";
                
                MatchCollection matches = Regex.Matches(indexContent, pattern);
                foreach (Match match in matches)
                {
                    if (match.Groups.Count > 1)
                    {
                        string libraryName = match.Groups[1].Value;
                        libraryNames.Add(libraryName);
                    }
                }

                if (libraryNames.Count == 0)
                {
                    return "No documentation files found.";
                }

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
                // First get the index to find the URL for the specific library
                string indexContent = httpClient.GetStringAsync(documentationIndexUrl).Result;
                
                // Find the URL for the specified library
                string pattern = $@"- \[{Regex.Escape(libraryNugetPackageName)}\]\(([^)]+)\)";
                Match match = Regex.Match(indexContent, pattern);
                
                if (!match.Success)
                {
                    return $"Library '{libraryNugetPackageName}' not found in documentation index.";
                }

                string documentationUrl = match.Groups[1].Value;
                
                // Fetch the documentation content
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