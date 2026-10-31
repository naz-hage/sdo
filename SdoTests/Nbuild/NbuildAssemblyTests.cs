using Microsoft.VisualStudio.TestTools.UnitTesting;
using Nbuild;

namespace NbuildTests
{
    [TestClass]
    public class NbuildAssemblyTests
    {
        [TestMethod]
        public void NbuildCodeIsIncludedInSdoAssembly()
        {
            Assert.AreEqual(typeof(Sdo.Program).Assembly, typeof(Command).Assembly);
        }
    }
}