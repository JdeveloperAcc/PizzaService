using System;
using System.Collections.Generic;
using System.Globalization;
using System.Security.Cryptography;
using System.Text;

namespace PizzaService.Web.Models
{
    public class HashProvider
    {
        // Confirm processed data securely
        public static Submitted Calc(IEnumerable<string> ids, int status)
        {
            DateTime dt = DateTime.Now;

            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("{0}|",
                dt.AddSeconds(-10).ToString("G",
                  CultureInfo.CreateSpecificCulture("en-us")));
            foreach (string id in ids)
                sb.AppendFormat("{0}|", id);
            sb.Append(status.ToString().ToLower());

            using (SHA256 mySHA256 = SHA256.Create())
            {
                byte[] hashed = mySHA256.ComputeHash(Encoding.Default.GetBytes(sb.ToString()));
                return new Submitted()
                {
                    Qty = status,
                    Time = dt,
                    Hash = Convert.ToBase64String(hashed)
                };
            }
        }
    }
}
