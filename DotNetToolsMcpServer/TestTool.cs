using ModelContextProtocol.Server;
using System.ComponentModel;
using System.Diagnostics;

namespace DotNetToolsMcpServer
{
    [McpServerToolType]
    public class TestTool
    {
        [McpServerTool, Description("Runs all tests in a .NET test project using dotnet test with the specified absolute path to a test project .csproj file.")]
        public static string RunTests(string testProjectAbsolutePath)
        {
            try
            {
                ProcessStartInfo startInfo = new ProcessStartInfo
                {
                    FileName = "dotnet",
                    Arguments = $"test \"{testProjectAbsolutePath}\"",
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
                    return $"Tests passed for {testProjectAbsolutePath}\n{output}";
                }
                else
                {
                    return $"Tests failed for {testProjectAbsolutePath}\nError: {error}\nOutput: {output}";
                }
            }
            catch (Exception ex)
            {
                return $"Error running tests: {ex.Message}";
            }
        }

        [McpServerTool, Description("Runs specific tests in a .NET test project using dotnet test with a custom filter. Provide the absolute path to the test project and a filter expression (e.g., 'FullyQualifiedName~TestName' or 'Category=Integration').")]
        public static string RunSpecificTests(string testProjectAbsolutePath, string filterExpression)
        {
            try
            {
                ProcessStartInfo startInfo = new ProcessStartInfo
                {
                    FileName = "dotnet",
                    Arguments = $"test \"{testProjectAbsolutePath}\" --filter \"{filterExpression}\"",
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
                    return $"Specific tests passed for {testProjectAbsolutePath} with filter '{filterExpression}'\n{output}";
                }
                else
                {
                    return $"Specific tests failed for {testProjectAbsolutePath} with filter '{filterExpression}'\nError: {error}\nOutput: {output}";
                }
            }
            catch (Exception ex)
            {
                return $"Error running specific tests: {ex.Message}";
            }
        }
    }
}