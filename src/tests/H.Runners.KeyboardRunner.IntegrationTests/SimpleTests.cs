using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace H.Runners.IntegrationTests
{
    [TestClass]
    public class SimpleTests
    {
        [TestMethod]
        public void Keyboard123Test()
        {
            using var runner = new KeyboardRunner();
            
            runner.Keyboard("123");
        }
    }
}
