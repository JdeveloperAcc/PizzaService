using Microsoft.Extensions.Configuration;
using PizzaService.Web.Models;
using System.Timers;

namespace PizzaService.Tests
{
    [TestClass]
    public sealed class TimerTest
    {
        private static IConfiguration? Config = null;

        private static System.Timers.Timer? myTimer = null;
        private static int cnt = 0;

        public static void InitConfigRepository()
        {
            TimerTest.Config = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
        }

        public static void InitTimer()
        {            
            int intervalMs = Cfg.Read(Config).IntervalMs;
            TimerTest.myTimer = new System.Timers.Timer(intervalMs);
            TimerTest.myTimer.Elapsed += OnTimedEvent;
            TimerTest.myTimer.Enabled = true;
            TimerTest.cnt = 0;
        }

        [TestMethod]
        // Wait for ~ 40 seconds to finish the test.
        public async Task TestTicks()
        {
            TimerTest.InitConfigRepository();
            TimerTest.InitTimer();

            // test case 1
            int expectedCnt = 2;
            int intervalMs = Cfg.Read(Config).IntervalMs;   // 20000 ms
            int toCheckFinished = 40;                       // 40 ms
            int waitMs = expectedCnt * intervalMs + toCheckFinished;
            await Task.Delay(waitMs);                       // 40040 ms

            Assert.AreEqual<int>(expectedCnt, TimerTest.cnt);
        }

        private static async void OnTimedEvent(Object? source, ElapsedEventArgs e)
        {
            TimerTest.cnt++;
            await Task.Delay(10);
        }
    }
}