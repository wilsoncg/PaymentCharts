using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

namespace Process.UnitTests
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            var days = Test.days;

            Assert.IsFalse(days.IsEmpty, "days empty");
            Assert.IsTrue(days.ToList().Count == 2, "days incorrect");

            var amounts = Test.amounts.ToList();

            Assert.IsTrue(amounts.Any(), "amounts empty");
            //Assert.IsTrue(amounts.Where(x => x. > 0, "types incorrect");
            //Assert.IsTrue(Test.s.Length > 0, "sum incorrect");
        }
    }
}
