using PizzaService.Web.Models;

namespace PizzaService.Tests
{
    [TestClass]
    public sealed class HashProviderTest
    {
        [TestMethod]
        public void TestHashVerified()
        {
            // test case 1
            string[] ids = ["1", "2", "3", "4", "5", "20", "22"];
            int counted = ids.Length;

            Submitted expected = new Submitted()
            {
                Qty = counted,
                Time = DateTime.Now,
                Hash = HashProvider.Calc(ids, counted).Hash
            };
            
            Submitted released = HashProvider.Calc(ids, counted);

            Assert.AreEqual<int?>(expected.Qty, released.Qty);
            if (string.Compare(expected.Time?.ToString("G"), released.Time?.ToString("G")) == 0)
            {
                Assert.AreEqual<string>(expected.Time?.ToString("G"), released.Time?.ToString("G"));
                Assert.AreEqual<string>(expected.Hash, released.Hash);
            }
            else
            {
                Assert.AreNotEqual<string>(expected.Time?.ToString("G"), released.Time?.ToString("G"));
                Assert.AreNotEqual<string>(expected.Hash, released.Hash);
            }
        }
    }
}
