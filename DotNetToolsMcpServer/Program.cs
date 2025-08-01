using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace DotNetToolsMcpServer
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);
            builder.Logging.AddConsole(consoleLogOptions =>
            {
                // Configure all logs to go to stderr
                consoleLogOptions.LogToStandardErrorThreshold = LogLevel.Trace;
            });

            if (builder.Services == null)
                throw new NullReferenceException(nameof(builder.Services));

            builder.Services
                .AddMcpServer()
                .WithStdioServerTransport()
                .WithTools<BuildTool>()
                .WithTools<TestTool>()
                .WithTools<DocumentationTool>();

            await builder.Build().RunAsync();
        }
    }
}
