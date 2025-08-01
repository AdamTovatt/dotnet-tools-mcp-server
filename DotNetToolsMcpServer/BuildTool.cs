using ModelContextProtocol.Server;
using System.ComponentModel;
using System.Diagnostics;

namespace DotNetToolsMcpServer
{
    [McpServerToolType]
    public class BuildTool
    {
        [McpServerTool, Description("Builds a .NET project using dotnet build with the specified absolute path to a .csproj file.")]
        public static string Build(string csprojAbsolutePath)
        {
            try
            {
                ProcessStartInfo startInfo = new ProcessStartInfo
                {
                    FileName = "dotnet",
                    Arguments = $"build \"{csprojAbsolutePath}\"",
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };

                using Process process = Process.Start(startInfo)!;
                string output = process.StandardOutput.ReadToEnd();
                string error = process.StandardError.ReadToEnd();
                process.WaitForExit();

                if (process.ExitCode == 0)
                {
                    return $"Build successful for {csprojAbsolutePath}\n{output}";
                }
                else
                {
                    return $"Build failed for {csprojAbsolutePath}\nError: {error}\nOutput: {output}";
                }
            }
            catch (Exception ex)
            {
                return $"Error building project: {ex.Message}";
            }
        }

        [McpServerTool, Description("Builds a .NET solution using dotnet build with the specified absolute path to a .sln file.")]
        public static string BuildSolution(string solutionAbsolutePath)
        {
            try
            {
                ProcessStartInfo startInfo = new ProcessStartInfo
                {
                    FileName = "dotnet",
                    Arguments = $"build \"{solutionAbsolutePath}\"",
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };

                using Process process = Process.Start(startInfo)!;
                string output = process.StandardOutput.ReadToEnd();
                string error = process.StandardError.ReadToEnd();
                process.WaitForExit();

                if (process.ExitCode == 0)
                {
                    return $"Solution build successful for {solutionAbsolutePath}\n{output}";
                }
                else
                {
                    return $"Solution build failed for {solutionAbsolutePath}\nError: {error}\nOutput: {output}";
                }
            }
            catch (Exception ex)
            {
                return $"Error building solution: {ex.Message}";
            }
        }
    }
}