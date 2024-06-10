using Microsoft.Extensions.Configuration;

namespace PizzaService.Web.Models
{
    public class Cfg
    {
        private static class Key
        {
            public const string StoreInMemory = "StoreInMemory";

            public const string TimerIntervalMs = "TimerIntervalMs";

            public const string SubtractSent = "SubtractSent";
        }

        public StorageType Store { get; private set; }
        public int IntervalMs { get; private set; }
        public bool IsExtendedSync { get; private set; }

        protected Cfg() { }

        public static Cfg Read(IConfiguration configuration)
        {
            bool inMemory = bool.Parse(configuration[Key.StoreInMemory]);
            bool isExtendedSync = bool.Parse(configuration[Key.SubtractSent]);
            return new Cfg()
            {
                Store = inMemory ? StorageType.Memory : StorageType.Database,
                IntervalMs = int.Parse(configuration[Key.TimerIntervalMs]),
                IsExtendedSync = isExtendedSync
            };
        }
    }
}
