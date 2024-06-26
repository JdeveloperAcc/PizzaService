using Microsoft.EntityFrameworkCore;
using PizzaService.Web.Models;

namespace PizzaService.Tests
{
    [TestClass]
    public sealed class MemoryStoreTest
    {
        public const string SchemaName = "TestedOrderList";

        private DbContext? Context = null;

        public class OrderSameParameters : EqualityComparer<Order>
        {
            public override bool Equals(Order? od1, Order? od2)
            {
                if (od1 == null && od2 == null)
                    return true;
                else if (od1 == null || od2 == null)
                    return false;

                return (
                        // Comment: "We run test cases in random order."
                        // od1.Id == od2.Id &&
                        string.Equals(od1.ProductId, od2.ProductId) &&
                        od1.Quantity == od2.Quantity);
            }

            public override int GetHashCode(Order ox)
            {

                long hCode =
                    // Comment: "We run test cases in random order."
                    //(long)ox.Id ^                    
                    ox.ProductId.GetHashCode() ^ (long)ox.Quantity;
                return hCode.GetHashCode();
            }
        }

        public MemoryStoreTest()
        {
            MemoryStoreInit(SchemaName);
        }

        public void MemoryStoreInit(string schemaName)
        {
            var contextOptions = new DbContextOptionsBuilder<TodoDb>()
                .UseInMemoryDatabase(schemaName)
                .Options;
            this.Context = new TodoDb(contextOptions);
        }

        public DbContext ProvideContext()
        {
            return this.Context ?? throw new NullReferenceException();
        }

