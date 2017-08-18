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
            Microsoft.FSharp.Collections.FSharpList<int> days = Test.days;
            Assert.IsFalse(days.IsEmpty, "days empty");
            Assert.IsTrue(days.Length == 2, "days incorrect");
            //Assert.IsTrue(Test.t.Length > 0, "types incorrect");
            //Assert.IsTrue(Test.s.Length > 0, "sum incorrect");
        }
    }
}
