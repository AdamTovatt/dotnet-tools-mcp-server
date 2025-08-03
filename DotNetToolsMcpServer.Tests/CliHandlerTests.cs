using System.Text;

namespace DotNetToolsMcpServer.Tests
{
    [TestClass]
    public class CliHandlerTests
    {
        private static readonly string TestConfigDirectory = Path.Combine(Path.GetTempPath(), "DotNetToolsMcpServer_CliTest");
        private StringWriter? _consoleOutput;
        private TextWriter? _originalConsoleOut;
        private LibraryConfigurationManager? _configManager;

        [TestInitialize]
        public void Setup()
        {
            // Clean up any existing test files
            if (Directory.Exists(TestConfigDirectory))
            {
                Directory.Delete(TestConfigDirectory, true);
            }
            
            // Create test config manager
            IConfigFilePathProvider testProvider = new TestConfigFilePathProvider(TestConfigDirectory);
            _configManager = new LibraryConfigurationManager(testProvider);
            
            // Capture console output for testing
            _originalConsoleOut = Console.Out;
            _consoleOutput = new StringWriter();
            Console.SetOut(_consoleOutput);
        }

        [TestCleanup]
        public void Cleanup()
        {
            // Clean up test files
            if (Directory.Exists(TestConfigDirectory))
            {
                Directory.Delete(TestConfigDirectory, true);
            }
            
            // Restore console output
            if (_originalConsoleOut != null)
            {
                Console.SetOut(_originalConsoleOut);
            }

            _consoleOutput?.Dispose();
        }

        [TestMethod]
        public void TestCliHandler_NoArguments()
        {
            int exitCode = CliHandler.HandleCli(new string[0], _configManager!);
            Assert.AreEqual(1, exitCode);
            
            string output = _consoleOutput!.ToString();
            Assert.IsTrue(output.Contains("Usage:"));
            Assert.IsTrue(output.Contains("add-library"));
            Assert.IsTrue(output.Contains("remove-library"));
            Assert.IsTrue(output.Contains("list-libraries"));
            Assert.IsTrue(output.Contains("config-path"));
            Assert.IsTrue(output.Contains("open-config"));
        }

        [TestMethod]
        public void TestCliHandler_UnknownCommand()
        {
            int exitCode = CliHandler.HandleCli(new[] { "unknown-command" }, _configManager!);
            Assert.AreEqual(1, exitCode);
            
            string output = _consoleOutput!.ToString();
            Assert.IsTrue(output.Contains("Unknown command: unknown-command"));
        }

        [TestMethod]
        public void TestCliHandler_AddLibrary_Success()
        {
            string[] args = { "add-library", "--name=TestLibrary", "--url=https://example.com/test.md" };
            int exitCode = CliHandler.HandleCli(args, _configManager!);
            
            Assert.AreEqual(0, exitCode);
            
            string output = _consoleOutput!.ToString();
            Assert.IsTrue(output.Contains("Successfully added library 'TestLibrary'"));
            
            // Verify the library was actually added
            string listResult = _configManager!.ListLibraries();
            Assert.IsTrue(listResult.Contains("TestLibrary"));
        }

        [TestMethod]
        public void TestCliHandler_AddLibrary_MissingName()
        {
            string[] args = { "add-library", "--url=https://example.com/test.md" };
            int exitCode = CliHandler.HandleCli(args, _configManager!);
            
            Assert.AreEqual(1, exitCode);
            
            string output = _consoleOutput!.ToString();
            Assert.IsTrue(output.Contains("Both --name and --url are required"));
        }

        [TestMethod]
        public void TestCliHandler_AddLibrary_MissingUrl()
        {
            string[] args = { "add-library", "--name=TestLibrary" };
            int exitCode = CliHandler.HandleCli(args, _configManager!);
            
            Assert.AreEqual(1, exitCode);
            
            string output = _consoleOutput!.ToString();
            Assert.IsTrue(output.Contains("Both --name and --url are required"));
        }

        [TestMethod]
        public void TestCliHandler_RemoveLibrary_Success()
        {
            // First add a library
            _configManager!.AddLibrary("TestLibrary", "https://example.com/test.md");
            
            string[] args = { "remove-library", "--name=TestLibrary" };
            int exitCode = CliHandler.HandleCli(args, _configManager);
            
            Assert.AreEqual(0, exitCode);
            
            string output = _consoleOutput!.ToString();
            Assert.IsTrue(output.Contains("Successfully removed library 'TestLibrary'"));
            
            // Verify the library was actually removed
            string listResult = _configManager.ListLibraries();
            Assert.IsTrue(listResult.Contains("No libraries configured"));
        }

        [TestMethod]
        public void TestCliHandler_RemoveLibrary_MissingName()
        {
            string[] args = { "remove-library" };
            int exitCode = CliHandler.HandleCli(args, _configManager!);
            
            Assert.AreEqual(1, exitCode);
            
            string output = _consoleOutput!.ToString();
            Assert.IsTrue(output.Contains("--name is required for remove-library command"));
        }

        [TestMethod]
        public void TestCliHandler_RemoveLibrary_NotFound()
        {
            string[] args = { "remove-library", "--name=NonExistentLibrary" };
            int exitCode = CliHandler.HandleCli(args, _configManager!);
            
            Assert.AreEqual(1, exitCode);
            
            string output = _consoleOutput!.ToString();
            Assert.IsTrue(output.Contains("Error removing library"));
        }

        [TestMethod]
        public void TestCliHandler_ListLibraries_Empty()
        {
            string[] args = { "list-libraries" };
            int exitCode = CliHandler.HandleCli(args, _configManager!);
            
            Assert.AreEqual(0, exitCode);
            
            string output = _consoleOutput!.ToString();
            Assert.IsTrue(output.Contains("No libraries configured"));
        }

        [TestMethod]
        public void TestCliHandler_ListLibraries_WithLibraries()
        {
            // Add some libraries
            _configManager!.AddLibrary("Library1", "https://example.com/lib1.md");
            _configManager.AddLibrary("Library2", "https://example.com/lib2.md");
            
            string[] args = { "list-libraries" };
            int exitCode = CliHandler.HandleCli(args, _configManager);
            
            Assert.AreEqual(0, exitCode);
            
            string output = _consoleOutput!.ToString();
            Assert.IsTrue(output.Contains("Configured libraries:"));
            Assert.IsTrue(output.Contains("Library1"));
            Assert.IsTrue(output.Contains("Library2"));
        }

        [TestMethod]
        public void TestCliHandler_ConfigPath_Success()
        {
            string[] args = { "config-path" };
            int exitCode = CliHandler.HandleCli(args, _configManager!);
            
            Assert.AreEqual(0, exitCode);
            
            string output = _consoleOutput!.ToString().Trim();
            string expectedPath = Path.Combine(TestConfigDirectory, "libraries.md");
            Assert.AreEqual(expectedPath, output);
        }

        [TestMethod]
        public void TestCliHandler_OpenConfig_Success()
        {
            string[] args = { "open-config" };
            int exitCode = CliHandler.HandleCli(args, _configManager!);
            
            Assert.AreEqual(0, exitCode);
            
            string output = _consoleOutput!.ToString();
            Assert.IsTrue(output.Contains("Opened configuration file:"));
            
            // Verify the config file was created
            string configPath = Path.Combine(TestConfigDirectory, "libraries.md");
            Assert.IsTrue(File.Exists(configPath));
            
            // Verify the file contains the expected header
            string fileContent = File.ReadAllText(configPath);
            Assert.IsTrue(fileContent.Contains("# Library Documentation Configuration"));
        }
    }
} 