        [TestMethod]
        public async Task TestSimple()
        {
            var storeIO = new Adapter<DbContext>(ProvideContext()).GetStorageIO<TodoDb>();

            // test case 1
            int expected1 = 0;
            var listed1 = await storeIO.Todos.ToListAsync<Order>();

            Assert.AreEqual<int>(expected1, listed1.Count);

            // test case 2
            Order od = new Order()
            {
                ProductId = "case97972",
                Quantity = 2465
            };

            int expectedCnt2 = 1;

            storeIO.Todos.Add(od);  // od is added origin
            int insertedCnt2 = await storeIO.SaveChangesAsync();

            Assert.AreEqual<int>(expectedCnt2, insertedCnt2);

            // Comment: "We run test cases in random order."
            //uint expectedId2 = 1; 
            //Assert.AreEqual<uint>(expectedId2, od.Id);

            // test case 3
            Order expected3 = od;
            Order? found3 = await storeIO.FindAsync<Order>(od.Id);

            // Comment: "We run test cases in random order."
            //Assert.AreEqual<Order?>(expected3, found3);
            Assert.AreEqual<string?>(expected3.ProductId, found3?.ProductId);
            Assert.AreEqual<uint?>(expected3.Quantity, found3?.Quantity);

            // test case 4
            Order expected4 = od;
            Order? found4 = await storeIO.Todos
                .Where(p => p.ProductId == expected4.ProductId)
                .FirstOrDefaultAsync<Order>();

            // Comment: "We run test cases in random order."
            //Assert.AreEqual<Order?>(expected4, found4); // od is compared to found
            Assert.AreEqual<string?>(expected4.ProductId, found4?.ProductId);
            Assert.AreEqual<uint?>(expected4.Quantity, found4?.Quantity);


            // test case 5
            Order nextOd = new Order()
            {
                ProductId = "case46541",
                Quantity = 2975
            };

            int expectedCnt5 = 1;
            storeIO.Todos.Add(nextOd);
            int insertedCnt5 = await storeIO.SaveChangesAsync();

            Assert.AreEqual<int>(expectedCnt5, insertedCnt5);
            // Comment: "We run test cases in random order."
            //uint expectedId5 = 2;
            //Assert.AreEqual<uint>(expectedId5, nextOd.Id);

            // test case 6
            Order expected6 = nextOd; // nextOd is added origin too
            Order? found6 = await storeIO.FindAsync<Order>(nextOd.Id);

            Assert.AreEqual<Order?>(expected6, found6);

            // test case 7
            Order expected7 = nextOd;
            Order? found7 = await storeIO.Todos
                .Where(p => p.ProductId == expected7.ProductId)
                .FirstOrDefaultAsync<Order>();

            // Comment: "We run test cases in random order."
            //Assert.AreEqual<Order?>(expected7, found7); // origin matches with found too
            Assert.AreEqual<string?>(expected7.ProductId, found7?.ProductId);
            Assert.AreEqual<uint?>(expected7.Quantity, found7?.Quantity);

            // test case 8
            List<Order> batch = new List<Order>()
            {
                new Order()
                {
                    ProductId = "case97972",
                    Quantity = 24
                },
                new Order()
                {
                    ProductId = "case97972",
                    Quantity = 24
                },
                new Order()
                {
                    ProductId = "case46541",
                    Quantity = 5
                },
                new Order()
                {
                    ProductId = "case46541",
                    Quantity = 5
                }
            };

            int expectedCnt8 = batch.Count;
            foreach(Order item in batch)
            {
                storeIO.Todos.Add(item);
            }

            int submittedCnt8 = await storeIO.SaveChangesAsync();

            Assert.AreEqual<int>(expectedCnt8, submittedCnt8);

            // test case 9
            List<Order> expected9 = new List<Order>()
            {
                new Order()
                {
                    Id = 1,
                    ProductId = "case97972",
                    Quantity = 2465
                },
                new Order()
                {
                    Id = 2,
                    ProductId = "case46541",
                    Quantity = 2975
                },
                new Order()
                {
                    Id = 3,
                    ProductId = "case97972",
                    Quantity = 24
                },
                new Order()
                {
                    Id = 4,
                    ProductId = "case97972",
                    Quantity = 24
                },
                new Order()
                {
                    Id = 5,
                    ProductId = "case46541",
                    Quantity = 5
                },
                new Order()
                {
                    Id = 6,
                    ProductId = "case46541",
                    Quantity = 5
                }
            };
            List<Order> listed9 = await storeIO.Todos.ToListAsync<Order>();

            Assert.AreEqual<int>(expected9.Count, listed9.Count);
            for (int i = 0; i < expected9.Count; i++)
            {
                // Expected are not origin for given!
                Order needed = expected9[i];
                Order given = listed9[i];
                // Comment: "We run test cases in random order."
                //Assert.AreEqual<uint>(needed.Id, given.Id);
                Assert.AreEqual<string>(needed.ProductId, given.ProductId);
                Assert.AreEqual<uint>(needed.Quantity, given.Quantity);
            }

            // test case 10
            int expected10 = 0;
            storeIO.Todos.RemoveRange(listed9);
            await storeIO.SaveChangesAsync();
            List<Order> listed10 = await storeIO.Todos.ToListAsync<Order>();

            Assert.AreEqual<int>(expected10, listed10.Count);
        }

