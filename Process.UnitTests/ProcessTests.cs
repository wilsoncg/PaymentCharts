using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using FSharp.Data.Sql.Runtime;

namespace Process.UnitTests
{
    [TestClass]
    public class ProcessTests
    {
        [TestMethod]
        public void TestStacksAreCorrect()
        {
            var stacks = Transactions.getDaysStacks(120, ChartSettings.PaymentsDb.GetDataContext()).ToList();

            Assert.IsTrue(stacks.Any(), "stacks empty");
            Assert.AreEqual(2, stacks.Count);

            var deposits = stacks.Where(x => x.Name == "Card Deposit");
            var withdrawals = stacks.Where(x => x.Name == "Card Withdrawal");

            Assert.AreEqual(1, deposits.Count());
            Assert.AreEqual(2655 + 220, deposits.Sum(x => x.Amounts.Sum()));

            Assert.AreEqual(1, withdrawals.Count());
            Assert.AreEqual(110, withdrawals.Sum(x => x.Amounts.Sum()));
        }

        //[TestMethod]
        //public void TestCurrencyMapping()
        //{
        //    var rates = Transactions.precomputed.ToList();

        //    const int amount = 1;
        //    var audToUsd = Transactions.convert(amount, 1, 11);
        //    Assert.AreEqual(0.7899m, audToUsd);

        //    var usdToAud = decimal.Round(Transactions.convert(amount, 11, 1), 5);
        //    Assert.AreEqual(1.26598m, usdToAud);

        //    var audTo24 = decimal.Round(Transactions.convert(amount, 1, 24), 5);
        //    Assert.AreEqual(2.82689m, audTo24);
        //}
    }
}
