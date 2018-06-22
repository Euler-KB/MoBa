using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MoneyMerchantLocator.API.Helpers
{
    public static class PasswordHelpers
    {
        static int IterationCount = 1000;

        public static string Generate(string plainPassword, out string salt)
        {
            byte[] passwordPayload;
            using (System.Security.Cryptography.Rfc2898DeriveBytes generator = new System.Security.Cryptography.Rfc2898DeriveBytes(plainPassword, 32))
            {
                generator.IterationCount = IterationCount;
                salt = Convert.ToBase64String(generator.Salt);
                passwordPayload = generator.GetBytes(32);
            }

            return Convert.ToBase64String(passwordPayload);
        }

        public static bool AreEqual(string salt, string hashed, string plain)
        {
            using (System.Security.Cryptography.Rfc2898DeriveBytes generator = new System.Security.Cryptography.Rfc2898DeriveBytes(plain, Convert.FromBase64String(salt)))
            {
                generator.IterationCount = IterationCount;
                return generator.GetBytes(32).SequenceEqual(Convert.FromBase64String(hashed));
            }
        }
    }
}