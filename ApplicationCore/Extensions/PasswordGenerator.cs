using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationCore.Extensions
{
    public static class PasswordGenerator
    {
        public static string GeneratePassword()
        {
            var password = "";
            bool isUpper = false;
            bool isLower = false;
            bool isDigit = false;

            var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            var random = new Random();


            while (!isUpper || !isLower || !isDigit)
            {
                password = "";

                for (int i = 0; i < 5; i++)
                    password += chars[random.Next(chars.Length)];

                isUpper = password.Any(char.IsUpper);
                isLower = password.Any(char.IsLower);
                isDigit = password.Any(char.IsDigit);
            }

            password += ".";
            return password;
        }
    }
}