        [TestMethod]
        public async Task TestRegular()
        {
            // Adapter Operations:
            Agregator processingQueue = new Agregator(ProvideContext());

            // test case 1
            int exptected1 = 1;
            Order[] arrOrders1 =
            [
                new Order()
                {
                    ProductId = "case97972",
                    Quantity = 24
                },
                new Order()
                {
                    ProductId = "case97972",
                    Quantity = 24
                }
            ];

            int submittedQty1 = await processingQueue.SingletonManipulator.AddDataIncrementAsync(arrOrders1);
            Assert.AreEqual<int>(exptected1, submittedQty1);

            // test case 2
            Order expected2 = new Order()
            {
                Id = 1,
                ProductId = "case97972",
                Quantity = 48
            };
            Order found2 = await processingQueue.SingletonManipulator.GetDataItemAsync(1);
            // Comment: "We run test cases in random order."
            //Assert.AreEqual<uint>(expected2.Id, found2.Id);
            //Assert.AreEqual<string>(expected2.ProductId, found2.ProductId);
            //Assert.AreEqual<uint>(expected2.Quantity, found2.Quantity);

            // test case 3
            Order expected3 = expected2;
            Order filtered3 = await processingQueue.SingletonManipulator.GetDataNamedAsync(expected3.ProductId);
            // Comment: "We run test cases in random order."
            //Assert.AreEqual<uint>(expected3.Id, filtered3.Id);
            Assert.AreEqual<string>(expected3.ProductId, filtered3.ProductId);
            Assert.AreEqual<uint>(expected3.Quantity, filtered3.Quantity);

            // test case 4
            int expected4 = 5;
            Order[] arrOrders4 =
            [
                new Order()
                {
                    ProductId = "case12578",
                    Quantity = 1
                },
                new Order()
                {
                    ProductId = "case12345",
                    Quantity = 2
                },
                new Order()
                {
                    ProductId = "case97972",
                    Quantity = 24
                },
                new Order()
                {
                    ProductId = "case46541",
                    Quantity = 5
                },
                new Order()
                {
                    ProductId = "case77125",
                    Quantity = 2
                },
            ];
            int submittedQty4 = await processingQueue.SingletonManipulator.AddDataIncrementAsync(arrOrders4);
            Assert.AreEqual<int>(expected4, submittedQty4);

            // test case 5
            Order[] expected5 =
            {
                new Order()
                {
                    Id = 1,
                    ProductId = "case97972",
                    Quantity = 72
                },
                new Order()
                {
                    Id = 2,
                    ProductId = "case12578",
                    Quantity = 1
                },
                new Order()
                {
                    Id = 3,
                    ProductId = "case12345",
                    Quantity = 2
                },
                new Order()
                {
                    Id = 4,
                    ProductId = "case46541",
                    Quantity = 5
                },
                new Order()
                {
                    Id = 5,
                    ProductId = "case77125",
                    Quantity = 2
                }
            };
            IEnumerable<Order> listed5 = await processingQueue.SingletonManipulator.GetAggregatedResultsAsync();

            Assert.AreEqual<int>(expected5.Count(), listed5.Count());

            // learn.microsoft.com "Enumerable.Except Method"
            // Get all the elements from the first array
            // except for the elements from the second array.
            Order[] array5 = listed5.ToArray<Order>();
            OrderSameParameters comparer = new OrderSameParameters();
            IEnumerable<Order> diffs5 = array5.Except<Order>(expected5, comparer);
            Assert.IsTrue(diffs5.Count() == 0);

            // test case 6
            int expected6 = 0;
            IEnumerable<Order> deducted6 = expected5;
            await processingQueue.SingletonManipulator.ConfirmDeductedAsync(deducted6);
            IEnumerable<Order> listed6 = await processingQueue.SingletonManipulator.GetAggregatedResultsAsync();

            Assert.AreEqual<int>(expected6, listed6.Count());

            // test case 7
            Order[] arrOrders7 =
            [
                new Order()
                {
                    ProductId = "case12578",
                    Quantity = 1
                },
                new Order()
                {
                    ProductId = "case12345",
                    Quantity = 2
                },
                new Order()
                {
                    ProductId = "case97972",
                    Quantity = 24
                },
                new Order()
                {
                    ProductId = "case46541",
                    Quantity = 5
                },
                new Order()
                {
                    ProductId = "case77125",
                    Quantity = 2
                },
            ];
            int expected7 = 5;
            Order[] expectedArr7 =
            [
                new Order()
                {
                    Id = 6,
                    ProductId = "case12578",
                    Quantity = 1
                },
                new Order()
                {
                    Id = 7,
                    ProductId = "case12345",
                    Quantity = 2
                },
                new Order()
                {
                    Id = 8,
                    ProductId = "case97972",
                    Quantity = 24
                },
                new Order()
                {
                    Id = 9,
                    ProductId = "case46541",
                    Quantity = 5
                },
                new Order()
                {
                    Id = 10,
                    ProductId = "case77125",
                    Quantity = 2
                },
            ];


            int confirmedCnt7 = await processingQueue.SingletonManipulator.AddDataIncrementAsync(arrOrders7);

            Assert.AreEqual<int>(expected7, confirmedCnt7);

            var listed7 = await processingQueue.SingletonManipulator.GetAggregatedResultsAsync();
            Assert.AreEqual<int>(expectedArr7.Length, listed7.Count());

            IEnumerable<Order> diffs7 = listed7.Except<Order>(expectedArr7, comparer);
            Assert.IsTrue(diffs7.Count() == 0);

            // test case 8
            int expected8 = 0;

            await processingQueue.SingletonManipulator.ConfirmDeductedAsync(listed7);
            var listed8 = await processingQueue.SingletonManipulator.GetAggregatedResultsAsync();
            Assert.AreEqual<int>(expected8, listed8.Count());
        }

