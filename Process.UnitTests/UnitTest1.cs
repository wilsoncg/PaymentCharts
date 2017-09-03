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

            var trans = Test.transactions;
            Assert.AreEqual(1, trans.Count(x => x.Item1 == 17));
            Assert.AreEqual(2, trans.Count(x => x.Item1 == 16));

            var amounts = Test.amounts.ToList();

            Assert.IsTrue(amounts.Any(), "amounts empty");
            Assert.AreEqual("Deposit", amounts.ElementAt(0).Item1);
            var deps = amounts.ElementAt(0).Item2;
            Assert.AreEqual(1, deps.Count());
            Assert.AreEqual(17, deps.ElementAt(0).Item1);
            Assert.AreEqual(220m, amounts.ElementAt(0).Item2.First().Item3);

            Assert.AreEqual("Withdrawal", amounts.ElementAt(1).Item1);
            Assert.AreEqual(16, amounts.ElementAt(1).Item2.First().Item1);
            Assert.AreEqual(220m, amounts.ElementAt(1).Item2.First().Item3);
            //Assert.IsTrue(amounts.Where(x => x. > 0, "types incorrect");
            //Assert.IsTrue(Test.s.Length > 0, "sum incorrect");
        }
    }
}
