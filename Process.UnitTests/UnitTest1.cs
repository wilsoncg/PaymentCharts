using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Process.UnitTests
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            Assert.IsTrue(Test.d.Length > 0, "days incorrect");
            //Assert.IsTrue(Test.t.Length > 0, "types incorrect");
            //Assert.IsTrue(Test.s.Length > 0, "sum incorrect");
        }
    }
}
