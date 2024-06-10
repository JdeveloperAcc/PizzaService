using PizzaService.Web.Models;
using System.Text;
using System.Text.Json;

namespace PizzaService.Tests
{
    [TestClass]
    public sealed class S2SFeedConsoleTest
    {
        [TestMethod]
        public void TestJsonFormatted()
        {
            // test casse 1
            IEnumerable<Order> listed1 = new List<Order>();

            string expected1 = @"[]";
            string json1 = ToJsonString(listed1);

            Assert.AreEqual<string>(expected1, json1);

            // test case 2
            IEnumerable<Order> listed2 = new List<Order>()
            {
                new Order() {
                    Id = 2,
                    ProductId = "56",
                    Quantity = 5
                },
                new Order() {
                    Id = 4,
                    ProductId = "89",
                    Quantity = 8
                },
                new Order() {
                    Id = 8,
                    ProductId = "26",
                    Quantity = 5
                },
                new Order() {
                    Id = 10,
                    ProductId = "189",
                    Quantity = 2
                }
            };

            StringBuilder sb = new StringBuilder();
            sb.AppendLine("[");
            sb.AppendLine($"  {{{Environment.NewLine}    \"Id\": 2,{Environment.NewLine}    \"ProductId\": \"56\",{Environment.NewLine}    \"Quantity\": 5{Environment.NewLine}  }},");
            sb.AppendLine($"  {{{Environment.NewLine}    \"Id\": 4,{Environment.NewLine}    \"ProductId\": \"89\",{Environment.NewLine}    \"Quantity\": 8{Environment.NewLine}  }},");
            sb.AppendLine($"  {{{Environment.NewLine}    \"Id\": 8,{Environment.NewLine}    \"ProductId\": \"26\",{Environment.NewLine}    \"Quantity\": 5{Environment.NewLine}  }},");
            sb.AppendLine($"  {{{Environment.NewLine}    \"Id\": 10,{Environment.NewLine}    \"ProductId\": \"189\",{Environment.NewLine}    \"Quantity\": 2{Environment.NewLine}  }}");
            sb.Append("]");
            string expected2 = sb.ToString();
            string json2 = ToJsonString(listed2);

            int noPosition = -1;
            int diffIndex = S2SFeedConsoleTest.FindFirstDifferenceIndex(expected2, json2, out char x, out char y);
            Assert.AreEqual<int>(noPosition, diffIndex);
            Assert.AreEqual<string>(expected2, json2);
        }

        private string ToJsonString(IEnumerable<Order> lst)
        {
            return JsonSerializer.Serialize(lst, new JsonSerializerOptions
            {
                WriteIndented = true
            });
        }

        public static int FindFirstDifferenceIndex(string a, string b, out char aChar, out char bChar)
        {
            aChar = bChar = '\0';

            for (int i = 0; i < a.Length && i < b.Length; ++i)
            {
                char ax = a[i];
                char by = b[i];
                if (ax != by)
                {
                    aChar = ax;
                    bChar = by;
                    return i;
                }
            }

            int inx = -1;
            if (a.Length != b.Length)
            {
                if (a.Length < b.Length)
                {
                    inx = a.Length;
                    bChar = b[inx];
                }
                else
                {
                    inx = b.Length;
                    aChar = a[inx];
                }
            }

            return inx;
        }
    }
}
