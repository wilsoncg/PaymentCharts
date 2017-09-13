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

        [TestMethod]
        public void TestCurrencyMapping()
        {
            var rates = Test.precomputed.ToList();

            const int amount = 1;
            var audToUsd = Test.convert(amount, 1, 11);
            Assert.AreEqual(0.7899m, audToUsd);

            var usdToAud = decimal.Round(Test.convert(amount, 11, 1), 5);
            Assert.AreEqual(1.26598m, usdToAud);
        }
    }
}
