namespace DotNetToolsMcpServer.Tests
{
    [TestClass]
    public class DocumentationToolTests
    {
        private static readonly string TestConfigDirectory = Path.Combine(Path.GetTempPath(), "DotNetToolsMcpServer_Test");
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
        }

        [TestCleanup]
        public void Cleanup()
        {
            // Clean up test files
            if (Directory.Exists(TestConfigDirectory))
            {
                Directory.Delete(TestConfigDirectory, true);
            }
        }

        [TestMethod]
        public void TestListAvailableDocumentationFiles_Empty()
        {
            string result = DocumentationTool.ListAvailableDocumentationFiles();
            Assert.IsTrue(result.Contains("No documentation files found"));
        }

        [TestMethod]
        public void TestGetDocumentationForLibrary_NotFound()
        {
            string result = DocumentationTool.GetDocumentationForLibrary("NonExistentLibrary");
            Assert.IsTrue(result.Contains("not found in documentation index"));
        }

        [TestMethod]
        public void TestLibraryConfigurationManager_AddAndList()
        {
            // Test adding a library
            _configManager!.AddLibrary("TestLibrary", "https://example.com/test.md");
            
            // Test listing libraries
            string listResult = _configManager.ListLibraries();
            Assert.IsTrue(listResult.Contains("TestLibrary"));
            Assert.IsTrue(listResult.Contains("Configured libraries:"));
        }

        [TestMethod]
        public void TestLibraryConfigurationManager_AddDuplicate()
        {
            _configManager!.AddLibrary("TestLibrary", "https://example.com/test.md");
            
            // Try to add the same library again
            Assert.ThrowsException<InvalidOperationException>(() =>
            {
                _configManager.AddLibrary("TestLibrary", "https://example.com/test2.md");
            });
        }

        [TestMethod]
        public void TestLibraryConfigurationManager_RemoveLibrary()
        {
            // Add a library
            _configManager!.AddLibrary("TestLibrary", "https://example.com/test.md");
            
            // Verify it exists
            string listResult = _configManager.ListLibraries();
            Assert.IsTrue(listResult.Contains("TestLibrary"));
            
            // Remove it
            _configManager.RemoveLibrary("TestLibrary");
            
            // Verify it's gone
            listResult = _configManager.ListLibraries();
            Assert.IsTrue(listResult.Contains("No libraries configured"));
        }

        [TestMethod]
        public void TestLibraryConfigurationManager_RemoveNonExistent()
        {
            Assert.ThrowsException<InvalidOperationException>(() =>
            {
                _configManager!.RemoveLibrary("NonExistentLibrary");
            });
        }

        [TestMethod]
        public void TestLibraryConfigurationManager_GetLibraryUrl()
        {
            _configManager!.AddLibrary("TestLibrary", "https://example.com/test.md");
            
            string url = _configManager.GetLibraryUrl("TestLibrary");
            Assert.AreEqual("https://example.com/test.md", url);
            
            string nonExistentUrl = _configManager.GetLibraryUrl("NonExistentLibrary");
            Assert.AreEqual(string.Empty, nonExistentUrl);
        }
    }
} 