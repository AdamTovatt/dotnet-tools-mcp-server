namespace DotNetToolsMcpServer.Tests
{
    [TestClass]
    public class DocumentationToolTests
    {
        [TestMethod]
        public void TestListAvailableDocumentationFiles()
        {
            string result = DocumentationTool.ListAvailableDocumentationFiles();
            Console.WriteLine(result);
        }

        [TestMethod]
        public void TestGetDocumentationForLibrary()
        {
            string result = DocumentationTool.GetDocumentationForLibrary("EasyReasy.Auth");
            Console.WriteLine(result);
        }
    }
} 