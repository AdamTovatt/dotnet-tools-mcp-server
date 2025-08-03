namespace DotNetToolsMcpServer.Tests
{
    public class TestConfigFilePathProvider : IConfigFilePathProvider
    {
        private readonly string _testConfigDirectory;

        public TestConfigFilePathProvider(string testConfigDirectory)
        {
            _testConfigDirectory = testConfigDirectory;
        }

        public string GetConfigFilePath()
        {
            return Path.Combine(_testConfigDirectory, "libraries.md");
        }
    }
} 