using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

namespace Process.UnitTests
{
    [TestClass]
    public class ProcessTests
    {
        [TestMethod]
        public void TestStacksAreCorrect()
        {
            var trans = Test.transactions;
            Assert.AreEqual(1, trans.Count(x => x.Item1 == 02));
            Assert.AreEqual(2, trans.Count(x => x.Item1 == 01));

            var stacks = Test.stacks.ToList();

            Assert.IsTrue(stacks.Any(), "stacks empty");
            Assert.IsTrue(stacks.Count == 2);

            var deposits = stacks.Where(x => x.Name == "Deposit");
            var withdrawals = stacks.Where(x => x.Name == "Withdrawal");
            
            Assert.AreEqual(1, deposits.Count());
            Assert.AreEqual(2655 + 220, deposits.Sum(x => x.Amounts.Sum()));

            Assert.AreEqual(1, withdrawals.Count());
            Assert.AreEqual(110, withdrawals.Sum(x => x.Amounts.Sum()));
        }
    }
}
