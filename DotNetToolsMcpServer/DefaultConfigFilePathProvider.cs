namespace DotNetToolsMcpServer
{
    public class DefaultConfigFilePathProvider : IConfigFilePathProvider
    {
        public string GetConfigFilePath()
        {
            string configDirectory = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "DotNetToolsMcpServer");
            return Path.Combine(configDirectory, "libraries.md");
        }
    }
} 