        [TestMethod]
        public void TestTwoConcurrentThreadOnOneContextFault()
        {
            // test case 1
            TodoDb storeIO = new Adapter<DbContext>(ProvideContext()).GetStorageIO<TodoDb>();
            try
            {
                Thread thread1 = new Thread(() => QueryDataFromMemory(storeIO, 100));
                Thread thread2 = new Thread(() => QueryDataFromMemory(storeIO, 55));

                thread1.Start();
                thread2.Start();

                thread2.Join();
                thread1.Join();
            }
            catch
            {
                Assert.Fail("SystemException!");
            }
        }

        public void QueryDataFromMemory(TodoDb db, int timeMs)
        {
            try
            {
                var listed1 = db.Todos.ToList<Order>();
                Thread.Sleep(timeMs);
                var listed2 = db.Todos.ToList<Order>();
            }
            catch (InvalidOperationException ex)
            {
                /* learn.microsoft.com "DbContext Lifetime, Configuration, and Initialization"
				DbContext is not thread-safe. Do not share contexts between threads. 
				Make sure to await all async calls before continuing to use the context instance.
                 */
                Assert.AreEqual<string>("InvalidOperationException", ex.GetType().Name);
            }
            catch
            {
                Assert.Fail("SystemException!!");
            }
        }

        [TestMethod]
        public async Task TestDisposedDbContextDuringScopeFault()
        {
            // test case 1
            TodoDb storeIO = new Adapter<DbContext>(ProvideContext()).GetStorageIO<TodoDb>();
            using (TodoDb context = storeIO)
            {
                var listed = await context.Todos.ToListAsync<Order>();
            }

            // ! storeIO has been disposed!

            // test case 2
            try
            {
                int expectedCnt2 = 0;
                storeIO = new Adapter<DbContext>(ProvideContext()).GetStorageIO<TodoDb>();
                var emptyListed = await storeIO.Todos.ToListAsync<Order>();
                Assert.AreEqual<int>(expectedCnt2, emptyListed.Count);
            }
            catch (System.ObjectDisposedException ex)
            {
                Assert.AreEqual<string>("ObjectDisposedException", ex.GetType().Name);
            }
            catch
            {
                Assert.Fail();
            }
            finally
            {
                // clean up
                Context = null;
            }
        }

        //[TestMethod]
        public async Task TestDbSetPrimaryKeyIdentityOverflowFault()
        {
            TodoDb storeIO = new Adapter<DbContext>(ProvideContext()).GetStorageIO<TodoDb>();

            // test case 1
            Order od = new Order()
            {
                Id = uint.MaxValue,
                ProductId = "case24587",
                Quantity = 1
            };

            storeIO.Todos.Add(od);  // od is added origin
            int expectedCnt1 = 1;
            int insertedCnt1 = await storeIO.SaveChangesAsync();
            Assert.AreEqual<int>(expectedCnt1, insertedCnt1);

            // test case 2
            Order next = new Order()
            {
                ProductId = "case24588",
                Quantity = 1
            };

            try
            {
                storeIO.Todos.Add(next);  // od is added origin
                int expectedCnt2 = 1;
                int insertedCnt2 = await storeIO.SaveChangesAsync();
                Assert.AreEqual<int>(expectedCnt2, insertedCnt2);

                uint expectedId2 = 1;
                Order? found2 = await storeIO.FindAsync<Order>(expectedId2);
                Assert.IsNotNull(found2);
                Assert.AreEqual<uint?>(expectedId2, found2?.Id);
            }
            catch (System.OverflowException ex)
            {
                Assert.AreEqual<string>("OverflowException", ex.GetType().Name);
            }
            catch
            {
                Assert.Fail();
            }
            finally
            {
                // clean up
                storeIO.Remove(od);
                await storeIO.SaveChangesAsync();
            }
        }
    }
}